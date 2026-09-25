import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import { CheckCircle2, ChevronRight, Circle, Code2, Play, RotateCcw, TerminalSquare } from "lucide-react";

import { SiteHeader } from "@/components/site-header";
import { Button } from "@/components/ui/button";

export const Route = createFileRoute("/practice")({
  head: () => ({
    meta: [
      { title: "Technical Interview Practice — Interview Easy" },
      { name: "description", content: "Practice technical interview questions in a live code editor with automatic unit tests and instant results." },
      { name: "keywords", content: "coding interview practice, online code editor, automated unit tests, developer interview questions" },
      { property: "og:title", content: "Technical Interview Practice — Interview Easy" },
      { property: "og:description", content: "Solve realistic coding questions and run automated unit tests before your next interview." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: PracticePage,
});

const tests = [
  "returns [0, 1] for [2, 7, 11, 15]",
  "handles duplicate values",
  "returns an empty array when no pair exists",
];

function PracticePage() {
  const [hasRun, setHasRun] = useState(false);

  return (
    <div className="min-h-screen bg-cloud-gradient text-ink">
      <SiteHeader />
      <main className="container-fluid py-10 sm:py-14">
        <div className="flex flex-col gap-4 border-b border-glass-border pb-8 lg:flex-row lg:items-end lg:justify-between">
          <div className="max-w-3xl">
            <p className="text-sm font-semibold text-ai-accent">Practice workspace</p>
            <h1 className="mt-2 font-display text-4xl font-bold leading-tight tracking-normal sm:text-5xl">
              Write code. Run tests. <span className="text-ai-gradient">Improve fast.</span>
            </h1>
            <p className="mt-4 max-w-2xl text-lg text-soft-ink">
              Work through realistic interview questions with a focused editor and automatic unit tests.
            </p>
          </div>
          <Button asChild variant="glass" size="hero">
            <Link to="/" hash="topic-menu">Choose another topic <ChevronRight aria-hidden="true" /></Link>
          </Button>
        </div>

        <section className="mt-8 overflow-hidden rounded-2xl border border-glass-border bg-glass shadow-ai-card backdrop-blur-xl">
          <div className="grid lg:grid-cols-[0.82fr_1.18fr]">
            <div className="border-b border-glass-border p-5 sm:p-7 lg:border-b-0 lg:border-r">
              <div className="flex items-center justify-between gap-3">
                <span className="rounded-full bg-brand-soft px-3 py-1 text-xs font-semibold text-brand">Arrays · Medium</span>
                <span className="text-xs font-medium text-faint-ink">25 min</span>
              </div>
              <h2 className="mt-5 font-display text-2xl font-bold tracking-normal">Two Sum</h2>
              <p className="mt-3 text-sm leading-6 text-soft-ink">
                Given an array of integers and a target, return the indices of two numbers that add up to the target.
              </p>
              <div className="mt-6 border-t border-glass-border pt-5">
                <h3 className="text-sm font-semibold">Example</h3>
                <pre className="mt-3 overflow-x-auto rounded-lg bg-ink p-4 font-mono text-xs leading-6 text-brand-foreground">{`Input: nums = [2, 7, 11, 15]\nTarget: 9\nOutput: [0, 1]`}</pre>
              </div>
              <div className="mt-6 flex gap-3">
                <Button type="button" variant="glass" onClick={() => setHasRun(false)}>
                  <RotateCcw aria-hidden="true" /> Reset
                </Button>
                <Button type="button" variant="hero" onClick={() => setHasRun(true)}>
                  <Play aria-hidden="true" /> Run tests
                </Button>
              </div>
            </div>

            <div className="min-w-0 bg-ink text-brand-foreground">
              <div className="flex h-12 items-center justify-between border-b border-glass-border px-4">
                <span className="flex items-center gap-2 text-xs font-semibold"><Code2 aria-hidden="true" /> solution.js</span>
                <span className="text-xs text-faint-ink">JavaScript</span>
              </div>
              <pre className="min-h-72 overflow-x-auto p-5 font-mono text-sm leading-7 sm:p-7">{`function twoSum(nums, target) {
  const seen = new Map();

  for (let i = 0; i < nums.length; i++) {
    const complement = target - nums[i];
    if (seen.has(complement)) {
      return [seen.get(complement), i];
    }
    seen.set(nums[i], i);
  }

  return [];
}`}</pre>
              <div className="border-t border-glass-border bg-background p-5 text-ink sm:p-6">
                <div className="flex items-center justify-between gap-3">
                  <h3 className="flex items-center gap-2 font-display font-semibold"><TerminalSquare className="text-brand" aria-hidden="true" /> Test results</h3>
                  {hasRun && <span className="text-xs font-semibold text-success">3 of 3 passed</span>}
                </div>
                <ul className="mt-4 grid gap-3">
                  {tests.map((test) => (
                    <li key={test} className="flex items-center gap-3 text-sm text-soft-ink">
                      {hasRun ? <CheckCircle2 className="size-4 shrink-0 text-success" aria-hidden="true" /> : <Circle className="size-4 shrink-0 text-faint-ink" aria-hidden="true" />}
                      {test}
                    </li>
                  ))}
                </ul>
              </div>
            </div>
          </div>
        </section>
      </main>
    </div>
  );
}