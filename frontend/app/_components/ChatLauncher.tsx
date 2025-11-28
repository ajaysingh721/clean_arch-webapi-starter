'use client';

import { useState } from 'react';
import { MessageCircle } from 'lucide-react';

import { Button } from '@/components/ui/button';
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from '@/components/ui/sheet';
import { ChatPanel } from './ChatPanel';

export function ChatLauncher() {
  const [open, setOpen] = useState(false);

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <div className="fixed bottom-5 right-5 z-40 flex flex-col items-end gap-2 sm:bottom-8 sm:right-8">
        <p className="hidden text-xs uppercase tracking-[0.4em] text-muted-foreground sm:block">Need help?</p>
        <SheetTrigger asChild>
          <Button
            size="lg"
            className="flex items-center gap-3 rounded-full bg-primary px-5 py-3 text-base font-semibold text-primary-foreground shadow-xl shadow-primary/40 hover:translate-y-0.5"
          >
            <span className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-primary-foreground/10">
              <MessageCircle className="h-4 w-4 text-primary-foreground" />
            </span>
            {open ? 'Hide chat' : 'Chat with Copilot'}
          </Button>
        </SheetTrigger>
      </div>
      <SheetContent side="right" className="flex h-full w-full flex-col border-l border-border/40 bg-background/95 sm:max-w-3xl">
        <SheetHeader>
          <SheetTitle>CleanArch GPT Mock</SheetTitle>
          <SheetDescription>Live sandbox backed by the deterministic mock completion service.</SheetDescription>
        </SheetHeader>
        <div className="flex-1 overflow-y-auto px-2 pb-6">
          <ChatPanel />
        </div>
      </SheetContent>
    </Sheet>
  );
}
