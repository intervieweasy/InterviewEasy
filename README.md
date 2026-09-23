# InterviewHub - Enterprise Interview Platform (Updated)

## 📋 Project Overview

**InterviewHub** is an enterprise-grade interview management platform that facilitates end-to-end technical interview processes including scheduling, live audio/video interviews, collaborative coding sessions, and structured feedback collection. The platform serves three distinct user personas through specialized interfaces while maintaining a unified backend architecture.

### Key Capabilities
- 🎥 Real-time audio/video interviews with recording
- 💻 Collaborative code editor with multi-language support
- 📝 **Client-provided question sets (Basic/Medium/Advanced) organized by course & language**
- 📅 Intelligent scheduling and calendar management
- 🎯 **Automated test case execution for coding questions**
- 📊 Dynamic, client-specific feedback forms
- 🔒 Enterprise-grade security and compliance

---

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT LAYER                                    │
├─────────────────┬─────────────────┬─────────────────┬───────────────────────┤
│  Client Portal  │  Interviewer    │  Admin Console  │   Candidate Portal    │
│    (React)      │   (React)       │   (Angular)     │      (React)          │
└────────┬────────┴────────┬────────┴────────┬────────┴───────────┬───────────┘
         │                 │                 │                    │
         └─────────────────┴────────┬────────┴────────────────────┘
                                    │
                          ┌─────────▼─────────┐
                          │   API Gateway     │
                          │  (Azure APIM /    │
                          │   AWS API GW)     │
                          └─────────┬─────────┘
                                    │
         ┌──────────────────────────┼──────────────────────────┐
         │                          │                          │
┌────────▼────────┐      ┌──────────▼──────────┐    ┌─────────▼─────────┐
│  Core API       │      │  Real-Time Service  │    │  Media Service    │
│  (ASP.NET Core) │      │  (SignalR/Azure)    │    │  (WebRTC/LiveKit) │
└────────┬────────┘      └──────────┬──────────┘    └─────────┬─────────┘
         │                          │                          │
         └──────────────────────────┼──────────────────────────┘
                                    │
    ┌───────────────────────────────┼───────────────────────────────┐
    │                               │                               │
┌───▼───────────┐          ┌────────▼────────┐           ┌─────────▼─────────┐
│  PostgreSQL   │          │     Redis       │           │   S3/Blob Storage │
│  (Primary DB) │          │  (Cache/PubSub) │           │  (Media/Recording)│
└───────────────┘          └─────────────────┘           └───────────────────┘
                                    │
                          ┌─────────▼─────────┐
                          │  Code Sandbox     │
                          │  (Docker/K8s)     │
                          └───────────────────┘
```

---

## 🧩 Question Taxonomy (Core of InterviewHub)

Every interview question is defined across **four orthogonal dimensions**:

```
┌─────────────────────────────────────────────────────────────────────┐
│                    QUESTION DIMENSIONS                               │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│   1. DIFFICULTY              2. COURSE / TRACK                      │
│      • Basic                    • C# & .NET                         │
│      • Medium                   • SQL & Databases                   │
│      • Advanced                 • Data Structures                   │
│                                 • System Design                     │
│                                 • DevOps                           │
│                                 • Frontend (React/Angular)          │
│                                 • Cloud (AWS/Azure)                 │
│                                                                      │
│   3. LANGUAGE / RUNTIME      4. TOPICS (free tags)                  │
│      • csharp                   • linq, async-await, ef-core        │
│      • dotnet                   • joins, indexing, transactions     │
│      • sql                      • arrays, hashmaps, recursion       │
│      • python                   • microservices, caching            │
│      • java                     • harness, kubernetes, terraform    │
│      • javascript/typescript                                        │
│      • go, rust, cpp                                                │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

### Example Question Matrix

| Course | Language | Basic | Medium | Advanced |
|--------|----------|-------|--------|----------|
| C# & .NET | `csharp`, `dotnet` | 12 | 20 | 8 |
| SQL & Databases | `sql` | 15 | 18 | 6 |
| Data Structures | `csharp`, `java`, `python` | 20 | 25 | 10 |
| System Design | `text`, `csharp` | 4 | 8 | 6 |
| DevOps | `yaml`, `bash`, `text` | 6 | 10 | 5 |
| Frontend | `javascript`, `typescript` | 10 | 12 | 4 |

