import { createFileRoute, Link } from "@tanstack/react-router";
import { Clock, Globe2, Mail } from "lucide-react";

import { Button } from "@/components/ui/button";
import { SiteHeader } from "@/components/site-header";

export const Route = createFileRoute("/careers")({
  head: () => ({
    meta: [
      { title: "Careers at Interview Easy — Remote developer roles" },
      {
        name: "description",
        content:
          "Ten remote openings at Interview Easy paying from $10 per hour: .NET, JavaScript, Angular, React, AWS, Azure, Docker, Kubernetes and more.",
      },
      {
        name: "keywords",
        content: "Interview Easy careers, remote developer jobs, .NET jobs, React jobs, Angular jobs, cloud jobs",
      },
      { property: "og:title", content: "Careers at Interview Easy — Remote developer roles" },
      {
        property: "og:description",
        content: "Remote roles from $10 per hour across .NET, JavaScript, cloud and DevOps. Apply at careers@intervieweasy.in.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: CareersPage,
});

type Job = {
  title: string;
  stack: string[];
  rate: string;
  type: string;
  experience: string;
  summary: string;
};

const jobs: Job[] = [
  {
    title: "Senior .NET Backend Engineer",
    stack: ["C#", "ASP.NET Core", "Entity Framework"],
    rate: "$22 - $35 / hr",
    type: "Remote · Full time",
    experience: "5+ years",
    summary: "Build and scale the question library APIs and background workers.",
  },
  {
    title: ".NET Web API Developer",
    stack: ["ASP.NET Core Web API", "LINQ", "SQL Server"],
    rate: "$16 - $26 / hr",
    type: "Remote · Contract",
    experience: "3+ years",
    summary: "Design clean REST endpoints and write the tests that keep them honest.",
  },
  {
    title: "Angular Frontend Engineer",
    stack: ["Angular", "TypeScript", "RxJS"],
    rate: "$15 - $28 / hr",
    type: "Remote · Full time",
    experience: "3+ years",
    summary: "Own the learner dashboard and practice player experience.",
  },
  {
    title: "React Frontend Engineer",
    stack: ["React", "TypeScript", "Tailwind CSS"],
    rate: "$15 - $30 / hr",
    type: "Remote · Full time",
    experience: "3+ years",
    summary: "Ship the public site, topic menu and video carousel work.",
  },
  {
    title: "Full Stack JavaScript Developer",
    stack: ["Node.js", "Vue.js", "MongoDB"],
    rate: "$14 - $25 / hr",
    type: "Remote · Contract",
    experience: "2+ years",
    summary: "Move across API and UI work on integration-heavy features.",
  },
  {
    title: "AWS Cloud Engineer",
    stack: ["AWS", "Terraform", "Serverless"],
    rate: "$20 - $34 / hr",
    type: "Remote · Full time",
    experience: "4+ years",
    summary: "Run our media delivery, storage and cost optimisation on AWS.",
  },
  {
    title: "Azure Cloud Engineer",
    stack: ["Azure", "App Services", "Azure Functions"],
    rate: "$20 - $34 / hr",
    type: "Remote · Full time",
    experience: "4+ years",
    summary: "Own App Services, Functions, Service Bus and Application Insights.",
  },
  {
    title: "DevOps Engineer (Docker & Kubernetes)",
    stack: ["Docker", "Kubernetes", "Azure DevOps"],
    rate: "$18 - $32 / hr",
    type: "Remote · Full time",
    experience: "4+ years",
    summary: "Containerise services and keep CI/CD pipelines fast and boring.",
  },
  {
    title: "Database Engineer",
    stack: ["SQL Server", "PostgreSQL", "Cosmos DB"],
    rate: "$16 - $28 / hr",
    type: "Remote · Contract",
    experience: "3+ years",
    summary: "Model the question bank and tune the queries behind search.",
  },
  {
    title: "Technical Content & Video Trainer",
    stack: ["Interview prep", "Video scripting", "Live training"],
    rate: "$10 - $20 / hr",
    type: "Remote · Part time",
    experience: "2+ years",
    summary: "Write explanations and record walkthrough videos for each topic.",
  },
];

function CareersPage() {
  return (
    <div className="min-h-screen bg-cloud-gradient">
      <SiteHeader />

      <main className="container-fluid py-12">
        <section className="max-w-2xl">
          <span className="inline-flex items-center gap-2 rounded-full border border-glass-border bg-glass px-3 py-1 text-xs font-semibold text-ai-accent shadow-sm backdrop-blur-md">
            <Globe2 className="size-3.5" aria-hidden="true" /> 10 open roles · 100% remote
          </span>
          <h1 className="mt-5 font-display text-4xl font-bold leading-tight tracking-normal sm:text-5xl">
            Build <span className="text-ai-gradient">Interview Easy</span> from anywhere
          </h1>
          <p className="mt-4 text-lg text-soft-ink">
            Every role is remote, hourly and paid from $10 per hour upward based on skill and experience. Send your CV to{" "}
            <a className="font-semibold text-brand hover:text-ai-accent" href="mailto:careers@intervieweasy.in">
              careers@intervieweasy.in
            </a>
            .
          </p>
        </section>

        <section className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">
          {jobs.map((job) => (
            <article
              key={job.title}
              className="flex flex-col rounded-3xl border border-glass-border bg-glass p-6 shadow-ai-soft backdrop-blur-xl"
            >
              <h2 className="font-display text-xl font-bold tracking-normal">{job.title}</h2>
              <p className="mt-2 text-sm text-soft-ink">{job.summary}</p>
              <div className="mt-4 flex flex-wrap gap-2">
                {job.stack.map((item) => (
                  <span
                    key={item}
                    className="rounded-full border border-glass-border bg-glass-strong px-3 py-1 text-xs font-medium text-soft-ink"
                  >
                    {item}
                  </span>
                ))}
              </div>
              <dl className="mt-5 grid gap-1 text-sm">
                <div className="flex items-center justify-between">
                  <dt className="text-faint-ink">Rate</dt>
                  <dd className="font-semibold text-brand">{job.rate}</dd>
                </div>
                <div className="flex items-center justify-between">
                  <dt className="text-faint-ink">Engagement</dt>
                  <dd className="font-medium">{job.type}</dd>
                </div>
                <div className="flex items-center justify-between">
                  <dt className="text-faint-ink">Experience</dt>
                  <dd className="font-medium">{job.experience}</dd>
                </div>
              </dl>
              <Button asChild variant="glass" className="mt-6 w-full">
                <a href={`mailto:careers@intervieweasy.in?subject=${encodeURIComponent(`Application: ${job.title}`)}`}>
                  <Mail aria-hidden="true" /> Apply for this role
                </a>
              </Button>
            </article>
          ))}
        </section>

        <section className="mt-10 rounded-3xl border border-glass-border bg-glass p-6 shadow-ai-soft backdrop-blur-xl sm:p-8">
          <div className="flex flex-col items-start gap-4 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <h2 className="font-display text-2xl font-bold tracking-normal">Hiring process</h2>
              <p className="mt-1 flex items-center gap-2 text-sm text-soft-ink">
                <Clock className="size-4" aria-hidden="true" /> CV review, 30 minute intro call, paid task, offer — usually within two weeks.
              </p>
            </div>
            <Button asChild variant="hero" size="hero">
              <a href="mailto:careers@intervieweasy.in">careers@intervieweasy.in</a>
            </Button>
          </div>
        </section>
      </main>

      <footer className="container-fluid py-10 text-sm text-soft-ink">
        InterviewEasy — practice smarter, not harder. © 2026
      </footer>
    </div>
  );
}
