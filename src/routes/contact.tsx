import { createFileRoute, Link } from "@tanstack/react-router";
import { Mail, MapPin, MessageSquare, Phone } from "lucide-react";

import { Button } from "@/components/ui/button";
import { SiteHeader } from "@/components/site-header";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { toast } from "sonner";

export const Route = createFileRoute("/contact")({
  head: () => ({
    meta: [
      { title: "Contact Interview Easy — Talk to our team" },
      {
        name: "description",
        content:
          "Contact the Interview Easy team about corporate training, individual plans, careers, or partnership enquiries.",
      },
      {
        name: "keywords",
        content: "contact Interview Easy, corporate developer training, interview preparation support, developer careers",
      },
      { property: "og:title", content: "Contact Interview Easy — Talk to our team" },
      {
        property: "og:description",
        content: "Reach the Interview Easy team for corporate training, individual plans, and careers.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: ContactPage,
});

const channels = [
  {
    icon: Mail,
    label: "Careers",
    value: "careers@intervieweasy.in",
    href: "mailto:careers@intervieweasy.in",
  },
  {
    icon: MessageSquare,
    label: "Training and sales",
    value: "hello@intervieweasy.in",
    href: "mailto:hello@intervieweasy.in",
  },
  {
    icon: Phone,
    label: "Support hours",
    value: "Mon to Sat, 10:00 to 19:00 IST",
  },
  {
    icon: MapPin,
    label: "Working model",
    value: "Fully remote team",
  },
];

function ContactPage() {
  return (
    <div className="min-h-screen bg-cloud-gradient">
      <SiteHeader />

      <main className="container-fluid grid gap-8 py-12 lg:grid-cols-[1fr_1.1fr]">
        <section>
          <h1 className="font-display text-4xl font-bold leading-tight tracking-normal sm:text-5xl">
            Let us <span className="text-ai-gradient">talk</span>
          </h1>
          <p className="mt-4 max-w-md text-lg text-soft-ink">
            Questions about corporate training, individual preparation plans, or joining the team? Pick a channel below.
          </p>
          <div className="mt-8 grid gap-4 sm:grid-cols-2">
            {channels.map((channel) => (
              <article
                key={channel.label}
                className="rounded-2xl border border-glass-border bg-glass p-5 shadow-ai-soft backdrop-blur-xl"
              >
                <channel.icon className="text-brand" aria-hidden="true" />
                <h2 className="mt-3 font-display font-semibold">{channel.label}</h2>
                {channel.href ? (
                  <a className="mt-1 block text-sm font-medium text-brand hover:text-ai-accent" href={channel.href}>
                    {channel.value}
                  </a>
                ) : (
                  <p className="mt-1 text-sm text-soft-ink">{channel.value}</p>
                )}
              </article>
            ))}
          </div>
        </section>

        <section className="rounded-3xl border border-glass-border bg-glass p-6 shadow-ai-card backdrop-blur-xl sm:p-8">
          <h2 className="font-display text-2xl font-bold tracking-normal">Send a message</h2>
          <p className="mt-1 text-sm text-soft-ink">We reply within one business day.</p>
          <form
            className="mt-6 grid gap-4"
            onSubmit={(event) => {
              event.preventDefault();
              const form = event.currentTarget;
              toast.success("Thanks! Your message is on its way.");
              form.reset();
            }}
          >
            <div className="grid gap-4 sm:grid-cols-2">
              <label className="grid gap-2 text-sm font-medium">
                Name
                <Input required name="name" placeholder="Your full name" />
              </label>
              <label className="grid gap-2 text-sm font-medium">
                Email
                <Input required type="email" name="email" placeholder="you@company.com" />
              </label>
            </div>
            <label className="grid gap-2 text-sm font-medium">
              Topic
              <Input required name="topic" placeholder="Corporate training, individual plan, careers..." />
            </label>
            <label className="grid gap-2 text-sm font-medium">
              Message
              <Textarea required name="message" rows={5} placeholder="Tell us what you need" />
            </label>
            <Button type="submit" variant="hero" size="hero" className="justify-self-start">
              Send message
            </Button>
          </form>
        </section>
      </main>

      <footer className="container-fluid py-10 text-sm text-soft-ink">
        InterviewEasy — practice smarter, not harder. © 2026
      </footer>
    </div>
  );
}