A **Client requirement** for "Senior .NET Developer" would pull questions like:
- Course: `C# & .NET` → Language: `csharp` → Difficulty: `medium`/`advanced`
- Course: `SQL & Databases` → Language: `sql` → Difficulty: `medium`
- Course: `System Design` → Difficulty: `advanced`

---

## 👥 User Personas & Workflows

### 1. 🏢 Client (Hiring Company)

**Primary Goals:** Define requirements, upload course-wise questions with test cases, schedule interviews, review feedback.

#### Workflow: Create Requirement + Upload Questions
```
Login → Requirements → New Requirement
    │
    ├── Step 1: Role Details
    │       ├── Title: "Senior .NET Developer"
    │       ├── Skills: [C#, .NET, SQL, Azure]
    │       └── Duration: 60 min
    │
    ├── Step 2: Define Courses
    │       ├── Course 1: "C# & .NET"   → Language: csharp
    │       ├── Course 2: "SQL & DB"    → Language: sql
    │       └── Course 3: "System Design" → Language: text
    │
    ├── Step 3: Upload Questions per Course
    │       ├── Upload C#_questions.json
    │       │       ├── Basic (12)
    │       │       ├── Medium (20)
    │       │       └── Advanced (8)
    │       ├── Upload SQL_questions.json
    │       │       ├── Basic (15)
    │       │       ├── Medium (18)
    │       │       └── Advanced (6)
    │       └── Upload SystemDesign_questions.json
    │               ├── Medium (8)
    │               └── Advanced (6)
    │
    ├── Step 4: Configure Feedback Template
    │       ├── C# Proficiency (1-5)
    │       ├── SQL Fundamentals (1-5)
    │       ├── System Design (text)
    │       └── Overall Recommendation
    │
    └── Step 5: Submit → Admin activates
```

#### Workflow: Review Interview Results
```
Login → Interviews → Select Completed Interview
    │
    ├── View Recording (HLS Stream)
    ├── Review Code Submissions
    │       ├── Question: "Implement Repository Pattern"
    │       ├── Course: C# & .NET | Difficulty: Medium
    │       ├── Test Cases: 7/10 passed ✅
    │       └── Time Spent: 18 min
    ├── Read Interviewer Feedback
    └── Make Hiring Decision
```

---

### 2. 👨‍💼 Interviewer

**Primary Goals:** Conduct interviews, select questions from client-provided sets, assess candidates.

#### Workflow: Conduct Interview
```
Login → Today's Schedule → Select Interview
    │
    ├── Pre-Interview Review
    │       ├── Client: HirePro
    │       ├── Role: Senior .NET Developer
    │       └── Available Questions:
    │               ├── C# & .NET (40)
    │               ├── SQL & DB (39)
    │               └── System Design (14)
    │
    ├── Start Interview
    │
    ├── Question Picker
    │       ├── Filter: Course=[C# & .NET ▼]
    │       │         Language=[csharp ▼]
    │       │         Difficulty=[● Medium  ○ Advanced]
    │       │
    │       ├── Results:
    │       │   • Implement Repository Pattern     [Preview] [Add]
    │       │   • Async/Await Deadlock Scenario    [Preview] [Add]
    │       │   • LINQ GroupBy Optimization        [Preview] [Add]
    │       │   • Dependency Injection Lifetimes   [Preview] [Add]
    │       │
    │       └── Preview shows:
    │               ├── Problem statement
    │               ├── Starter code
    │               ├── Sample test cases
    │               └── Expected duration
    │
    ├── During Interview
    │       ├── Push question → candidate screen
    │       ├── Watch real-time code changes
    │       ├── Candidate clicks "Run" → sample tests
    │       ├── Candidate clicks "Submit" → hidden tests
    │       └── See pass/fail + score in real-time
    │
    └── End Interview → Submit Feedback
```

