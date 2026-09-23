import { createFileRoute, Link } from "@tanstack/react-router";
import { useState } from "react";
import {
  ArrowRight,
  BookOpen,
  BrainCircuit,
  Braces,
  Building2,
  Check,
  ChevronLeft,
  ChevronRight,
  Cloud,
  Code2,
  CreditCard,
  Database,
  Layers3,
  Play,
  Search,
  Sparkles,
  UserRound,
  Workflow,
} from "lucide-react";

const tracks = [
  {
    icon: Building2,
    title: "Corporate training",
    subtitle: "For teams and hiring managers",
    tone: "bg-brand-soft text-brand",
    price: "Custom quote per cohort",
    primary: true,
    cta: "Talk to us",
    ctaTo: "/contact",
    points: [
      "Skill assessment for the whole team before the programme starts",
      "Curriculum built from your stack: .NET, JavaScript, cloud, DevOps",
      "Live trainer sessions plus recorded walkthroughs",
      "Mock interview panels and a manager progress dashboard",
    ],
  },
  {
    icon: UserRound,
    title: "Individual",
    subtitle: "For developers preparing on their own",
    tone: "bg-ai-accent-soft text-ai-accent",
    price: "Free to start, upgrade any time",
    primary: false,
    cta: "Start practicing",
    ctaTo: "/",
    points: [
      "Full question library with explanations, videos, and examples",
      "Pick a topic path and practise at your own pace",
      "Daily practice sets and bookmarked answers",
      "Interview-day checklists for each technology",
    ],
  },
] as const;

const workflow = [
  { title: "Tell us your goal", description: "Choose corporate training or individual practice." },
  { title: "Skill check", description: "A short assessment shows your current level." },
  { title: "Your plan", description: "We map topics and a weekly schedule." },
  { title: "Learn and practise", description: "Explanations, videos, and hands-on examples." },
  { title: "Mock interviews", description: "Feedback rounds until you are interview ready." },
];

import { Button } from "@/components/ui/button";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "Interview Easy — Developer Interview Questions, Videos, and Practice" },
      {
        name: "description",
        content:
          "Prepare for .NET, JavaScript, database, Azure, AI, and integration interviews with searchable questions, explanations, videos, examples, and practice prompts.",
      },
      {
        property: "og:title",
        content: "Interview Easy — Developer Interview Questions, Videos, and Practice",
      },
      {
        property: "og:description",
        content:
          "Search .NET, JavaScript, database, Azure, AI, and integration interview topics, then learn each answer with text, video, and examples.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: Index,
});

const topics = [
  {
    icon: "#",
    title: ".NET Stack",
    description: "C#, ASP.NET, Core, APIs and desktop apps",
    count: "14 topic areas",
  },
  {
    icon: "JS",
    title: "JavaScript Stack",
    description: "JavaScript, Angular, React, TypeScript and Node.js",
    count: "9 topic areas",
  },
  {
    icon: "DB",
    title: "Databases",
    description: "SQL Server, MongoDB, RavenDB and PostgreSQL",
    count: "6 topic areas",
  },
  {
    icon: "AI",
    title: "AI & Integrations",
    description: "AI, payments, SAP, Salesforce and messaging",
    count: "12 topic areas",
  },
];

