import { createFileRoute } from "@tanstack/react-router";
import { ArrowRight, BookOpen, Code2, Play, Search, Sparkles } from "lucide-react";

import { Button } from "@/components/ui/button";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "Interview Easy — Developer Interview Questions, Videos, and Practice" },
      {
        name: "description",
        content:
          "Prepare for developer interviews with searchable questions, clear explanations, video walkthroughs, code examples, and practice prompts.",
      },
      {
        property: "og:title",
        content: "Interview Easy — Developer Interview Questions, Videos, and Practice",
      },
      {
        property: "og:description",
        content:
          "Search developer interview topics, learn each answer with text and video, then practice with examples.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: Index,
});

const topics = [
  {
    icon: "🧠",
    title: "Data Structures",
    description: "Arrays, trees, graphs and hashing",
    count: "2,140 questions",
  },
  {
    icon: "⚙️",
    title: "System Design",
    description: "Scaling, caching and consistency",
    count: "860 questions",
  },
  {
    icon: "🔌",
    title: "Backend & APIs",
    description: "REST, auth, queues and databases",
    count: "1,320 questions",
  },
  {
    icon: "🎨",
    title: "Frontend",
    description: "React, CSS and performance",
    count: "1,780 questions",
  },
];

const steps = [
  {
    number: "1",
    title: "Pick a topic",
    description: "Filter by role, level, and difficulty to build your practice session.",
    tone: "bg-brand-soft text-brand",
  },
  {
    number: "2",
    title: "Learn the why",
    description: "Read the explanation, watch the video, and study the example.",
    tone: "bg-ai-accent-soft text-ai-accent",
  },
  {
    number: "3",
    title: "Practice & track",
    description: "Solve, self-grade, and watch your confidence climb over time.",
    tone: "bg-success-soft text-success",
  },
];

