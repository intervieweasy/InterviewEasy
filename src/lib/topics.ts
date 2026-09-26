import {
  BrainCircuit,
  Braces,
  Building2,
  Cloud,
  Coffee,
  CreditCard,
  Database,
  Globe,
  Layers3,
  Smartphone,
  Terminal,
} from "lucide-react";

export type Topic = {
  name: string;
  slug: string;
  category: string;
  description: string;
  keywords: string;
};

const describe = (name: string, category: string) =>
  `${name} interview questions with clear explanations, practical examples, video lessons, and guided practice for ${category.toLowerCase()} roles.`;

const makeTopics = (category: string, names: readonly string[]): Topic[] =>
  names.map((name) => ({
    name,
    slug: name
      .toLowerCase()
      .replace(/#/g, "-sharp")
      .replace(/&/g, "-and-")
      .replace(/\+/g, "plus")
      .replace(/[^a-z0-9]+/g, "-")
      .replace(/^-|-$/g, ""),
    category,
    description: describe(name, category),
    keywords: `${name} interview questions, ${name} interview answers, ${name} examples, ${category} interview preparation`,
  }));

export const menuGroups = [
  {
    title: ".NET",
    description: "Microsoft development, APIs, data access, and desktop applications.",
    Icon: Layers3,
    tone: "bg-brand-soft text-brand",
    topics: makeTopics(".NET", [
      "C#", "ASP.NET", "ASP.NET MVC", "ASP.NET Web API", "ASP.NET Core", "ASP.NET Core Web API",
      "ADO.NET", "Entity Framework", "LINQ", "NHibernate", "Fluent NHibernate", "Windows Forms",
      "WPF (ERP)", "Web Services, WCF, etc.",
    ]),
  },
  {
    title: "Java",
    description: "Core Java, Spring ecosystem, persistence, and enterprise services.",
    Icon: Coffee,
    tone: "bg-ai-accent-soft text-ai-accent",
    topics: makeTopics("Java", [
      "Core Java", "Advanced Java", "Spring", "Spring Boot", "Spring MVC", "Hibernate",
      "JSP & Servlets", "Java Microservices",
    ]),
  },
  {
    title: "JavaScript & Web",
    description: "Frontend frameworks, typed web development, and server-side runtimes.",
    Icon: Braces,
    tone: "bg-brand-soft text-brand",
    topics: makeTopics("JavaScript & Web", [
      "JavaScript", "jQuery", "AngularJS", "Angular", "React", "TypeScript", "Vue.js", "Node.js",
    ]),
  },
  {
    title: "Python",
    description: "Python development, web frameworks, and data tooling.",
    Icon: Terminal,
    tone: "bg-success-soft text-success",
    topics: makeTopics("Python", [
      "Python", "Django", "Flask", "FastAPI", "Pandas & NumPy", "Python Scripting & Automation",
    ]),
  },
  {
    title: "PHP",
    description: "PHP frameworks, CMS development, and web applications.",
    Icon: Globe,
    tone: "bg-ai-accent-soft text-ai-accent",
    topics: makeTopics("PHP", [
      "PHP", "Laravel", "CodeIgniter", "Symfony", "WordPress",
    ]),
  },
  {
    title: "Mobile",
    description: "Android, iOS, and cross-platform mobile development.",
    Icon: Smartphone,
    tone: "bg-brand-soft text-brand",
    topics: makeTopics("Mobile", [
      "Android", "Kotlin", "Jetpack Compose", "Flutter", "React Native", "iOS & Swift",
    ]),
  },
  {
    title: "Databases",
    description: "Database concepts, queries, data modelling, and performance.",
    Icon: Database,
    tone: "bg-success-soft text-success",
    topics: makeTopics("Databases", ["SQL Server", "MongoDB", "RavenDB", "Cosmos DB", "SQLite", "PostgreSQL"]),
  },
  {
    title: "Azure & DevOps",
    description: "Cloud services, deployment pipelines, containers, and monitoring.",
    Icon: Cloud,
    tone: "bg-brand-soft text-brand",
    topics: makeTopics("Azure & DevOps", [
      "Azure", "Azure DevOps", "CI/CD Pipeline", "Kubernetes", "Docker", "App Services", "Logic Apps",
      "Service Bus", "Storage Accounts", "Application Insights", "Hangfire", "Azure Functions", "SignalR",
    ]),
  },
  {
    title: "SAP & Enterprise",
    description: "SAP modules, ABAP development, and enterprise system integration.",
    Icon: Building2,
    tone: "bg-ai-accent-soft text-ai-accent",
    topics: makeTopics("SAP & Enterprise", [
      "SAP ABAP", "SAP Function Modules", "SAP MM", "SAP SD", "SAP FICO", "SAP HANA",
      "SAP PI/PO", "SAP BTP",
    ]),
  },
  {
    title: "Artificial Intelligence",
    description: "Essential concepts for building and discussing AI-powered products.",
    Icon: BrainCircuit,
    tone: "bg-success-soft text-success",
    topics: makeTopics("Artificial Intelligence", [
      "AI Fundamentals", "Generative AI", "Prompt Engineering", "Chatbots", "Model APIs", "AI App Design",
    ]),
  },
  {
    title: "Integrations",
    description: "Payments, enterprise platforms, and customer communication channels.",
    Icon: CreditCard,
    tone: "bg-brand-soft text-brand",
    topics: makeTopics("Integrations", [
      "Payment Gateways", "Paytm", "PayPal", "CCAvenue", "Razorpay",
      "Salesforce Functions", "Salesforce Data Extensions", "SMS Integration", "WhatsApp Integration", "Email Integration",
    ]),
  },
] as const;

export const allTopics = menuGroups.flatMap((group) => group.topics);

export function getTopic(slug: string) {
  return allTopics.find((topic) => topic.slug === slug);
}
