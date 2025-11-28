'use client';

import { FormEvent, useCallback, useEffect, useMemo, useRef, useState } from 'react';

import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Textarea } from '@/components/ui/textarea';
import { cn } from '@/lib/utils';

type MessageRole = 'user' | 'assistant' | 'system';

type Message = {
  id: string;
  role: MessageRole;
  content: string;
  pending?: boolean;
  createdAt: number;
};

type ChatMessageDto = {
  role: MessageRole;
  content: string;
};

type ChatCompletionResponse = {
  assistantMessage: ChatMessageDto;
  conversation: ChatMessageDto[];
  usage: {
    promptTokens: number;
    completionTokens: number;
    totalTokens: number;
  };
  model: string;
  createdUtc: string;
};

const API_BASE = process.env.NEXT_PUBLIC_API_BASE_URL ?? '';
const COMPLETIONS_ENDPOINT = API_BASE
  ? `${API_BASE.replace(/\/$/, '')}/api/chat/completions`
  : '/api/chat/completions';

const uniqueId = () =>
  typeof crypto !== 'undefined' && 'randomUUID' in crypto
    ? crypto.randomUUID()
    : Math.random().toString(36).slice(2);

const buildMessage = (role: MessageRole, content: string, pending = false): Message => ({
  id: uniqueId(),
  role,
  content,
  pending,
  createdAt: Date.now(),
});

