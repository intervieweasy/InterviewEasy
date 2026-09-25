import { Link } from "@tanstack/react-router";
import { CalendarPlus, Menu, X } from "lucide-react";
import { useState } from "react";

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
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <header className="container-fluid relative z-20 pt-5 sm:pt-6">
      <nav className="rounded-2xl border border-glass-border bg-glass px-4 py-3 shadow-ai-soft backdrop-blur-xl sm:px-5">
        <div className="flex items-center justify-between gap-4">
          <Brand />
          <div className="hidden items-center gap-5 text-sm font-medium text-soft-ink lg:flex">
            <Link to="/practice" activeProps={{ className: "text-brand" }} className="transition-colors hover:text-brand">Practice</Link>
            <Link to="/" hash="topic-menu" className="transition-colors hover:text-brand">Questions</Link>
            <Link to="/pricing" activeProps={{ className: "text-brand" }} className="transition-colors hover:text-brand">Pricing</Link>
            <Link to="/demo" activeProps={{ className: "text-brand" }} className="transition-colors hover:text-brand">Demo</Link>
            <Link to="/careers" activeProps={{ className: "text-brand" }} className="transition-colors hover:text-brand">Careers</Link>
            <Link to="/contact" activeProps={{ className: "text-brand" }} className="transition-colors hover:text-brand">Contact</Link>
          </div>
          <div className="flex items-center gap-2">
            <Button asChild variant="hero" size="sm" className="hidden rounded-lg px-4 py-2 sm:inline-flex">
              <Link to="/demo"><CalendarPlus aria-hidden="true" /> Schedule interview</Link>
            </Button>
            <Button type="button" variant="glass" size="icon" className="lg:hidden" aria-label={mobileOpen ? "Close menu" : "Open menu"} aria-expanded={mobileOpen} onClick={() => setMobileOpen((open) => !open)}>
              {mobileOpen ? <X aria-hidden="true" /> : <Menu aria-hidden="true" />}
            </Button>
          </div>
        </div>
        {mobileOpen && (
          <div className="mt-3 grid gap-1 border-t border-glass-border pt-3 text-sm font-medium text-soft-ink lg:hidden">
            <Link to="/practice" className="rounded-lg px-3 py-2.5 hover:bg-glass-strong hover:text-brand">Practice</Link>
            <Link to="/" hash="topic-menu" className="rounded-lg px-3 py-2.5 hover:bg-glass-strong hover:text-brand">Questions</Link>
            <Link to="/pricing" className="rounded-lg px-3 py-2.5 hover:bg-glass-strong hover:text-brand">Pricing</Link>
            <Link to="/demo" className="rounded-lg px-3 py-2.5 hover:bg-glass-strong hover:text-brand">Demo</Link>
            <Link to="/careers" className="rounded-lg px-3 py-2.5 hover:bg-glass-strong hover:text-brand">Careers</Link>
            <Link to="/contact" className="rounded-lg px-3 py-2.5 hover:bg-glass-strong hover:text-brand">Contact</Link>
          </div>
        )}
      </nav>
    </header>
  );
}
