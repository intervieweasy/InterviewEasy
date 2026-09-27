import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import {
  ArrowRight,
  ArrowUpRight,
  BookOpen,
  BrainCircuit,
  CalendarCheck,
  CheckCircle2,
  ChevronLeft,
  ChevronRight,
  Code2,
  FileBarChart,
  MonitorUp,
  Play,
  Radio,
  Search,
  TestTube2,
  UsersRound,
  Video,
} from "lucide-react";

const workflow = [
  { icon: CalendarCheck, title: "Schedule", description: "Set the role, panel, time, and structured question set." },
  { icon: UsersRound, title: "Invite", description: "Send one secure interview link to everyone involved." },
  { icon: Video, title: "Interview", description: "Meet on video and move through prepared questions." },
  { icon: TestTube2, title: "Test code", description: "Run the candidate solution against automated unit tests." },
  { icon: FileBarChart, title: "Decide", description: "Compare scorecards and share a candidate report." },
];

import { Button } from "@/components/ui/button";
import { SiteHeader } from "@/components/site-header";
import { menuGroups } from "@/lib/topics";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "Interview Easy — Schedule and Run Technical Interviews" },
      {
        name: "description",
        content:
          "Schedule and manage technical interviews with live video, structured questions, a collaborative code editor, automated unit tests, and candidate reports.",
      },
      {
        name: "keywords",
        content:
          "technical interview platform, interview scheduling, live coding interview, automated unit tests, interview questions, candidate scorecards",
      },
      {
        property: "og:title",
        content: "Interview Easy — Schedule and Run Technical Interviews",
      },
      {
        property: "og:description",
        content:
          "Schedule candidates, meet on video, assess code with automated tests, and make structured hiring decisions.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: Index,
});

const topics = menuGroups.map(({ title, description, Icon, tone, topics: groupTopics }) => ({
  Icon,
  tone,
  title,
  description,
  count: `${groupTopics.length} topic areas`,
}));

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

const featuredVideos = [
  {
    title: "C# Interview Questions and Answers",
    topic: "C#",
    duration: "18 min",
    embedUrl: "https://www.youtube-nocookie.com/embed/8msl4Y8oIoU",
  },
  {
    title: "ASP.NET Core Web API Interview Guide",
    topic: "ASP.NET Core",
    duration: "16 min",
    embedUrl: "https://www.youtube-nocookie.com/embed/fmvcAzHpsk8",
  },
  {
    title: "JavaScript Interview Questions",
    topic: "JavaScript",
    duration: "13 min",
    embedUrl: "https://www.youtube-nocookie.com/embed/9YkUCxvaLEk",
  },
] as const;

const features = [
  {
    icon: BrainCircuit,
    label: "AI-assisted interviews",
    title: "Keep every interview focused.",
    description: "Bring relevant technical questions and consistent evaluation prompts into the conversation, so your team can spend more time getting to know the candidate.",
    detail: "Structured questions",
    scene: "ai",
  },
  {
    icon: CalendarCheck,
    label: "Easy scheduling",
    title: "From availability to interview in fewer steps.",
    description: "Coordinate candidates, interviewers, and question sets in one place. Give everyone a clear plan before the call begins.",
    detail: "One shared interview plan",
    scene: "schedule",
  },
  {
    icon: MonitorUp,
    label: "Screen sharing",
    title: "See the thinking, not just the answer.",
    description: "Walk through architecture, debug a problem together, and discuss real work with a shared screen alongside the conversation.",
    detail: "Collaborative review",
    scene: "share",
  },
  {
    icon: Radio,
    label: "Video recording",
    title: "Make room to review the details.",
    description: "Keep the interview conversation and its key moments together for a more thoughtful panel review after the session.",
    detail: "Reviewable sessions",
    scene: "record",
  },
  {
    icon: TestTube2,
    label: "Automated test cases",
    title: "Let working code show the way.",
    description: "Pair coding questions with test cases and make the results part of the discussion instead of relying on guesswork.",
    detail: "Clear test results",
    scene: "tests",
  },
  {
    icon: FileBarChart,
    label: "Decision-ready feedback",
    title: "Leave with a clearer decision.",
    description: "Bring interview notes, coding results, and panel feedback together for a more consistent candidate review.",
    detail: "Panel scorecards",
    scene: "feedback",
  },
] as const;