export function ChatPanel() {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [usage, setUsage] = useState<ChatCompletionResponse['usage'] | null>(null);
  const abortController = useRef<AbortController | null>(null);
  const streamTimers = useRef<Set<number>>(new Set());
  const scrollAnchor = useRef<HTMLDivElement | null>(null);
  const lastPromptRef = useRef<string | null>(null);

  const stableHistory = useMemo<ChatMessageDto[]>(
    () =>
      messages
        .filter((message) => !message.pending)
        .map((message) => ({ role: message.role, content: message.content })),
    [messages],
  );

  useEffect(() => {
    const timers = streamTimers.current;
    return () => {
      abortController.current?.abort();
      timers.forEach((timerId) => window.clearInterval(timerId));
      timers.clear();
    };
  }, []);

  useEffect(() => {
    scrollAnchor.current?.scrollIntoView({ behavior: 'smooth', block: 'end' });
  }, [messages]);

  const startStreaming = useCallback((messageId: string, fullText: string) => {
    streamTimers.current.forEach((timerId) => window.clearInterval(timerId));
    streamTimers.current.clear();

    let index = 0;
    const intervalId = window.setInterval(() => {
      index = Math.min(fullText.length, index + 6);
      setMessages((prev) =>
        prev.map((message) =>
          message.id === messageId
            ? {
                ...message,
                content: fullText.slice(0, index),
                pending: index < fullText.length,
              }
            : message,
        ),
      );

      if (index >= fullText.length) {
        window.clearInterval(intervalId);
        streamTimers.current.delete(intervalId);
      }
    }, 18);

    streamTimers.current.add(intervalId);
  }, []);

  const sendPrompt = useCallback(
    async (prompt: string, historyOverride?: ChatMessageDto[]) => {
      if (!prompt.trim()) {
        setError('Please enter a prompt before sending.');
        return;
      }

      if (isLoading) {
        return;
      }

      const controller = new AbortController();
      abortController.current?.abort();
      abortController.current = controller;

      const userMessage = buildMessage('user', prompt.trim());
      const assistantPlaceholder = buildMessage('assistant', '', true);

      setMessages((prev) => [...prev.filter((m) => !m.pending), userMessage, assistantPlaceholder]);
      setInput('');
      setError(null);
      setIsLoading(true);
      lastPromptRef.current = prompt.trim();

      try {
        const payload = {
          prompt: prompt.trim(),
          history: historyOverride ?? stableHistory,
        };

        const response = await fetch(COMPLETIONS_ENDPOINT, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload),
          signal: controller.signal,
        });

        if (!response.ok) {
          const message = await response.text();
          throw new Error(message || 'The assistant could not complete your request.');
        }

        const completion: ChatCompletionResponse = await response.json();
        setUsage(completion.usage);

        const normalizedConversation = completion.conversation.map((message, index, array) => {
          const isLatestAssistant = index === array.length - 1 && message.role === 'assistant';
          return {
            ...buildMessage(message.role, isLatestAssistant ? '' : message.content),
            content: isLatestAssistant ? '' : message.content,
            pending: isLatestAssistant,
          };
        });

        setMessages(normalizedConversation);
        const newestAssistant = normalizedConversation[normalizedConversation.length - 1];
        if (newestAssistant) {
          startStreaming(newestAssistant.id, completion.assistantMessage.content);
        }
      } catch (err) {
        if ((err as Error).name === 'AbortError') {
          setError('Generation cancelled.');
          return;
        }

        const message = err instanceof Error ? err.message : 'Something went wrong.';
        setError(message);
        setMessages((prev) => prev.filter((m) => m.id !== assistantPlaceholder.id));
      } finally {
        setIsLoading(false);
        abortController.current = null;
      }
    },
    [isLoading, stableHistory, startStreaming],
  );

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    void sendPrompt(input);
  };

  const handleRetry = () => {
    if (!lastPromptRef.current) {
      return;
    }

    const historyBeforeLastTurn = stableHistory.slice(0, Math.max(0, stableHistory.length - 2));
    void sendPrompt(lastPromptRef.current, historyBeforeLastTurn);
  };

  const handleAbort = () => {
    abortController.current?.abort();
    abortController.current = null;
    setIsLoading(false);
  };

  const handleClear = () => {
    abortController.current?.abort();
    abortController.current = null;
    setIsLoading(false);
    setMessages([]);
    setUsage(null);
    setError(null);
    lastPromptRef.current = null;
  };

  return (
    <Card className="w-full border-border/40 bg-background/80 shadow-2xl backdrop-blur">
      <CardHeader className="space-y-4">
        <div className="flex flex-wrap items-center justify-between gap-4">
          <div className="space-y-2">
            <Badge variant="outline" className="px-3 py-1 text-[11px] tracking-[0.4em]">
              MODEL
            </Badge>
            <CardTitle className="text-2xl">GPT Mock v1 · deterministic</CardTitle>
            <CardDescription>
              Multi-turn assistant wired to <code className="rounded bg-muted px-1">/api/chat/completions</code>.
            </CardDescription>
          </div>
          <div className="flex gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleRetry}
              disabled={!lastPromptRef.current || isLoading}
            >
              Retry last
            </Button>
            <Button type="button" variant="ghost" size="sm" onClick={handleClear}>
              Clear chat
            </Button>
          </div>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <ScrollArea className="h-[360px] rounded-2xl border border-border/40 bg-muted/5 p-4">
          {messages.length === 0 && (
            <div className="text-sm text-muted-foreground">
              <p className="font-semibold text-foreground">Welcome! 👋</p>
              <p>
                Ask anything about your project, draft release notes, or explore requirements. I keep the entire conversation in
                context.
              </p>
            </div>
          )}

          <div className="space-y-3">
            {messages.map((message) => (
              <div
                key={message.id}
                className={cn(
                  'flex flex-col gap-2 rounded-2xl border px-4 py-3 text-sm shadow-sm transition',
                  message.role === 'user'
                    ? 'border-primary/40 bg-primary/10 text-primary-foreground'
                    : 'border-border/60 bg-card/80 text-card-foreground',
                )}
              >
                <div className="text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground">
                  {message.role === 'user' ? 'You' : 'Assistant'}
                </div>
                <p className="whitespace-pre-line text-sm leading-relaxed">{message.content}</p>
                {message.pending && <span className="text-xs text-muted-foreground">streaming...</span>}
              </div>
            ))}
          </div>
          <div ref={scrollAnchor} />
        </ScrollArea>

        {error && <p className="text-sm text-destructive">{error}</p>}

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <Textarea
            placeholder="Share context, ask follow-up questions, or paste tasks you need help with..."
            value={input}
            onChange={(event) => setInput(event.target.value)}
            disabled={isLoading}
          />
          <div className="flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
            <div className="flex gap-2">
              <Button type="submit" disabled={isLoading} size="lg">
                {isLoading ? 'Thinking…' : 'Send message'}
              </Button>
              <Button type="button" variant="outline" disabled={!isLoading} onClick={handleAbort}>
                Stop
              </Button>
            </div>
            {usage && (
              <div className="flex flex-wrap gap-3 text-xs">
                <span>Prompt tokens: {usage.promptTokens}</span>
                <span>Completion tokens: {usage.completionTokens}</span>
                <span>Total: {usage.totalTokens}</span>
              </div>
            )}
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