#### Workflow: Question Selection Filter
```
Question Picker Panel
    │
    ├── Course:        [C# & .NET        ▼]
    ├── Language:      [csharp           ▼]
    ├── Difficulty:    [Medium] [Advanced]  (multi-select)
    ├── Topics:        [linq, async, di]  (autocomplete)
    └── Used before:   [ ] Hide              
    
    Sort by: [ Difficulty ↑ | Times Used | Avg Time ]
```

---

### 3. 👤 Candidate

**Primary Goals:** Attend interview, solve coding questions, run test cases.

#### Workflow: Attend Interview
```
Magic Link → Identity Verification → Pre-Check (Cam/Mic)
    │
    ├── Waiting Room
    │
    ├── Interview Session
    │       ├── 🎥 Video/Audio Active
    │       ├── 📝 Code Editor (language auto-set)
    │       ├── 📋 Problem Statement Panel
    │       ├── 🧪 Sample Test Cases (visible)
    │       ├── ▶️  Run Code (against samples)
    │       ├── 📤 Submit (against hidden tests)
    │       └── 💬 Chat with Interviewer
    │
    └── Session Complete
```

#### Candidate Code Editor View
```
┌──────────────────────────────────────────────────────────────┐
│ Question: Implement Repository Pattern                       │
│ Course: C# & .NET  |  Difficulty: Medium  |  Time: 20:00 ⏱  │
├──────────────────────────────────────────────────────────────┤
│ Problem Statement (Markdown rendered)                        │
│ ────────────────────────────────────────────────             │
│ Implement a generic repository pattern with...               │
│                                                              │
│ Examples:                                                    │
│   Input:  repo.Add(new User{...})                            │
│   Output: id assigned                                         │
├──────────────────────────────────────────────────────────────┤
│ Code Editor (Monaco) - csharp                                │
│ ────────────────────────────────────────────────             │
│ public interface IRepository<T> {                            │
│     T Add(T entity);                                          │
│     // Your code here                                         │
│ }                                                             │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│ [▶ Run]  [📤 Submit]                    Sample Tests (3/3) ✅│
│ ┌────────────────────────────────────────────────────────┐   │
│ │ ✅ tc-001: Add entity returns with ID                  │   │
│ │ ✅ tc-002: Add null throws ArgumentNullException       │   │
│ │ ✅ tc-003: Generic type constraint works               │   │
│ └────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────┘
```

---

### 4. 🔧 Admin

**Primary Goals:** Manage clients, review uploaded question sets, oversee the platform.

#### Workflow: Client Onboarding & Question Review
```
Login → Clients → HirePro → Requirements → "Senior .NET Developer"
    │
    ├── Review Uploaded Questions
    │       ├── C# & .NET:    40 questions ✅ validated
    │       ├── SQL & DB:     39 questions ✅ validated
    │       └── System Design: 14 questions ⚠️ 2 warnings
    │               ├── Warning: "tc-005 expected_output empty"
    │               └── Warning: "Missing solution_code for 'text' lang"
    │
    ├── Approve / Request Changes
    │
    ├── Assign Interviewers
    │
    └── Activate Requirement
```

---

## 🛠️ Technology Stack

### Frontend

| Application | Technology | Purpose |
|-------------|-----------|---------|
| Client Portal | React 18 + TypeScript | Hiring company interface |
| Interviewer App | React 18 + TypeScript | Interview conductor + question picker |
| Candidate Portal | React 18 + TypeScript | Interview participant + code editor |
| Admin Console | Angular 17 + TypeScript | Platform + question review |
| Code Editor | Monaco Editor | In-browser IDE |
| Shared UI Library | Storybook + Tailwind | Component library |

### Backend

| Service | Technology | Purpose |
|---------|-----------|---------|
| Core API | ASP.NET Core 8 | Business logic, CRUD, imports |
| Real-Time Hub | SignalR / Azure SignalR | WebSocket communication |
| Media Server | LiveKit / Azure ACS | WebRTC SFU |
| Code Sandbox | Docker + Kubernetes | Multi-language code execution |
| Background Jobs | Hangfire / Azure Functions | Import processing, scoring |
| Import Parser | CsvHelper, YamlDotNet, EPPlus | JSON/YAML/Excel ingestion |

