import { createFileRoute, Link } from "@tanstack/react-router";
import { CalendarCheck, CheckCircle2, Code2, FileBarChart, Play, UsersRound, Video } from "lucide-react";

import { SiteHeader } from "@/components/site-header";
import { Button } from "@/components/ui/button";

export const Route = createFileRoute("/demo")({
  head: () => ({
    meta: [
      { title: "Interview Easy Product Demo — See the Complete Interview Flow" },
      { name: "description", content: "See how Interview Easy handles scheduling, live video, coding questions, automatic tests, scorecards, and candidate reports." },
      { name: "keywords", content: "technical interview demo, video interview platform, coding assessment demo, interview scorecards" },
      { property: "og:title", content: "Interview Easy Product Demo" },
      { property: "og:description", content: "Tour the complete technical interview workflow, from scheduling to candidate reports." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: DemoPage,
});

const tour = [
  { icon: CalendarCheck, title: "Schedule", text: "Pick a time, interview panel, role, and question set." },
  { icon: Video, title: "Meet", text: "Join a focused video room with the candidate and panel." },
  { icon: Code2, title: "Assess", text: "Share questions, write code together, and run unit tests." },
  { icon: FileBarChart, title: "Decide", text: "Combine scorecards into one clear candidate report." },
] as const;

function DemoPage() {
  return (
    <div className="min-h-screen bg-cloud-gradient text-ink">
      <SiteHeader />
      <main>
        <section className="container-fluid grid items-center gap-10 py-12 lg:grid-cols-[0.8fr_1.2fr] lg:py-16">
          <div>
            <p className="text-sm font-semibold text-ai-accent">Product tour</p>
            <h1 className="mt-2 font-display text-4xl font-bold leading-tight tracking-normal sm:text-5xl">One place for every part of the interview</h1>
            <p className="mt-4 max-w-xl text-lg text-soft-ink">Walk through scheduling, live video, structured questions, collaborative coding, automated tests, and team decisions.</p>
            <div className="mt-7 flex flex-wrap gap-3">
              <Button asChild variant="hero" size="hero"><Link to="/contact">Schedule a live demo</Link></Button>
              <Button asChild variant="glass" size="hero"><Link to="/practice"><Play aria-hidden="true" /> Try the code workspace</Link></Button>
            </div>
          </div>

          <div className="overflow-hidden rounded-2xl border border-glass-border bg-glass shadow-ai-card backdrop-blur-xl">
            <div className="flex items-center justify-between border-b border-glass-border px-5 py-4">
              <div><p className="text-xs font-semibold text-ai-accent">LIVE INTERVIEW</p><h2 className="font-display font-semibold">Senior Backend Engineer</h2></div>
              <span className="rounded-full bg-success-soft px-3 py-1 text-xs font-semibold text-success">00:32:18</span>
            </div>
            <div className="grid min-h-80 sm:grid-cols-[1.35fr_0.65fr]">
              <div className="bg-ink p-5 font-mono text-sm leading-7 text-brand-foreground">
                <p className="mb-5 text-xs text-faint-ink">candidate-solution.cs</p>
                <pre className="overflow-x-auto">{`public int[] TwoSum(int[] nums, int target)
{
  var seen = new Dictionary<int, int>();
  for (var i = 0; i < nums.Length; i++)
  {
    var needed = target - nums[i];
    if (seen.ContainsKey(needed))
      return new[] { seen[needed], i };
    seen[nums[i]] = i;
  }
  return Array.Empty<int>();
}`}</pre>
              </div>
              <div className="border-t border-glass-border p-5 sm:border-l sm:border-t-0">
                <UsersRound className="text-brand" aria-hidden="true" />
                <p className="mt-3 font-display font-semibold">Panel</p>
                <p className="mt-1 text-sm text-soft-ink">Priya, Alex + candidate</p>
                <div className="mt-6 border-t border-glass-border pt-5">
                  <p className="text-xs font-semibold text-faint-ink">AUTOMATED TESTS</p>
                  {["Base case", "Duplicates", "No match"].map((test) => <p key={test} className="mt-3 flex items-center gap-2 text-sm"><CheckCircle2 className="size-4 text-success" aria-hidden="true" /> {test}</p>)}
                </div>
              </div>
            </div>
          </div>
        </section>

        <section className="container-fluid py-10">
          <div className="grid gap-6 border-y border-glass-border py-10 md:grid-cols-4">
            {tour.map((item, index) => <article key={item.title}><span className="text-xs font-bold text-faint-ink">0{index + 1}</span><item.icon className="mt-4 text-brand" aria-hidden="true" /><h2 className="mt-3 font-display text-lg font-semibold">{item.title}</h2><p className="mt-1 text-sm leading-6 text-soft-ink">{item.text}</p></article>)}
          </div>
        </section>
      </main>
    </div>
  );
}