function Index() {
  return (
    <div className="relative min-h-screen w-full overflow-hidden bg-cloud-gradient text-ink">
      <div className="pointer-events-none absolute -left-24 -top-24 size-[420px] rounded-full bg-brand-glow blur-[120px]" />
      <div className="pointer-events-none absolute -right-24 top-1/3 size-[420px] rounded-full bg-ai-accent-soft blur-[120px]" />
      <div className="pointer-events-none absolute bottom-0 left-1/3 size-[360px] rounded-full bg-cloud-cool blur-[120px]" />

      <header className="relative z-20 mx-auto max-w-6xl px-4 pt-5 sm:px-6 sm:pt-6">
        <nav className="flex items-center justify-between rounded-2xl border border-glass-border bg-glass px-4 py-3 shadow-ai-soft backdrop-blur-xl sm:px-5">
          <a href="#top" className="flex items-center gap-2" aria-label="Interview Easy home">
            <span className="grid size-9 place-items-center rounded-xl bg-ai-gradient font-display text-lg font-bold text-brand-foreground">
              I
            </span>
            <span className="font-display text-lg font-bold tracking-tight">
              Interview<span className="text-brand">Easy</span>
            </span>
          </a>
          <div className="hidden items-center gap-7 text-sm font-medium text-soft-ink md:flex">
            <a href="#questions" className="transition-colors hover:text-brand">
              Questions
            </a>
            <a href="#practice" className="transition-colors hover:text-brand">
              Practice
            </a>
            <a href="#roadmap" className="transition-colors hover:text-brand">
              Roadmaps
            </a>
            <a href="#pricing" className="transition-colors hover:text-brand">
              Pricing
            </a>
          </div>
          <div className="flex items-center gap-3">
            <a className="hidden text-sm font-medium text-soft-ink hover:text-brand sm:inline" href="#questions">
              Sign in
            </a>
            <Button asChild variant="hero" size="sm" className="rounded-xl px-4 py-2">
              <a href="#questions">Get started</a>
            </Button>
          </div>
        </nav>
      </header>

      <main id="top" className="relative z-10">
        <section className="mx-auto grid max-w-6xl items-center gap-12 px-4 pb-10 pt-14 sm:px-6 lg:grid-cols-2">
          <div>
            <span className="inline-flex items-center gap-2 rounded-full border border-glass-border bg-glass px-3 py-1 text-xs font-semibold text-ai-accent shadow-sm backdrop-blur-md">
              <span className="size-2 rounded-full bg-ai-accent" /> 12,000+ questions with video
            </span>
            <h1 className="mt-5 font-display text-5xl font-bold leading-[1.05] tracking-normal sm:text-6xl">
              Ace your next <span className="text-ai-gradient">developer interview</span>
            </h1>
            <p className="mt-5 max-w-md text-lg text-soft-ink">
              Every question paired with a clear explanation, a walkthrough video, and a runnable example — so you practice until it becomes second nature.
            </p>
            <div className="mt-7 flex flex-wrap gap-3">
              <Button asChild variant="hero" size="hero">
                <a href="#questions">
                  Start practicing free <ArrowRight aria-hidden="true" />
                </a>
              </Button>
              <Button asChild variant="glass" size="hero">
                <a href="#practice">
                  <Play aria-hidden="true" /> Watch a sample
                </a>
              </Button>
            </div>
            <dl className="mt-8 flex flex-wrap gap-8">
              <div>
                <dt className="font-display text-2xl font-bold">12k+</dt>
                <dd className="text-xs text-faint-ink">Questions</dd>
              </div>
              <div>
                <dt className="font-display text-2xl font-bold">3.4k</dt>
                <dd className="text-xs text-faint-ink">Videos</dd>
              </div>
              <div>
                <dt className="font-display text-2xl font-bold">98%</dt>
                <dd className="text-xs text-faint-ink">Pass rate</dd>
              </div>
            </dl>
          </div>

          <div className="animate-floaty">
            <article className="rounded-3xl border border-glass-border bg-glass p-5 shadow-ai-card backdrop-blur-xl sm:p-6">
              <div className="flex items-center justify-between">
                <span className="rounded-full bg-brand-soft px-3 py-1 text-xs font-semibold text-brand">
                  System Design
                </span>
                <span className="text-xs font-medium text-faint-ink">Hard</span>
              </div>
              <h2 className="mt-4 font-display text-xl font-semibold leading-snug">
                Design a rate limiter for a public API
              </h2>
              <p className="mt-2 text-sm text-soft-ink">
                Token bucket vs sliding window — trade-offs, edge cases, and a working example.
              </p>
              <div className="mt-5 grid grid-cols-3 gap-3">
                <div className="rounded-xl border border-glass-border bg-glass-strong p-3 text-center">
                  <div className="font-display font-bold text-brand">04:12</div>
                  <div className="mt-1 text-[11px] text-faint-ink">Video</div>
                </div>
                <div className="rounded-xl border border-glass-border bg-glass-strong p-3 text-center">
                  <div className="font-display font-bold text-ai-accent">12</div>
                  <div className="mt-1 text-[11px] text-faint-ink">Steps</div>
                </div>
                <div className="rounded-xl border border-glass-border bg-glass-strong p-3 text-center">
                  <div className="font-display font-bold text-success">4.9</div>
                  <div className="mt-1 text-[11px] text-faint-ink">Rating</div>
                </div>
              </div>
              <Button asChild variant="hero" size="hero" className="mt-5 w-full">
                <a href="#questions">Open question</a>
              </Button>
            </article>
          </div>
        </section>

        <section id="questions" className="mx-auto max-w-6xl px-4 py-8 sm:px-6">
          <div className="mb-5 flex items-end justify-between gap-4">
            <h2 className="font-display text-2xl font-bold tracking-normal">Browse by topic</h2>
            <a className="text-sm font-semibold text-brand transition-colors hover:text-ai-accent" href="#practice">
              View all
            </a>
          </div>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {topics.map((topic) => (
              <article
                key={topic.title}
                className="rounded-2xl border border-glass-border bg-glass p-5 backdrop-blur-xl transition-colors hover:bg-glass-strong"
              >
                <div className="text-3xl" aria-hidden="true">
                  {topic.icon}
                </div>
                <h3 className="mt-3 font-display font-semibold">{topic.title}</h3>
                <p className="mt-1 text-sm text-soft-ink">{topic.description}</p>
                <div className="mt-4 text-xs font-semibold text-brand">{topic.count}</div>
              </article>
            ))}
          </div>
        </section>

        <section id="practice" className="mx-auto max-w-6xl px-4 py-8 sm:px-6">
          <div className="grid gap-5 lg:grid-cols-[0.9fr_1.1fr]">
            <div className="rounded-3xl border border-glass-border bg-glass p-6 shadow-ai-soft backdrop-blur-xl">
              <div className="inline-flex size-11 items-center justify-center rounded-xl bg-ai-accent-soft text-ai-accent">
                <Search aria-hidden="true" />
              </div>
              <h2 className="mt-4 font-display text-2xl font-bold tracking-normal">Search, learn, and practice in one place</h2>
              <p className="mt-2 text-sm text-soft-ink">
                Use Interview Easy to jump from a question to its explanation, video, code example, and practice prompt without losing focus.
              </p>
            </div>
            <div className="rounded-3xl border border-glass-border bg-glass p-5 shadow-ai-soft backdrop-blur-xl">
              <div className="rounded-2xl border border-glass-border bg-ink p-4 font-mono text-sm text-brand-foreground">
                <div className="text-faint-ink">// Two Sum example</div>
                <div>const seen = new Map();</div>
                <div>for (let i = 0; i &lt; nums.length; i++) &#123;</div>
                <div className="pl-4">const need = target - nums[i];</div>
                <div className="pl-4">if (seen.has(need)) return [seen.get(need), i];</div>
                <div className="pl-4">seen.set(nums[i], i);</div>
                <div>&#125;</div>
              </div>
              <div className="mt-4 grid gap-3 sm:grid-cols-3">
                <div className="rounded-xl border border-glass-border bg-glass-strong p-3">
                  <BookOpen className="text-brand" aria-hidden="true" />
                  <p className="mt-2 text-sm font-semibold">Explanation</p>
                </div>
                <div className="rounded-xl border border-glass-border bg-glass-strong p-3">
                  <Play className="text-ai-accent" aria-hidden="true" />
                  <p className="mt-2 text-sm font-semibold">Video</p>
                </div>
                <div className="rounded-xl border border-glass-border bg-glass-strong p-3">
                  <Code2 className="text-success" aria-hidden="true" />
                  <p className="mt-2 text-sm font-semibold">Example</p>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section id="roadmap" className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
          <div className="rounded-3xl border border-glass-border bg-glass p-6 shadow-ai-soft backdrop-blur-xl sm:p-8">
            <div className="flex items-center gap-3">
              <Sparkles className="text-ai-accent" aria-hidden="true" />
              <h2 className="font-display text-2xl font-bold tracking-normal">How it works</h2>
            </div>
            <div className="mt-6 grid gap-6 md:grid-cols-3">
              {steps.map((step) => (
                <article key={step.number} className="rounded-2xl border border-glass-border bg-glass-strong p-5">
                  <div className={`grid size-10 place-items-center rounded-xl font-display font-bold ${step.tone}`}>
                    {step.number}
                  </div>
                  <h3 className="mt-4 font-display font-semibold">{step.title}</h3>
                  <p className="mt-1 text-sm text-soft-ink">{step.description}</p>
                </article>
              ))}
            </div>
          </div>
        </section>

        <section id="pricing" className="mx-auto max-w-6xl px-4 pb-4 pt-2 sm:px-6">
          <div className="rounded-3xl border border-glass-border bg-glass p-6 text-center shadow-ai-soft backdrop-blur-xl sm:p-8">
            <h2 className="font-display text-2xl font-bold tracking-normal">Start with free practice</h2>
            <p className="mx-auto mt-2 max-w-xl text-sm text-soft-ink">
              Explore the question library first. Add paid plans later when you are ready to offer progress tracking, saved answers, and team preparation.
            </p>
            <Button asChild variant="hero" size="hero" className="mt-5">
              <a href="#questions">Browse questions</a>
            </Button>
          </div>
        </section>
      </main>

      <footer className="relative z-10 mx-auto max-w-6xl px-4 py-10 text-center text-sm text-soft-ink sm:px-6">
        InterviewEasy — practice smarter, not harder. © 2026
      </footer>
    </div>
  );
}
