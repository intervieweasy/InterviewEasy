# InterviewEasy Specifications

Specification-driven development (SDD) artifacts for InterviewEasy.
Every feature starts as a spec, is reviewed, planned, implemented, and marked done.

## The Loop

1. Spec - Write `{feature}.spec.md`
2. Review - Approve or revise
3. Plan - Write `{feature}.plan.md`
4. Implement - Code + tests together
5. Verify - All acceptance criteria checked
6. Commit - Conventional commit, push

## Folder Layout

- `_templates/` - Spec + plan templates
- `000-foundation/` - Solution structure, DB strategy, event bus
- `001-identity/` - Auth, users, tenants
- `002-requirement/` - Client requirements, courses
- `003-question/` - Questions, test cases, import
- `004-sandbox/` - Code execution
- `005-session/` - Live interview orchestration
- `006-feedback/` - Dynamic feedback forms
- `007-proctoring/` - Integrity and cheating detection
- `008-notification/` - Email, SMS, Slack
- `009-recording/` - Video recording pipeline
- `010-analytics/` - Reports, dashboards
- `011-bff-gateway/` - Frontend aggregators
- `012-frontends/` - React + Angular apps

## Rules

1. No code without an approved spec.
2. One spec at a time - do not skip ahead.
3. Every acceptance criterion must be testable.
4. Mark spec as Implemented only after tests pass.
5. Update the Last Updated date on every change.