function FeatureScene({ scene }: { scene: (typeof features)[number]["scene"] }) {
  return (
    <div className="flex min-h-72 flex-col overflow-hidden rounded-md border border-glass-border bg-glass-strong shadow-ai-soft sm:min-h-80">
      <div className="flex h-11 items-center justify-between border-b border-glass-border px-4 text-xs font-semibold text-soft-ink">
        <span className="flex items-center gap-2"><span className="size-2 rounded-full bg-success" /> Interview Easy / Workspace</span>
        <span className="hidden sm:inline">Senior Software Engineer</span>
      </div>
      <div className="grid flex-1 grid-cols-[3.25rem_1fr] sm:grid-cols-[4rem_1fr]">
        <div className="flex flex-col items-center gap-5 border-r border-glass-border py-5 text-faint-ink">
          <CalendarCheck className="size-4 text-brand" aria-hidden="true" />
          <Video className="size-4" aria-hidden="true" />
          <Code2 className="size-4" aria-hidden="true" />
          <FileBarChart className="size-4" aria-hidden="true" />
        </div>
        <div className="min-w-0 p-4 sm:p-6">
          {scene === "ai" && <>
            <p className="text-xs font-semibold text-ai-accent">QUESTION SET / BACKEND</p>
            <h3 className="mt-2 font-display text-xl font-semibold">Explore how they solve problems</h3>
            <div className="mt-5 border-l-2 border-brand bg-brand-soft p-4 text-sm font-medium text-ink">How would you design an API that handles sudden traffic spikes?</div>
            <div className="mt-4 grid gap-2 text-xs text-soft-ink sm:grid-cols-2"><span className="border-b border-glass-border py-2">01 · Clarify requirements</span><span className="border-b border-glass-border py-2">02 · Explain trade-offs</span><span className="border-b border-glass-border py-2">03 · Consider failure modes</span><span className="border-b border-glass-border py-2">04 · Discuss scaling</span></div>
          </>}
          {scene === "schedule" && <>
            <p className="text-xs font-semibold text-ai-accent">UPCOMING INTERVIEW</p>
            <h3 className="mt-2 font-display text-xl font-semibold">Backend Engineer · Technical round</h3>
            <div className="mt-5 grid gap-3 sm:grid-cols-2">
              <div className="rounded-md border border-glass-border bg-glass p-4"><p className="text-xs text-soft-ink">DATE & TIME</p><p className="mt-2 font-semibold">Tuesday, 10:30 AM</p></div>
              <div className="rounded-md border border-glass-border bg-glass p-4"><p className="text-xs text-soft-ink">INTERVIEW PANEL</p><p className="mt-2 font-semibold">2 interviewers</p></div>
            </div>
            <div className="mt-4 flex items-center gap-2 text-sm text-success"><CheckCircle2 className="size-4" aria-hidden="true" /> Interview plan ready to share</div>
          </>}
          {scene === "share" && <>
            <p className="text-xs font-semibold text-ai-accent">SCREEN SHARE / SYSTEM DESIGN</p>
            <div className="mt-4 border border-glass-border bg-ink p-4 font-mono text-xs leading-6 text-brand-foreground sm:p-5">
              <p className="text-faint-ink">architecture-notes.md</p>
              <p className="mt-3">Client → API Gateway → Services</p>
              <p>                         ↓</p>
              <p>                    Queue → Workers</p>
              <p>                         ↓</p>
              <p>                       Storage</p>
            </div>
            <p className="mt-3 flex items-center gap-2 text-sm text-soft-ink"><MonitorUp className="size-4 text-brand" aria-hidden="true" /> Shared view for the whole panel</p>
          </>}
          {scene === "record" && <>
            <div className="flex items-center justify-between gap-3"><p className="text-xs font-semibold text-ai-accent">INTERVIEW SESSION</p><span className="flex items-center gap-1.5 text-xs font-semibold text-ai-accent"><span className="size-2 rounded-full bg-ai-accent" /> REC</span></div>
            <div className="mt-4 grid grid-cols-2 gap-2 sm:gap-3">
              <div className="flex aspect-[1.35] items-end rounded-md bg-ink p-3 text-xs font-medium text-brand-foreground">Interviewer</div>
              <div className="flex aspect-[1.35] items-end rounded-md bg-soft-ink p-3 text-xs font-medium text-brand-foreground">Candidate</div>
            </div>
            <p className="mt-4 text-sm text-soft-ink">Discussion · Technical assessment · Panel review</p>
          </>}
          {scene === "tests" && <>
            <div className="flex items-center justify-between gap-2"><p className="text-xs font-semibold text-ai-accent">SOLUTION.JS</p><span className="text-xs font-semibold text-success">3 / 3 PASSED</span></div>
            <div className="mt-4 bg-ink p-4 font-mono text-xs leading-6 text-brand-foreground"><p>function twoSum(nums, target) {'{'}</p><p className="pl-4">const seen = new Map();</p><p className="pl-4">// find the matching pair</p><p>{'}'}</p></div>
            <div className="mt-3 space-y-2 text-xs text-soft-ink">{["Base case", "Duplicate values", "No matching pair"].map((test) => <p key={test} className="flex items-center gap-2"><CheckCircle2 className="size-4 text-success" aria-hidden="true" /> {test}</p>)}</div>
          </>}
          {scene === "feedback" && <>
            <p className="text-xs font-semibold text-ai-accent">CANDIDATE SCORECARD</p>
            <h3 className="mt-2 font-display text-xl font-semibold">Technical interview summary</h3>
            <div className="mt-5 space-y-4">{[["Problem solving", "Strong"], ["Code quality", "Strong"], ["Communication", "Good"]].map(([name, rating]) => <div key={name} className="flex items-center justify-between gap-3 border-b border-glass-border pb-3 text-sm"><span>{name}</span><span className="font-semibold text-brand">{rating}</span></div>)}</div>
            <p className="mt-4 text-xs text-soft-ink">Panel notes and test results in one review.</p>
          </>}
        </div>
      </div>
    </div>
  );
}

