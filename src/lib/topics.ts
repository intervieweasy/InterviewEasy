import { BrainCircuit, Braces, Cloud, CreditCard, Database, Layers3 } from "lucide-react";

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
    title: "JavaScript & Web",
    description: "Frontend frameworks, typed web development, runtimes, and Python.",
    Icon: Braces,
    tone: "bg-ai-accent-soft text-ai-accent",
    topics: makeTopics("JavaScript & Web", [
      "JavaScript", "jQuery", "AngularJS", "Angular", "React", "TypeScript", "Vue.js", "Node.js", "Python",
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
    title: "Artificial Intelligence",
    description: "Essential concepts for building and discussing AI-powered products.",
    Icon: BrainCircuit,
    tone: "bg-ai-accent-soft text-ai-accent",
    topics: makeTopics("Artificial Intelligence", [
      "AI Fundamentals", "Generative AI", "Prompt Engineering", "Chatbots", "Model APIs", "AI App Design",
    ]),
  },
  {
    title: "Integrations",
    description: "Payments, enterprise platforms, and customer communication channels.",
    Icon: CreditCard,
    tone: "bg-success-soft text-success",
    topics: makeTopics("Integrations", [
      "Payment Gateways", "Paytm", "PayPal", "CCAvenue", "Razorpay", "SAP Function Modules",
      "Salesforce Functions", "Salesforce Data Extensions", "SMS Integration", "WhatsApp Integration", "Email Integration",
    ]),
  },
] as const;

export const allTopics = menuGroups.flatMap((group) => group.topics);

export function getTopic(slug: string) {
  return allTopics.find((topic) => topic.slug === slug);
}
