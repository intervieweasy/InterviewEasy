import { createFileRoute, Link } from "@tanstack/react-router";
import { Check, Minus } from "lucide-react";

import { SiteHeader } from "@/components/site-header";
import { Button } from "@/components/ui/button";

export const Route = createFileRoute("/pricing")({
  head: () => ({
    meta: [
      { title: "Interview Platform Pricing — Interview Easy" },
      { name: "description", content: "Simple pricing for scheduling, running, and reviewing technical interviews with video, code, and automated tests." },
      { name: "keywords", content: "technical interview platform pricing, interview scheduling software, coding interview tool pricing" },
      { property: "og:title", content: "Interview Platform Pricing — Interview Easy" },
      { property: "og:description", content: "Choose a plan for structured video and coding interviews with automated test cases." },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: PricingPage,
});

const plans = [
  {
    name: "Starter",
    price: "$0",
    note: "For trying the interview workspace",
    action: "Try practice",
    to: "/practice" as const,
    featured: false,
    features: ["3 interviews per month", "Question library", "Code editor", "Automated test runs"],
  },
  {
    name: "Hiring Team",
    price: "$49",
    note: "Per interviewer, billed monthly",
    action: "Book a demo",
    to: "/demo" as const,
    featured: true,
    features: ["Unlimited interviews", "Live video rooms", "Interview scheduling", "Shared scorecards", "Candidate reports"],
  },
  {
    name: "Scale",
    price: "Custom",
    note: "For high-volume hiring teams",
    action: "Contact us",
    to: "/contact" as const,
    featured: false,
    features: ["Everything in Hiring Team", "Custom question banks", "SSO and team controls", "Priority support", "Usage reporting"],
  },
] as const;

function PricingPage() {
  return (
    <div className="min-h-screen bg-cloud-gradient text-ink">
      <SiteHeader />
      <main className="container-fluid py-12 sm:py-16">
        <section className="mx-auto max-w-3xl text-center">
          <p className="text-sm font-semibold text-ai-accent">Straightforward pricing</p>
          <h1 className="mt-2 font-display text-4xl font-bold leading-tight tracking-normal sm:text-5xl">Run better technical interviews</h1>
          <p className="mx-auto mt-4 max-w-2xl text-lg text-soft-ink">Start with practice, then add scheduling, live video, team scorecards, and candidate reports when you are ready.</p>
        </section>

        <section className="mt-10 grid gap-5 lg:grid-cols-3">
          {plans.map((plan) => (
            <article key={plan.name} className={`flex flex-col rounded-2xl border p-6 backdrop-blur-xl sm:p-8 ${plan.featured ? "border-brand bg-glass-strong shadow-ai-card" : "border-glass-border bg-glass shadow-ai-soft"}`}>
              {plan.featured && <span className="mb-5 self-start rounded-full bg-brand-soft px-3 py-1 text-xs font-semibold text-brand">Most popular</span>}
              <h2 className="font-display text-xl font-bold">{plan.name}</h2>
              <p className="mt-4 font-display text-4xl font-bold tracking-normal">{plan.price}</p>
              <p className="mt-2 min-h-10 text-sm text-soft-ink">{plan.note}</p>
              <ul className="my-7 grid gap-3 text-sm">
                {plan.features.map((feature) => <li key={feature} className="flex items-start gap-2"><Check className="mt-0.5 size-4 shrink-0 text-success" aria-hidden="true" /> {feature}</li>)}
              </ul>
              <Button asChild variant={plan.featured ? "hero" : "glass"} size="hero" className="mt-auto w-full">
                <Link to={plan.to}>{plan.action}</Link>
              </Button>
            </article>
          ))}
        </section>

        <section className="mt-10 border-y border-glass-border py-8 text-center">
          <p className="flex items-center justify-center gap-2 text-sm text-soft-ink"><Minus className="size-4" aria-hidden="true" /> Need a tailored interview volume or migration plan?</p>
          <Button asChild variant="link" className="mt-2"><Link to="/contact">Talk to the Interview Easy team</Link></Button>
        </section>
      </main>
    </div>
  );
}