### Data & Storage

| Component | Technology | Purpose |
|-----------|-----------|---------|
| Primary DB | PostgreSQL 16 | Relational + JSONB for questions |
| Cache/PubSub | Redis 7 | Session, real-time state |
| Media Storage | AWS S3 / Azure Blob | Recordings, files |
| CDN | CloudFront / Azure CDN | Video delivery |
| Search | PostgreSQL GIN indexes | Question filtering |

### Infrastructure

| Component | Technology | Purpose |
|-----------|-----------|---------|
| Container Orchestration | Kubernetes (AKS/EKS) | Service deployment |
| API Gateway | Azure APIM / AWS API GW | Routing, rate limiting |
| CI/CD | GitHub Actions / Azure DevOps | Automated deployment |
| Monitoring | Prometheus + Grafana | Metrics, alerting |
| Logging | ELK Stack / Azure Monitor | Centralized logs |
| Secrets | Azure Key Vault / AWS Secrets | Credential management |

---

## 📊 Database Schema (High-Level)

```sql
-- Shared schema (across all tenants)
CREATE TABLE shared.tenants (
    id              UUID PRIMARY KEY,
    name            VARCHAR(200),
    schema_name     VARCHAR(100),      -- 'tenant_hirepro'
    routing_mode    VARCHAR(20),       -- 'shared' | 'dedicated'
    created_at      TIMESTAMPTZ DEFAULT NOW()
);

-- Inside each tenant schema (e.g., tenant_hirepro)

-- 1. Requirements (job openings)
CREATE TABLE requirements (
    id                    UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title                 VARCHAR(200) NOT NULL,
    description           TEXT,
    skills                TEXT[],
    feedback_template_id  UUID,
    status                VARCHAR(20) DEFAULT 'draft',
    created_at            TIMESTAMPTZ DEFAULT NOW()
);

-- 2. Courses (per requirement, e.g., "C# & .NET", "SQL")
CREATE TABLE requirement_courses (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    requirement_id    UUID NOT NULL REFERENCES requirements(id) ON DELETE CASCADE,
    code              VARCHAR(50) NOT NULL,      -- 'CSHARP_DOTNET'
    name              VARCHAR(200) NOT NULL,     -- 'C# & .NET'
    description       TEXT,
    languages         TEXT[] NOT NULL,           -- ['csharp','dotnet']
    display_order     INT DEFAULT 0,
    
    UNIQUE (requirement_id, code)
);

-- 3. Questions (scoped to requirement + course)
CREATE TABLE requirement_questions (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    requirement_id      UUID NOT NULL REFERENCES requirements(id) ON DELETE CASCADE,
    course_id           UUID NOT NULL REFERENCES requirement_courses(id) ON DELETE CASCADE,
    
    external_id         VARCHAR(100),            -- client's own ID
    title               VARCHAR(500) NOT NULL,
    difficulty          VARCHAR(20) NOT NULL 
                        CHECK (difficulty IN ('basic','medium','advanced')),
    language            VARCHAR(50) NOT NULL,    -- 'csharp', 'sql', 'python'
    topics              TEXT[] DEFAULT '{}',     -- ['linq','async','ef-core']
    
    problem_statement   JSONB NOT NULL,
    function_signature  JSONB NOT NULL,
    starter_code        JSONB NOT NULL,
    solution_code       JSONB NOT NULL,
    
    time_limit_ms       INT DEFAULT 2000,
    memory_limit_mb     INT DEFAULT 256,
    evaluation_config   JSONB DEFAULT '{}',
    
    is_active           BOOLEAN DEFAULT true,
    created_at          TIMESTAMPTZ DEFAULT NOW(),
    
    UNIQUE (requirement_id, external_id)
);

CREATE INDEX idx_questions_filter 
    ON requirement_questions(requirement_id, course_id, language, difficulty) 
    WHERE is_active = true;

CREATE INDEX idx_questions_topics 
    ON requirement_questions USING GIN(topics);

-- 4. Test Cases
CREATE TABLE requirement_test_cases (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    question_id         UUID NOT NULL REFERENCES requirement_questions(id) ON DELETE CASCADE,
    external_id         VARCHAR(100),
    type                VARCHAR(20) NOT NULL 
                        CHECK (type IN ('sample','hidden','performance')),
    name                VARCHAR(200),
    input_data          JSONB,
    expected_output     JSONB,
    visible_to_candidate BOOLEAN DEFAULT false,
    points              INT DEFAULT 10,
    explanation         TEXT,
    display_order       INT DEFAULT 0
);

-- 5. Interview Sessions (question selection snapshot)
CREATE TABLE interview_sessions (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    interview_id      UUID NOT NULL,
    question_id       UUID NOT NULL REFERENCES requirement_questions(id),
    selected_at       TIMESTAMPTZ DEFAULT NOW(),
    selected_by       UUID NOT NULL,
    time_spent_sec    INT,
    final_score       DECIMAL(5,2),
    code_snapshot_url TEXT
);

-- 6. Feedback (dynamic schema)
CREATE TABLE feedback_templates (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    requirement_id UUID NOT NULL REFERENCES requirements(id),
    name          VARCHAR(200),
    schema        JSONB NOT NULL,      -- dynamic fields
    created_at    TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE feedback_responses (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    interview_id      UUID NOT NULL,
    template_id       UUID NOT NULL REFERENCES feedback_templates(id),
    responses         JSONB NOT NULL,
    overall_rating    DECIMAL(3,2),
    recommendation    VARCHAR(20),
    submitted_at      TIMESTAMPTZ DEFAULT NOW()
);

-- 7. Import Jobs
CREATE TABLE requirement_import_jobs (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    requirement_id      UUID NOT NULL REFERENCES requirements(id),
    submitted_by        UUID NOT NULL,
    source_filename     VARCHAR(500),
    source_hash         VARCHAR(64),
    status              VARCHAR(20),
    imported_count      INT DEFAULT 0,
    warning_count       INT DEFAULT 0,
    failed_count        INT DEFAULT 0,
    validation_report   JSONB DEFAULT '{}',
    created_at          TIMESTAMPTZ DEFAULT NOW(),
    completed_at        TIMESTAMPTZ
);
```