const menuGroups = [
  {
    title: ".NET Menu",
    description: "Microsoft developer interview questions and examples.",
    Icon: Layers3,
    tone: "bg-brand-soft text-brand",
    items: [
      "C#",
      "ASP.NET",
      "ASP.NET MVC",
      "ASP.NET Web API",
      "ASP.NET Core",
      "ASP.NET Core Web API",
      "ADO.NET",
      "Entity Framework",
      "LINQ",
      "NHibernate",
      "Fluent NHibernate",
      "Windows Forms",
      "WPF (ERP)",
      "Web Services, WCF, etc.",
    ],
  },
  {
    title: "JavaScript Menu",
    description: "Frontend and runtime topics for modern web roles.",
    Icon: Braces,
    tone: "bg-ai-accent-soft text-ai-accent",
    items: [
      "JavaScript",
      "jQuery",
      "AngularJS",
      "Angular",
      "React",
      "TypeScript",
      "Vue.js",
      "Node.js",
      "Python",
    ],
  },
  {
    title: "Database Menu",
    description: "Database concepts, queries, design, and performance.",
    Icon: Database,
    tone: "bg-success-soft text-success",
    items: ["SQL Server", "MongoDB", "RavenDB", "Cosmos DB", "SQLite", "PostgreSQL"],
  },
  {
    title: "Azure Menu",
    description: "Cloud, deployment, DevOps, and production services.",
    Icon: Cloud,
    tone: "bg-brand-soft text-brand",
    items: [
      "Azure",
      "Azure DevOps",
      "CI/CD Pipeline",
      "Kubernetes",
      "Docker",
      "App Services",
      "Logic Apps",
      "Service Bus",
      "Storage Accounts",
      "Application Insights",
      "Hangfire",
      "Azure Functions",
      "SignalR",
    ],
  },
  {
    title: "AI Menu",
    description: "Interview topics for current AI-powered development.",
    Icon: BrainCircuit,
    tone: "bg-ai-accent-soft text-ai-accent",
    items: ["AI Fundamentals", "Generative AI", "Prompt Engineering", "Chatbots", "Model APIs", "AI App Design"],
  },
  {
    title: "Integrations Menu",
    description: "Payment, enterprise, and communication integrations.",
    Icon: CreditCard,
    tone: "bg-success-soft text-success",
    items: [
      "Payment Gateways",
      "Paytm",
      "PayPal",
      "CCAvenue",
      "Razorpay",
      "SAP Function Modules",
      "Salesforce Functions",
      "Salesforce Data Extensions",
      "SMS Integration",
      "WhatsApp Integration",
      "Email Integration",
    ],
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

function Index() {
  const [activeVideo, setActiveVideo] = useState(0);
  const video = featuredVideos[activeVideo] ?? featuredVideos[0];

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

      <header className="container-fluid relative z-20 pt-5 sm:pt-6">
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
            <a href="#topic-menu" className="transition-colors hover:text-brand">
              Menu
            </a>
            <a href="#roadmap" className="transition-colors hover:text-brand">
              Roadmaps
            </a>
            <Link to="/careers" className="transition-colors hover:text-brand">
              Careers
            </Link>
            <Link to="/contact" className="transition-colors hover:text-brand">
              Contact
            </Link>
          </div>
          <div className="flex items-center gap-3">
            <a className="hidden text-sm font-medium text-soft-ink hover:text-brand sm:inline" href="#questions">
              Sign in
            </a>
            <Button asChild variant="hero" size="sm" className="rounded-xl px-4 py-2">
              <a href="#get-started">Get started</a>
            </Button>
          </div>
        </nav>
      </header>

      <main id="top" className="relative z-10">
        <section className="container-fluid grid items-center gap-12 pb-10 pt-14 lg:grid-cols-2">
          <div>
            <span className="inline-flex items-center gap-2 rounded-full border border-glass-border bg-glass px-3 py-1 text-xs font-semibold text-ai-accent shadow-sm backdrop-blur-md">
              <span className="size-2 rounded-full bg-ai-accent" /> 12,000+ questions across 50+ topics
            </span>
            <h1 className="mt-5 font-display text-5xl font-bold leading-[1.05] tracking-normal sm:text-6xl">
              Ace your next <span className="text-ai-gradient">developer interview</span>
            </h1>
            <p className="mt-5 max-w-md text-lg text-soft-ink">
              Every .NET, JavaScript, database, cloud, AI, and integration question paired with a clear explanation, video walkthrough, and runnable example.
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
                <dt className="font-display text-2xl font-bold">50+</dt>
                <dd className="text-xs text-faint-ink">Topics</dd>
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
                <div className="flex size-11 items-center justify-center rounded-xl bg-brand-soft font-display text-sm font-bold text-brand" aria-hidden="true">
                  {topic.icon}
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
            {menuGroups.map(({ title, description, Icon, tone, items }) => (
              <article
                key={title}
                className="rounded-3xl border border-glass-border bg-glass p-5 shadow-ai-soft backdrop-blur-xl sm:p-6"
              >
                <div className="flex items-start gap-3">
                  <div className={`inline-flex size-11 shrink-0 items-center justify-center rounded-xl ${tone}`}>
                    <Icon aria-hidden="true" />
                  </div>
                  <div>
                    <h3 className="font-display text-xl font-semibold tracking-normal">{title}</h3>
                    <p className="mt-1 text-sm text-soft-ink">{description}</p>
                  </div>
                </div>
                <div className="mt-5 flex flex-wrap gap-2">
                  {items.map((item) => (
                    <a
                      key={item}
                      href="#practice"
                      className="rounded-full border border-glass-border bg-glass-strong px-3 py-1.5 text-sm font-medium text-soft-ink transition-colors hover:text-brand"
                    >
                      {item}
                    </a>
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

        <section id="get-started" className="container-fluid py-10">
          <div className="text-center">
            <h2 className="font-display text-3xl font-bold tracking-normal">Get started</h2>
            <p className="mx-auto mt-2 max-w-xl text-sm text-soft-ink">
              Choose the track that fits you — a company upskilling a team, or one developer preparing for the next interview.
            </p>
          </div>
          <div className="mt-8 grid gap-6 lg:grid-cols-2">
            {tracks.map((track) => (
              <article
                key={track.title}
                className="flex flex-col rounded-3xl border border-glass-border bg-glass p-6 shadow-ai-card backdrop-blur-xl sm:p-8"
              >
                <div className="flex items-center gap-3">
                  <span className={`grid size-11 place-items-center rounded-2xl ${track.tone}`}>
                    <track.icon aria-hidden="true" />
                  </span>
                  <div>
                    <h3 className="font-display text-xl font-bold tracking-normal">{track.title}</h3>
                    <p className="text-sm text-soft-ink">{track.subtitle}</p>
                  </div>
                </div>
                <ul className="mt-5 grid gap-2 text-sm text-soft-ink">
                  {track.points.map((point) => (
                    <li key={point} className="flex items-start gap-2">
                      <Check className="mt-0.5 size-4 shrink-0 text-success" aria-hidden="true" />
                      {point}
                    </li>
                  ))}
                </ul>
                <p className="mt-5 text-sm font-semibold">{track.price}</p>
                <Button asChild variant={track.primary ? "hero" : "glass"} size="hero" className="mt-5">
                  <Link to={track.ctaTo}>
                    {track.cta} <ArrowRight aria-hidden="true" />
                  </Link>
                </Button>
              </article>
            ))}
          </div>

          <div className="mt-8 rounded-3xl border border-glass-border bg-glass p-6 shadow-ai-soft backdrop-blur-xl sm:p-8">
            <div className="flex items-center gap-3">
              <Workflow className="text-ai-accent" aria-hidden="true" />
              <h3 className="font-display text-2xl font-bold tracking-normal">How onboarding works</h3>
            </div>
            <ol className="mt-6 grid gap-4 md:grid-cols-5">
              {workflow.map((stage, index) => (
                <li
                  key={stage.title}
                  className="relative rounded-2xl border border-glass-border bg-glass-strong p-5"
                >
                  <span className="grid size-9 place-items-center rounded-xl bg-ai-gradient font-display font-bold text-brand-foreground">
                    {index + 1}
                  </span>
                  <h4 className="mt-3 font-display font-semibold">{stage.title}</h4>
                  <p className="mt-1 text-sm text-soft-ink">{stage.description}</p>
                </li>
              ))}
            </ol>
          </div>
        </section>

      </main>

      <footer className="container-fluid relative z-10 flex flex-col items-center gap-3 py-10 text-center text-sm text-soft-ink sm:flex-row sm:justify-between sm:text-left">
        <span>InterviewEasy — practice smarter, not harder. © 2026</span>
        <span className="flex flex-wrap items-center justify-center gap-4">
          <Link to="/careers" className="font-medium transition-colors hover:text-brand">
            Careers
          </Link>
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
