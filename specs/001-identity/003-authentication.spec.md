
---

# 📄 Spec 001-identity/003 — Authentication

Save as: `specs\001-identity\003-authentication.spec.md`

---

```markdown
# Spec: Authentication

**Service:** Identity
**Phase:** 001-identity
**Status:** Draft
**Author:** InterviewEasy Team
**Created:** 2026-09-30
**Last Updated:** 2026-09-30

---

## 1. Context

Authentication verifies who a user is. In InterviewEasy, the login
flow is the entry point for client admins, recruiters, and interviewers.
Candidates do NOT log in — they use signed magic links (covered
separately in Phase 5).

Authentication issues JWT access tokens (short-lived) and refresh
tokens (long-lived, stored in HTTP-only cookies or a dedicated
endpoint — see Spec 001-identity-005).

This spec defines the login flow, password verification, token
issuance, and account lockout handling.

---

## 2. Scope

### In Scope

- Login endpoint (email + password)
- Password verification
- Account lockout handling
- JWT access token issuance
- Refresh token issuance (details in Spec 005)
- Token claims
- Login audit
- Failed login tracking
- Logout endpoint
- Current user info endpoint (from token)

### Out of Scope

- User registration (Spec 001-identity-002)
- Refresh token rotation (Spec 001-identity-005)
- Password reset (Spec 001-identity-006)
- SSO / OAuth providers (future)
- Multi-factor authentication (future)
- Candidate magic-link auth (Phase 5)

---

## 3. User Stories

- **US-1:** As a **user**, I want to log in with email and password,
  so that I can access the platform.

- **US-2:** As a **user**, I want to stay logged in across sessions,
  so that I don't have to log in every time.

- **US-3:** As a **user**, I want to log out from any device, so that
  I can secure my account.

- **US-4:** As an **SRE**, I want to see failed login attempts, so
  that I can detect brute-force attacks.

- **US-5:** As a **security officer**, I want lockout after N failed
  attempts, so that brute-force is mitigated.

---

## 4. Functional Requirements

### 4.1 Login Endpoint

- **FR-1:** `POST /api/v1/auth/login` SHALL authenticate a user with
  email, password, and tenant code.

- **FR-2:** The login request SHALL include:
  - `email` (string)
  - `password` (string)
  - `tenantCode` (string) — which tenant the user belongs to

- **FR-3:** The response SHALL include:
  - `accessToken` (JWT string)
  - `refreshToken` (string — see Spec 005)
  - `expiresIn` (seconds)
  - `tokenType` (`"Bearer"`)
  - `user` (basic user info)

- **FR-4:** Login response SHALL NOT include password hash or
  verification tokens.

- **FR-5:** Invalid credentials SHALL return 401 with a **generic**
  message — no distinction between unknown email and wrong password.

- **FR-6:** Missing tenant SHALL return 401 (not 404) — do not leak
  tenant existence.

- **FR-7:** Suspended tenants SHALL return 403 with a clear message
  (`"Tenant suspended"`).

- **FR-8:** Suspended users SHALL return 401 (generic).

### 4.2 Password Verification

- **FR-9:** Passwords SHALL be verified using Argon2id (matching the
  hash from Spec 001-identity-002).

- **FR-10:** Password verification SHALL be constant-time — no timing
  leak of whether the email exists.

- **FR-11:** If a user does not exist, the system SHALL still perform
  a dummy hash verification to normalize response time.

### 4.3 JWT Access Tokens

- **FR-12:** Access tokens SHALL be JWT (JSON Web Tokens) signed with
  HMAC-SHA256 (HS256) or RS256 (asymmetric).

- **FR-13:** The signing key SHALL be loaded from Key Vault
  (Spec 006) and be at least 256 bits.

- **FR-14:** Access tokens SHALL expire after 15 minutes.

- **FR-15:** Access token claims SHALL include:
  - `sub` — user ID (Guid)
  - `tenant_id` — tenant ID (Guid)
  - `tenant_code` — tenant code (string)
  - `email` — user email
  - `name` — user full name
  - `roles` — array of role names
  - `permissions` — array of permission strings (optional, may be
    resolved server-side)
  - `iss` — `"intervieweasy"`
  - `aud` — `"intervieweasy-api"`
  - `iat` — issued at
  - `exp` — expires at (15 min from iat)
  - `jti` — unique token ID (for revocation)

- **FR-16:** Tokens SHALL be signed with a key that rotates every 90
  days — old keys kept for verification until tokens expire.

- **FR-17:** Tokens SHALL be validated on every request via the
  ASP.NET Core JWT middleware.

### 4.4 Refresh Tokens

- **FR-18:** A refresh token SHALL be issued on successful login.

- **FR-19:** Refresh tokens SHALL be opaque (not JWT) — a random
  256-bit string.

- **FR-20:** Refresh tokens SHALL be stored in the database with:
  - Hash of the token
  - User ID
  - Tenant ID
  - Expiry (7 days)
  - Device info (user agent, IP)

- **FR-21:** Refresh tokens SHALL be single-use (rotation — see Spec 005).

- **FR-22:** Refresh tokens SHALL be sent to the client in an
  HTTP-only, Secure, SameSite=Strict cookie.

### 4.5 Login Audit

- **FR-23:** Every login attempt (success or failure) SHALL be logged
  with:
  - Email (hashed)
  - Tenant code
  - Timestamp (UTC)
  - IP address
  - User agent
  - Outcome (success, invalid credentials, locked, suspended)

- **FR-24:** Successful logins SHALL update `LastLoginAt` on the user.

- **FR-25:** Failed logins SHALL increment `FailedLoginCount`.

- **FR-26:** On lockout, the user SHALL be notified via email.

### 4.6 Account Lockout

- **FR-27:** After 5 consecutive failed login attempts, the account
  SHALL lock for 15 minutes.

- **FR-28:** Lockout SHALL return 401 with a message (`"Account locked.
  Try again later."`) — but only AFTER valid email+password match, to
  avoid leaking account existence.