---

## 📥 Question Import Format (Client Uploads)

```json
{
  "bundle_version": "1.0",
  "course": {
    "code": "CSHARP_DOTNET",
    "name": "C# & .NET",
    "description": "Core .NET development questions",
    "languages": ["csharp", "dotnet"]
  },
  "questions": [
    {
      "external_id": "HP-CS-001",
      "title": "Implement Repository Pattern",
      "difficulty": "medium",
      "language": "csharp",
      "topics": ["design-patterns", "ef-core", "generics"],
      
      "problem_statement": {
        "markdown": "Implement a generic `IRepository<T>` interface..."
      },
      
      "starter_code": {
        "csharp": "public interface IRepository<T> {\n    // Your code here\n}"
      },
      
      "solution_code": {
        "csharp": "public interface IRepository<T> where T : class {\n    T Add(T entity);\n    ...\n}"
      },
      
      "test_cases": [
        {
          "id": "tc-001",
          "type": "sample",
          "name": "Add entity returns with ID",
          "input": { "code": "repo.Add(new User{Name=\"A\"})" },
          "expected_output": { "id": "not_null" },
          "visible_to_candidate": true,
          "points": 10
        },
        {
          "id": "tc-002",
          "type": "hidden",
          "name": "Null entity throws",
          "input": { "code": "repo.Add(null)" },
          "expected_output": { "exception": "ArgumentNullException" },
          "visible_to_candidate": false,
          "points": 20
        }
      ],
      
      "evaluation": {
        "time_limit_ms": 2000,
        "memory_limit_mb": 256,
        "partial_credit": true
      }
    },
    {
      "external_id": "HP-CS-002",
      "title": "Async/Await Deadlock Scenario",
      "difficulty": "advanced",
      "language": "csharp",
      "topics": ["async-await", "deadlock", "task"],
      "problem_statement": { "markdown": "Fix the deadlock in the following code..." }
    }
  ]
}
```

