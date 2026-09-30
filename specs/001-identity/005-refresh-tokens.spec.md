
---

# 📄 Spec 001-identity/005 — Refresh Tokens & Sessions

Save as: `specs\001-identity\005-refresh-tokens.spec.md`

---

```markdown
# Spec: Refresh Tokens & Sessions

**Service:** Identity
**Phase:** 001-identity
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Access tokens (JWT) are short-lived (15 minutes) to limit exposure if
stolen. Refresh tokens allow clients to obtain new access tokens
without re-entering credentials. They are long-lived (7 days), stored
securely server-side, and rotated on each use.

This spec defines the refresh flow, token rotation, session management,
and logout semantics. It builds on Spec 001-identity-003 (Authentication).

---

## 2. Scope

### In Scope

- Refresh token issuance (on login)
- Refresh endpoint
- Token rotation (single-use refresh tokens)
- Refresh token storage (hashed, per device)
- Session listing and revocation
- Logout single device vs all devices
- Refresh token reuse detection
- Refresh token cleanup

### Out of Scope

- Access token issuance (Spec 001-identity-003)
- Login flow (Spec 001-identity-003)
- Device fingerprinting (future)
- Concurrent session limits (future — see Open Questions)

---

## 3. User Stories

- **US-1:** As a **user**, I want to stay logged in for 7 days
 