# Document Access Approval System — README

## Overview
REST API for managing access to internal documents with a role-based approval workflow.

- **Stack:** ASP.NET Core, MediatR (CQRS), EF Core (Write/Read), SQLite/InMemory  
- **Actors:** `User` → requests access; `Approver` → approves/rejects; `Admin` → issues invite tokens, sees all  
- **Authentication:** JWT (`uid` – Guid of the user, `role` – roles) + **refresh tokens**  
- **Outbox + BackgroundJob:** events → projectors (including a notification stub)  
- **Docker:** container build after tests

---

## Project structure and key design decisions
**Solution split into layers for CQRS:**
- **DMS.Contracts** — DTOs, request/response models, event contracts  
- **DMS.Domain** — business entities, enums, value objects  
- **DMS.Application** — use case handlers, commands/queries, business logic  
- **DMS.Infrastructure.Write** — write-side EF Core context and repositories  
- **DMS.Infrastructure.Read** — read-side EF Core context for projections  
- **DMS.Web** — ASP.NET Core API controllers, dependency injection, startup logic

**Design principles:**
- **CQRS**: separate read and write models for scalability and simplicity  
- **JWT auth with refresh**: access token + refresh token rotation with parent `jti` check  
- **Role-based authorization**: enforced via `[Authorize(Roles = "...")]`  
- **Outbox pattern**: ensures reliable event publishing for projectors and background jobs  
- **Read projections**: kept in sync via event handlers (projectors)  
- **No direct document listing for users without invites**: prevents information leaks via document titles

---

## Roles and Access Rules
- **User**  
  - cannot see the document list or even know about their existence without an invite  
  - can see their own **invites (tokens)**; at this point only the document title becomes visible, preventing any unauthorized guessing about content  
  - can request access to a document using the token  
- **Approver**  
  - sees all access requests  
  - can approve or reject them  
- **Admin**  
  - issues **invite tokens** for documents  
  - sees all invites (including expired ones) and all requests

---

## Health Check
### `GET /api/ping`
Returns `"pong"`. Used for basic service availability check.

---

## Authentication
### `POST /api/auth/login`
**Body:**  
```json
{ "username": "user", "password": "pass" }
```
**200:**  
```json
{ "accessToken": "<JWT>", "refreshToken": "<REFRESH>" }
```
**401:**  
```json
{ "error": "Invalid username or password" }
```

### `POST /api/auth/refresh`
**Body:**  
```json
{ "refreshToken": "<REFRESH>", "lastAccessToken": "<JWT>" }
```
**200:** — same as login  
**401:** — invalid/expired refresh

---

## Documents (Admin)
### `GET /api/documents/get-list`
Returns the full list of documents. Available only to admins.

---

## Users (Admin)
### `GET /api/users/get-list`
Returns the list of all users with their IDs and usernames.

---

## Document Access

### 1) Issue Invite (Admin)
`POST /api/documentaccesses/{documentId}/issue-access-invite`  
Creates an invite token for a specific user and document.

**Body:**
```json
{
  "userId": "f3a6a2f0-3f4a-4c2f-a2af-52b53b07e72e",
  "expiresAtUtc": "2025-09-01T12:00:00Z"
}
```

---

### 2) My Invites
`GET /api/documentaccesses/invites?includeExpired=false`  
Returns a list of invites for the authenticated user. Admins see all invites.  
Only the document title is visible when an invite exists.

---

### 3) Request Access
`POST /api/documentaccesses/request-access`  
Uses an invite token to submit an access request with a reason and type.

**Body:**
```json
{
  "token": "INVITE-TOKEN-STRING",
  "reason": "Need access to review project documentation",
  "type": 1
}
```

---

### 4) List Requests
`GET /api/documentaccesses/get-list`  
Returns access requests based on user role:  
- Admin/Approver — all requests  
- User — only their own

---

### 5) Approve/Reject
`POST /api/documentaccesses/{inviteId}/approve`  
Approver/Admin sets the decision and optional comment.  
Triggers a notification event to the user (stub).

**Body:**
```json
{
  "status": 1,
  "comment": "Approved, valid until end of quarter"
}
```


---

## Events and Background Jobs

### Outbox Events
- `AccessInviteIssuedEvent` — invite created  
- `AccessRequestSubmittedEvent` — request submitted  
- `AccessRequestDecidedEvent` — request decided

### Projectors
- Update read models for invites, requests, and decisions  
- Send notification stubs

---

## Future Improvements (time permitting)
1. Full unit test coverage; positive scenario service tests  
2. Structured logging  
3. Request/response time measurement for service monitoring  
4. Health check across all layers, including database availability
