import { createFileRoute, Link, notFound } from "@tanstack/react-router";
import { ArrowLeft, ArrowRight, BookOpen, CheckCircle2, Code2, Play, Search } from "lucide-react";

import { SiteHeader } from "@/components/site-header";
import { Button } from "@/components/ui/button";
import { allTopics, getTopic } from "@/lib/topics";

export const Route = createFileRoute("/topics/$slug")({
  loader: ({ params }) => {
    const topic = getTopic(params.slug);
    if (!topic) throw notFound();
    return { topic };
  },
  head: ({ loaderData }) => {
    const topic = loaderData?.topic;
    if (!topic) {
      return { meta: [{ title: "Topic not found — Interview Easy" }, { name: "robots", content: "noindex" }] };
    }
    const title = `${topic.name} Interview Questions, Answers & Examples — Interview Easy`;
    return {
      meta: [
        { title },
        { name: "description", content: topic.description },
        { name: "keywords", content: topic.keywords },
        { property: "og:title", content: title },
        { property: "og:description", content: topic.description },
        { property: "og:type", content: "website" },
        { name: "twitter:card", content: "summary_large_image" },
      ],
    };
  },
  notFoundComponent: TopicNotFound,
  component: TopicPage,
});

const learningAreas = [
  { Icon: BookOpen, title: "Interview questions", text: "Review commonly asked questions from fundamentals through advanced scenarios." },
  { Icon: CheckCircle2, title: "Clear explanations", text: "Understand why each answer works and how to explain it confidently." },
  { Icon: Play, title: "Video lessons", text: "Watch focused walkthroughs that turn difficult concepts into practical steps." },
  { Icon: Code2, title: "Code examples", text: "Study realistic examples and patterns you can discuss in a technical interview." },
] as const;

function TopicPage() {
  const { topic } = Route.useLoaderData();
  const related = allTopics.filter((item) => item.category === topic.category && item.slug !== topic.slug).slice(0, 6);

  return (
    <div className="min-h-screen bg-cloud-gradient text-ink">
      <SiteHeader />
      <main className="container-fluid py-10 sm:py-14">
        <Link to="/" hash="topic-menu" className="inline-flex items-center gap-2 text-sm font-semibold text-brand hover:text-ai-accent">
          <ArrowLeft className="size-4" aria-hidden="true" /> All technologies
        </Link>

        <section className="mt-6 border-b border-glass-border pb-10">
          <p className="text-sm font-semibold text-ai-accent">{topic.category}</p>
          <h1 className="mt-3 max-w-4xl font-display text-4xl font-bold leading-tight tracking-normal sm:text-6xl">
            {topic.name} <span className="text-ai-gradient">interview preparation</span>
          </h1>
          <p className="mt-5 max-w-2xl text-lg text-soft-ink">{topic.description}</p>
          <div className="mt-7 flex flex-wrap gap-3">
            <Button asChild variant="hero" size="hero">
              <Link to="/" hash="questions"><Search aria-hidden="true" /> Browse questions</Link>
            </Button>
            <Button asChild variant="glass" size="hero">
              <Link to="/" hash="practice"><Play aria-hidden="true" /> Watch lessons</Link>
            </Button>
          </div>
        </section>

        <section className="py-10">
          <div className="mb-5">
            <p className="text-sm font-semibold text-ai-accent">Everything in one place</p>
            <h2 className="mt-1 font-display text-2xl font-bold tracking-normal">Learn, explain, and practise {topic.name}</h2>
          </div>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {learningAreas.map(({ Icon, title, text }) => (
              <article key={title} className="rounded-2xl border border-glass-border bg-glass p-5 shadow-ai-soft backdrop-blur-xl">
                <span className="grid size-11 place-items-center rounded-xl bg-brand-soft text-brand"><Icon aria-hidden="true" /></span>
                <h3 className="mt-4 font-display font-semibold">{title}</h3>
                <p className="mt-2 text-sm text-soft-ink">{text}</p>
              </article>
            ))}
          </div>
        </section>

        {related.length > 0 && (
          <section className="border-t border-glass-border py-10">
            <h2 className="font-display text-2xl font-bold tracking-normal">More in {topic.category}</h2>
            <div className="mt-5 grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
              {related.map((item) => (
                <Link key={item.slug} to="/topics/$slug" params={{ slug: item.slug }} className="group flex items-center justify-between rounded-xl border border-glass-border bg-glass px-4 py-3 font-medium shadow-ai-soft transition-colors hover:bg-glass-strong hover:text-brand">
                  {item.name}<ArrowRight className="size-4 transition-transform group-hover:translate-x-1" aria-hidden="true" />
                </Link>
              ))}
            </div>
          </section>
        )}
      </main>
    </div>
  );
}

function TopicNotFound() {
  return (
    <div className="min-h-screen bg-cloud-gradient text-ink">
      <SiteHeader />
      <main className="container-fluid py-24 text-center">
        <h1 className="font-display text-4xl font-bold tracking-normal">Topic not found</h1>
        <p className="mt-3 text-soft-ink">Choose another technology from the Interview Easy menu.</p>
        <Button asChild variant="hero" className="mt-6"><Link to="/" hash="topic-menu">Browse technologies</Link></Button>
      </main>
    </div>
  );
}
