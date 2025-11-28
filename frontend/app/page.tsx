import { ChatLauncher } from "./_components/ChatLauncher";

export default function Home() {
  return (
    <div className="min-h-screen bg-gradient-to-b from-black via-zinc-950 to-zinc-900 text-white">
      <div className="mx-auto flex max-w-6xl flex-col gap-10 px-6 py-12">
        <header className="flex flex-col gap-4">
          <p className="text-sm uppercase tracking-[0.4em] text-emerald-300">Conversational workspace</p>
          <h1 className="text-4xl font-semibold sm:text-5xl">Full-stack chatbot playground</h1>
          <p className="max-w-3xl text-base text-zinc-300">
            Chat with the mock GPT service wired into our Clean Architecture API. Tap the launcher in the bottom-right corner to
            open the sandbox, stream responses, and monitor usage like a modern production chatbot.
          </p>
        </header>

        <section className="grid gap-4 border-t border-white/10 pt-6 text-sm text-zinc-400 sm:grid-cols-3">
          <article>
            <h2 className="text-base font-semibold text-white">Multi-turn memory</h2>
            <p>The backend stores up to twenty prior turns so you can iterate and branch conversations confidently.</p>
          </article>
          <article>
            <h2 className="text-base font-semibold text-white">Deterministic replies</h2>
            <p>A seeded mock LLM guarantees reproducible answers, simplifying demos, tests, and local debugging.</p>
          </article>
          <article>
            <h2 className="text-base font-semibold text-white">LLM-style telemetry</h2>
            <p>Usage metrics mirror real completions, providing a blueprint for enforcing budgets or monitoring UX.</p>
          </article>
        </section>
      </div>

      <ChatLauncher />
    </div>
  );
}