function Index() {
  const [activeVideo, setActiveVideo] = useState(0);
  const [activeFeature, setActiveFeature] = useState(0);
  const video = featuredVideos[activeVideo] ?? featuredVideos[0];
  const feature = features[activeFeature] ?? features[0];

  const showPreviousVideo = () => {
    setActiveVideo((current) => (current - 1 + featuredVideos.length) % featuredVideos.length);
  };

  const showNextVideo = () => {
    setActiveVideo((current) => (current + 1) % featuredVideos.length);
  };

  return (
    <div className="relative min-h-screen w-full overflow-hidden bg-cloud-gradient text-ink">
      <div className="pointer-events-none absolute -left-24 -top-24 size-[420px] rounded-full bg-brand-glow blur-[120px]" />
      <div className="pointer-events-none absolute -right-24 top-1/3 size-[420px] rounded-full bg-ai-accent-soft blur-[120px]" />
      <div className="pointer-events-none absolute bottom-0 left-1/3 size-[360px] rounded-full bg-cloud-cool blur-[120px]" />

      <SiteHeader />

      <main id="top" className="relative z-10">
        <section className="container-fluid grid items-center gap-12 pb-10 pt-14 lg:grid-cols-2">
          <div>
            <span className="inline-flex items-center gap-2 rounded-full border border-glass-border bg-glass px-3 py-1 text-xs font-semibold text-ai-accent shadow-sm backdrop-blur-md">
              <span className="size-2 rounded-full bg-success" /> Scheduling · Video · Code · Automated tests
            </span>
            <h1 className="mt-5 font-display text-5xl font-bold leading-[1.05] tracking-normal sm:text-6xl">
              Run better <span className="text-ai-gradient">technical interviews</span>
            </h1>
            <p className="mt-5 max-w-md text-lg text-soft-ink">
              Schedule candidates, meet on video, ask structured technical questions, write code together, and run automated unit tests in one place.
            </p>
            <div className="mt-7 flex flex-wrap gap-3">
              <Button asChild variant="hero" size="hero">
                <Link to="/demo">Schedule an interview <ArrowRight aria-hidden="true" /></Link>
              </Button>
              <Button asChild variant="glass" size="hero">
                <Link to="/practice"><Play aria-hidden="true" /> Try code practice</Link>
              </Button>
            </div>
            <dl className="mt-8 flex flex-wrap gap-8">
              <div>
                <dt className="font-display text-2xl font-bold">One link</dt>
                <dd className="text-xs text-faint-ink">For every participant</dd>
              </div>
              <div>
                <dt className="font-display text-2xl font-bold">Live</dt>
                <dd className="text-xs text-faint-ink">Video and code</dd>
              </div>
              <div>
                <dt className="font-display text-2xl font-bold">Auto</dt>
                <dd className="text-xs text-faint-ink">Unit test results</dd>
              </div>
            </dl>
          </div>

          <article className="overflow-hidden rounded-3xl border border-glass-border bg-glass p-3 shadow-ai-card backdrop-blur-xl sm:p-4">
            <div className="relative aspect-video overflow-hidden rounded-2xl bg-ink">
              <iframe
                key={video.embedUrl}
                className="size-full"
                src={video.embedUrl}
                title={video.title}
                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                allowFullScreen
              />
            </div>
            <div className="flex items-center justify-between gap-4 px-1 pb-1 pt-4">
              <div className="min-w-0">
                <div className="flex items-center gap-2 text-xs font-semibold text-brand">
                  <span>{video.topic}</span>
                  <span aria-hidden="true">•</span>
                  <span className="text-faint-ink">{video.duration}</span>
                </div>
                <h2 className="mt-1 truncate font-display text-lg font-semibold">{video.title}</h2>
              </div>
              <div className="flex shrink-0 gap-2">
                <Button
                  type="button"
                  variant="glass"
                  size="icon"
                  onClick={showPreviousVideo}
                  aria-label="Previous video"
                  title="Previous video"
                >
                  <ChevronLeft aria-hidden="true" />
                </Button>
                <Button
                  type="button"
                  variant="hero"
                  size="icon"
                  onClick={showNextVideo}
                  aria-label="Next video"
                  title="Next video"
                >
                  <ChevronRight aria-hidden="true" />
                </Button>
              </div>
            </div>
            <div className="flex justify-center gap-2 pb-1 pt-2" aria-label="Choose a video">
              {featuredVideos.map((item, index) => (
                <Button
                  key={item.title}
                  type="button"
                  variant="ghost"
                  size="icon"
                  onClick={() => setActiveVideo(index)}
                  className={`h-4 min-w-0 rounded-full p-0 transition-all ${index === activeVideo ? "w-7 bg-brand hover:bg-brand" : "w-4 bg-cloud-mid hover:bg-brand-soft"}`}
                  aria-label={`Show video ${index + 1}: ${item.title}`}
                  aria-current={index === activeVideo ? "true" : undefined}
                />
              ))}
            </div>
          </article>
        </section>

        <section aria-label="Interview platform highlights" className="container-fluid border-y border-glass-border py-10 sm:py-14">
          <div className="mb-7 flex flex-wrap items-end justify-between gap-5">
            <div>
              <p className="text-sm font-semibold text-ai-accent">The interview, end to end</p>
              <h2 className="mt-2 max-w-2xl font-display text-3xl font-bold sm:text-4xl">Everything around a better interview.</h2>
            </div>
            <div className="flex items-center gap-2">
              <span className="mr-2 font-mono text-sm text-soft-ink">{String(activeFeature + 1).padStart(2, "0")} / {String(features.length).padStart(2, "0")}</span>
              <Button type="button" variant="glass" size="icon" aria-label="Previous feature" title="Previous feature" onClick={() => setActiveFeature((current) => (current - 1 + features.length) % features.length)}><ChevronLeft aria-hidden="true" /></Button>
              <Button type="button" variant="hero" size="icon" aria-label="Next feature" title="Next feature" onClick={() => setActiveFeature((current) => (current + 1) % features.length)}><ChevronRight aria-hidden="true" /></Button>
            </div>
          </div>
          <div className="grid items-center gap-8 lg:grid-cols-[0.8fr_1.2fr] lg:gap-12" aria-live="polite">
            <div className="max-w-xl">
              <div className="flex size-12 items-center justify-center rounded-md bg-brand-soft text-brand"><feature.icon aria-hidden="true" /></div>
              <p className="mt-5 text-sm font-semibold text-ai-accent">{feature.label}</p>
              <h3 className="mt-2 font-display text-3xl font-bold leading-tight sm:text-4xl">{feature.title}</h3>
              <p className="mt-4 text-base leading-7 text-soft-ink">{feature.description}</p>
              <p className="mt-5 flex items-center gap-2 text-sm font-semibold text-ink"><CheckCircle2 className="size-4 text-success" aria-hidden="true" /> {feature.detail}</p>
              <Button asChild variant="glass" className="mt-7"><Link to="/demo">Explore the demo <ArrowRight aria-hidden="true" /></Link></Button>
            </div>
            <FeatureScene scene={feature.scene} />
          </div>
          <div className="mt-8 grid grid-cols-3 gap-2 sm:grid-cols-6" aria-label="Choose a feature">
            {features.map((item, index) => <Button key={item.label} type="button" variant="ghost" aria-label={`Show ${item.label}`} aria-current={index === activeFeature ? "true" : undefined} onClick={() => setActiveFeature(index)} className={`h-auto min-w-0 flex-col gap-2 rounded-md border px-2 py-3 text-center text-[11px] leading-tight whitespace-normal sm:text-xs ${index === activeFeature ? "border-brand bg-brand-soft text-brand" : "border-glass-border bg-glass text-soft-ink hover:bg-glass-strong hover:text-ink"}`}><item.icon className="size-4" aria-hidden="true" /><span>{item.label}</span></Button>)}
          </div>
        </section>

        <section id="questions" className="container-fluid py-8">
          <div className="mb-5 flex items-end justify-between gap-4">
            <h2 className="font-display text-2xl font-bold tracking-normal">Browse by topic</h2>
            <a className="text-sm font-semibold text-brand transition-colors hover:text-ai-accent" href="#topic-menu">
              View all
            </a>
          </div>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {topics.map((topic) => (
              <article
                key={topic.title}
                className="rounded-2xl border border-glass-border bg-glass p-5 backdrop-blur-xl transition-colors hover:bg-glass-strong"
              >
                <div className={`flex size-11 items-center justify-center rounded-xl ${topic.tone}`} aria-hidden="true">
                  <topic.Icon />
                </div>
                <h3 className="mt-3 font-display font-semibold">{topic.title}</h3>
                <p className="mt-1 text-sm text-soft-ink">{topic.description}</p>
                <div className="mt-4 text-xs font-semibold text-brand">{topic.count}</div>
              </article>
            ))}
          </div>
        </section>

        <section id="topic-menu" className="container-fluid py-8">
          <div className="mb-5 flex flex-col gap-2 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <p className="text-sm font-semibold text-ai-accent">Interview topic menu</p>
              <h2 className="font-display text-2xl font-bold tracking-normal">Choose a technology to practice</h2>
            </div>
            <p className="max-w-md text-sm text-soft-ink">
              Each option can contain questions, explanation text, video, code examples, and practice tasks.
            </p>
          </div>
          <div className="grid gap-4 lg:grid-cols-2">
            {menuGroups.map(({ title, description, Icon, tone, topics: groupTopics }) => (
              <article
                key={title}
                className="overflow-hidden rounded-2xl border border-glass-border bg-glass shadow-ai-soft backdrop-blur-xl"
              >
                <div className="flex items-start gap-3 border-b border-glass-border p-5 sm:p-6">
                  <div className={`inline-flex size-11 shrink-0 items-center justify-center rounded-xl ${tone}`}>
                    <Icon aria-hidden="true" />
                  </div>
                  <div className="min-w-0 flex-1">
                    <h3 className="font-display text-xl font-semibold tracking-normal">{title}</h3>
                    <p className="mt-1 text-sm text-soft-ink">{description}</p>
                  </div>
                  <span className="rounded-full bg-glass-strong px-2.5 py-1 text-xs font-semibold text-faint-ink">
                    {groupTopics.length}
                  </span>
                </div>
                <div className="grid sm:grid-cols-2">
                  {groupTopics.map((topic) => (
                    <Link
                      key={topic.slug}
                      to="/topics/$slug"
                      params={{ slug: topic.slug }}
                      preload="intent"
                      className="group flex min-h-12 items-center justify-between gap-3 border-b border-glass-border px-5 py-3 text-sm font-medium text-soft-ink transition-colors hover:bg-glass-strong hover:text-brand sm:[&:nth-child(odd)]:border-r"
                    >
                      <span>{topic.name}</span>
                      <ArrowUpRight className="size-4 shrink-0 text-faint-ink transition-transform group-hover:-translate-y-0.5 group-hover:translate-x-0.5 group-hover:text-brand" aria-hidden="true" />
                    </Link>
                  ))}
                </div>
              </article>
            ))}
          </div>
        </section>

        <section id="practice" className="container-fluid py-8">
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

        <section id="roadmap" className="container-fluid py-10">
          <div className="rounded-3xl border border-glass-border bg-glass p-6 shadow-ai-soft backdrop-blur-xl sm:p-8">
            <div className="flex items-center gap-3">
              <CalendarCheck className="text-ai-accent" aria-hidden="true" />
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

        <section className="container-fluid py-10">
          <div className="flex flex-col gap-4 border-b border-glass-border pb-7 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <p className="text-sm font-semibold text-ai-accent">Complete interview flow</p>
              <h2 className="mt-1 font-display text-3xl font-bold tracking-normal">From calendar invite to hiring decision</h2>
            </div>
            <Button asChild variant="hero" size="hero"><Link to="/demo">View product demo <ArrowRight aria-hidden="true" /></Link></Button>
          </div>
          <ol className="grid border-b border-glass-border md:grid-cols-5">
            {workflow.map((stage, index) => (
              <li key={stage.title} className="border-b border-glass-border py-6 md:border-b-0 md:border-r md:px-5 md:first:pl-0 md:last:border-r-0 md:last:pr-0">
                <div className="flex items-center justify-between">
                  <stage.icon className="text-brand" aria-hidden="true" />
                  <span className="text-xs font-bold text-faint-ink">0{index + 1}</span>
                </div>
                <h3 className="mt-4 font-display text-lg font-semibold">{stage.title}</h3>
                <p className="mt-2 text-sm leading-6 text-soft-ink">{stage.description}</p>
              </li>
            ))}
          </ol>
        </section>

      </main>

      <footer className="container-fluid relative z-10 flex flex-col items-center gap-3 py-10 text-center text-sm text-soft-ink sm:flex-row sm:justify-between sm:text-left">
        <span>InterviewEasy — practice smarter, not harder. © 2026</span>
        <span className="flex flex-wrap items-center justify-center gap-4">
          <Link to="/practice" className="font-medium transition-colors hover:text-brand">Practice</Link>
          <Link to="/pricing" className="font-medium transition-colors hover:text-brand">Pricing</Link>
          <Link to="/demo" className="font-medium transition-colors hover:text-brand">Demo</Link>
          <Link to="/careers" className="font-medium transition-colors hover:text-brand">Careers</Link>
          <Link to="/contact" className="font-medium transition-colors hover:text-brand">
            Contact
          </Link>
          <a
            href="mailto:careers@intervieweasy.in"
            className="font-medium text-brand transition-colors hover:text-ai-accent"
          >
            careers@intervieweasy.in
          </a>
        </span>
      </footer>
    </div>
  );
}