Sample SQL course bundle:
```json
{
  "bundle_version": "1.0",
  "course": {
    "code": "SQL_DB",
    "name": "SQL & Databases",
    "languages": ["sql"]
  },
  "questions": [
    {
      "external_id": "HP-SQL-001",
      "title": "Find Nth Highest Salary",
      "difficulty": "medium",
      "language": "sql",
      "topics": ["joins", "subqueries", "ranking"],
      "problem_statement": { "markdown": "Write a query to find the Nth highest salary..." },
      "test_cases": [
        { "id": "tc-001", "type": "sample", "input": { "n": 2 }, "expected_output": "85000" },
        { "id": "tc-002", "type": "hidden", "input": { "n": 1 }, "expected_output": "120000" }
      ]
    }
  ]
}
```

---

## 🔄 Core System Workflows

### Workflow: Question Import → Live Interview → Scoring

```
┌──────────────────────────────────────────────────────────────────────┐
│                     END-TO-END QUESTION FLOW                          │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  CLIENT (React)                                                       │
│  ┌─────────────────────────────────────────┐                         │
│  │ Upload questions.json per course        │                         │
│  │  • Course: C# & .NET                    │                         │
│  │  • Language: csharp                     │                         │
│  │  • Difficulty: basic/medium/advanced    │                         │
│  │  • Test cases: sample + hidden          │                         │
│  └────────────────┬────────────────────────┘                         │
│                   │                                                   │
│                   ▼                                                   │
│  IMPORT PIPELINE (ASP.NET Core + Hangfire)                            │
│  ┌─────────────────────────────────────────┐                         │
│  │ 1. Parse JSON                            │                         │
│  │ 2. Validate schema                       │                         │
│  │ 3. Verify solution vs test cases         │                         │
│  │ 4. Store in requirement_questions        │                         │
│  │    + requirement_test_cases              │                         │
│  └────────────────┬────────────────────────┘                         │
│                   │                                                   │
│                   ▼                                                   │
│  INTERVIEWER (React)                                                  │
│  ┌─────────────────────────────────────────┐                         │
│  │ Filter: Course + Language + Difficulty   │                         │
│  │ Select question → push to session        │                         │
│  └────────────────┬────────────────────────┘                         │
│                   │                                                   │
│                   ▼                                                   │
│  CANDIDATE (React)                                                    │
│  ┌─────────────────────────────────────────┐                         │
│  │ See problem + starter code (language)   │                         │
│  │ Write code → Run → sample tests         │                         │
│  │ Submit → hidden tests                   │                         │
│  └────────────────┬────────────────────────┘                         │
│                   │                                                   │
│                   ▼                                                   │
│  SANDBOX (Docker/K8s)                                                 │
│  ┌─────────────────────────────────────────┐                         │
│  │ Execute code in isolated container       │                         │
│  │ Run all test cases                       │                         │
│  │ Compute score (partial credit)           │                         │
│  │ Stream results back via SignalR          │                         │
│  └────────────────┬────────────────────────┘                         │
│                   │                                                   │
│                   ▼                                                   │
│  INTERVIEWER SEES SCORE → Submits feedback                            │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

---

## 🔐 Security & Compliance

- **Authentication**: OAuth 2.0 / OIDC with Azure AD / Okta
- **RBAC**: `super_admin`, `client_admin`, `interviewer`, `candidate`
- **Encryption**: AES-256 at rest, TLS 1.3 in transit
- **Code Sandbox**: Isolated containers, no network, strict resource limits
- **Data isolation**: Schema-per-tenant, questions never cross clients
- **Recording consent**: Explicit candidate acknowledgment
- **GDPR/CCPA**: Data retention configurable per client

---

## 🚀 Deployment Architecture

### Environments
| Environment | DB | Video Storage | SignalR |
|-------------|-----|--------------|---------|
| Development | Local Docker | Local MinIO | Local |
| Staging | Small RDS | S3 dev bucket | Dev Azure SignalR |
| Production | HA RDS Multi-AZ | S3 prod (versioned) | Prod Azure SignalR |

### High Availability
```
Route 53 / Traffic Manager
    │
    ├── Region US-East ─┐
    ├── Region EU-West ─┼── Kubernetes Cluster (3-20 nodes)
    └── Region AP-South ┘
            ├── API Pods (3-10)
            ├── SignalR Pods (2-5)
            ├── Worker Pods (2-8)
            └── Sandbox Pods (auto-scale)