- **FR-29:** A successful login SHALL reset `FailedLoginCount` to 0
  and clear `LockedUntil`.

- **FR-30:** Lockout counter SHALL reset every 60 minutes (sliding
  window).

### 4.7 Logout

- **FR-31:** `POST /api/v1/auth/logout` SHALL:
  - Revoke the current refresh token
  - Clear the refresh token cookie
  - Return 204

- **FR-32:** Logout SHALL be idempotent — repeated calls return 204.

- **FR-33:** Access tokens SHALL NOT be revocable (short expiry is
  the safety net).

- **FR-34:** A user SHALL be able to log out from all devices via
  `POST /api/v1/auth/logout-all` — revokes all refresh tokens.

### 4.8 Current User Info

- **FR-35:** `GET /api/v1/auth/me` SHALL return the current user info
  derived from the access token + database.

- **FR-36:** The response SHALL include: user ID, email, name, tenant
  ID, tenant code, roles, permissions.

- **FR-37:** `/auth/me` SHALL validate the token and return 401 if
  expired or invalid.

### 4.9 Security Headers and Cookies

- **FR-38:** The login endpoint SHALL be rate-limited:
  - 10 attempts per minute per IP
  - 5 attempts per minute per email

- **FR-39:** The refresh token cookie SHALL be:
  - `HttpOnly`
  - `Secure` (HTTPS only)
  - `SameSite=Strict`
  - `Path=/api/v1/auth`

- **FR-40:** CORS SHALL allow only registered frontend origins
  (see Spec 007).

---

## 5. Non-Functional Requirements

- **NFR-1:** Login SHALL respond in under 500ms (p95) — including
  password verification.

- **NFR-2:** `/auth/me` SHALL respond in under 100ms (p95).

- **NFR-3:** Login endpoint SHALL sustain 100 requests/second.

- **NFR-4:** JWT validation SHALL add under 5ms per request.

- **NFR-5:** Zero password leakage in logs — passwords never logged.

- **NFR-6:** Failed login attempts SHALL be recorded even if the user
  doesn't exist.

- **NFR-7:** Access tokens SHALL NOT be stored client-side in
  localStorage — use in-memory + refresh via cookie.

---

## 6. Domain Model

### 6.1 Value Objects

- `AccessToken` — JWT string + expiry + claims
- `RefreshToken` — raw token + hashed token + user context
- `LoginAttempt` — audit record

### 6.2 Service Contracts

```csharp
public interface IAuthenticationService
{
    Task<LoginResult> LoginAsync(LoginRequest request,
        CancellationToken ct);
    Task LogoutAsync(Guid userId, string refreshToken,
        CancellationToken ct);
    Task LogoutAllAsync(Guid userId, CancellationToken ct);
    Task<UserInfo> GetCurrentUserAsync(Guid userId,
        CancellationToken ct);
}

public interface ITokenService
{
    AccessToken GenerateAccessToken(User user, IList<string> roles,
        IList<string> permissions);
    string GenerateRefreshToken();
    string HashRefreshToken(string raw);
}