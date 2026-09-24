import { Link } from "@tanstack/react-router";
import { Menu } from "lucide-react";

import logo from "@/assets/interview-easy-logo.png";
import { Button } from "@/components/ui/button";

export function Brand() {
  return (
    <Link to="/" className="flex min-w-0 items-center gap-2.5" aria-label="Interview Easy home">
      <img src={logo} alt="" width={1024} height={1024} className="size-10 shrink-0 object-contain" />
      <span className="truncate font-display text-lg font-bold tracking-normal">
        Interview<span className="text-brand">Easy</span>
      </span>
    </Link>
  );
}

export function SiteHeader() {
  return (
    <header className="container-fluid relative z-20 pt-5 sm:pt-6">
      <nav className="flex items-center justify-between gap-4 rounded-2xl border border-glass-border bg-glass px-4 py-3 shadow-ai-soft backdrop-blur-xl sm:px-5">
        <Brand />
        <div className="hidden items-center gap-6 text-sm font-medium text-soft-ink lg:flex">
          <Link to="/" hash="questions" className="transition-colors hover:text-brand">Questions</Link>
          <Link to="/" hash="topic-menu" className="transition-colors hover:text-brand">Topics</Link>
          <Link to="/careers" className="transition-colors hover:text-brand">Careers</Link>
          <Link to="/contact" className="transition-colors hover:text-brand">Contact</Link>
        </div>
        <div className="flex items-center gap-2">
          <Button asChild variant="hero" size="sm" className="hidden rounded-xl px-4 py-2 sm:inline-flex">
            <Link to="/" hash="get-started">Get started</Link>
          </Button>
          <Button asChild variant="glass" size="icon" className="lg:hidden" title="Browse topics">
            <Link to="/" hash="topic-menu" aria-label="Browse topics"><Menu aria-hidden="true" /></Link>
          </Button>
        </div>
      </nav>
    </header>
  );
}