```

---

## 📈 Scalability Targets

| Metric | Target |
|--------|--------|
| API Response (p95) | < 200ms |
| Video Latency | < 150ms |
| Code Execution | < 5s |
| Concurrent Interviews | 10,000+ |
| Recording Availability | < 2 min post-interview |
| Question Filter Query | < 50ms |

---

## 📅 Development Roadmap

### Phase 1: Foundation (Months 1-3)
- [ ] User auth & RBAC
- [ ] Requirements + Courses CRUD
- [ ] Question import (JSON only)
- [ ] Basic scheduling

### Phase 2: Real-Time (Months 4-6)
- [ ] WebRTC video/audio
- [ ] SignalR code sync
- [ ] Code sandbox (C#, SQL, Python, JS)
- [ ] Sample test case execution

### Phase 3: Question Intelligence (Months 7-9)
- [ ] Multi-format import (YAML, Excel)
- [ ] Hidden test cases + scoring
- [ ] Question picker with filters (course/language/difficulty)
- [ ] Dynamic feedback templates

### Phase 4: Scale (Months 10-12)
- [ ] Multi-region deployment
- [ ] Advanced monitoring
- [ ] AI-powered question suggestions
- [ ] SOC 2 / ISO 27001 certification

---

## 📚 Sample API Endpoints

### Question Management
```
POST   /api/v1/requirements/{id}/courses
POST   /api/v1/requirements/{id}/courses/{courseId}/questions/import
POST   /api/v1/requirements/{id}/courses/{courseId}/questions/import/preview
GET    /api/v1/requirements/{id}/questions
         ?course=CSHARP_DOTNET&language=csharp&difficulty=medium,advanced
GET    /api/v1/requirements/{id}/questions/{qid}
PUT    /api/v1/requirements/{id}/questions/{qid}
DELETE /api/v1/requirements/{id}/questions/{qid}
```

### Interview Session
```
POST   /api/v1/interviews/{id}/select-question   { questionId }
POST   /api/v1/interviews/{id}/code/run          { code, language }
POST   /api/v1/interviews/{id}/code/submit       { code, language }
GET    /api/v1/interviews/{id}/test-results
POST   /api/v1/interviews/{id}/feedback
```

### Example: Filter Questions During Interview
```http
GET /api/v1/requirements/req_abc123/questions
    ?course=CSHARP_DOTNET
    &language=csharp
    &difficulty=medium,advanced
    &topics=linq,async
Authorization: Bearer {token}

Response:
{
  "total": 23,
  "questions": [
    {
      "id": "q_001",
      "title": "Implement Repository Pattern",
      "course": "C# & .NET",
      "language": "csharp",
      "difficulty": "medium",
      "topics": ["design-patterns", "ef-core"],
      "estimatedMinutes": 20,
      "testCaseCount": 10
    },
    ...
  ]
}
```

---

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/question-import`)
3. Commit changes
4. Open Pull Request

---

## 📄 License

Copyright © 2024 InterviewHub. All rights reserved.

---

*Last Updated: December 2024*

**Summary of what changed in this version:**
- ✅ Added explicit **Question Taxonomy** (Difficulty × Course × Language × Topics)
- ✅ Added `language` and `topics` columns to `requirement_questions`
- ✅ Added `languages[]` to `requirement_courses` for multi-language courses
- ✅ Included **C#, .NET, SQL** as concrete examples throughout
- ✅ Added Question Picker workflow with **course + language + difficulty** filters
- ✅ Candidate editor shows **language-specific** code with sample/hidden test cases
- ✅ Import JSON now includes `language` per question
