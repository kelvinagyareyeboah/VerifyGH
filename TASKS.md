# VerifyGH — Team Tasks & Role Breakdown

> **DCIT 318 - Semester Project Assignment 1**  
> **Project Lead:** Agyare Kelvin Yeboah  
> **Status Tracking Document**

---

## 📅 Project Phases Overview

- [x] **Phase 1: Solution Architecture & Project Scaffolding** _(Completed)_
- [ ] **Phase 2: Database Schema & Authentication Layer**
- [ ] **Phase 3: Core API Endpoints & Business Logic**
- [ ] **Phase 4: Blazor Client UI Pages & MudBlazor Styling**
- [ ] **Phase 5: API Client Integration & SignalR Real-Time Hub**
- [ ] **Phase 6: QA Testing, Security Audit & Final Report**

---

## 👥 Individual Task Assignments

---

### 1. Agyare Kelvin Yeboah (22159683)

**Role:** Project Lead / Backend Architecture  
**Working Directory:** `src/VerifyGH.Shared/`, `src/VerifyGH.Server/`, `VerifyGH.slnx`

- [x] Scaffold multi-project solution (`Shared`, `Server`, `Client`).
- [x] Configure solution references and foundational NuGet packages.
- [ ] Define core domain models and shared contracts in `VerifyGH.Shared/DTOs/`.
- [ ] Implement global exception handling middleware and standard API response envelope.
- [ ] Conduct code reviews on pull requests from backend and frontend developers.
- [ ] Lead team milestone check-ins and ensure assignment requirements are met.

---

### 2. Anthony Gudu (22014087)

**Role:** Authentication & Security  
**Working Directory:** `src/VerifyGH.Server/Controllers/`, `src/VerifyGH.Server/Data/`

- [x] Implement `AuthController.cs` with the following endpoints:
  - `POST /api/auth/register` (Registration with Role: Student, Lecturer, Employer)
  - `POST /api/auth/login` (Returns JWT access token with role claims)
  - `GET /api/auth/me` (Returns authenticated user profile)
- [x] Configure ASP.NET Identity role seeding (`Student`, `Lecturer`, `Employer`, `Admin`) on application startup.
- [x] Enforce role-based authorization attributes (`[Authorize(Roles = "Lecturer")]`, etc.) across protected endpoints.
- [x] Implement password strength policies and secure claims generation.

---

### 3. Adjei David Boafo (22046873)

**Role:** Database Design  
**Working Directory:** `src/VerifyGH.Server/Models/`, `src/VerifyGH.Server/Data/`

- [ ] Create EF Core entity classes:
  - `Project.cs` (Title, Description, RepoUrl, LiveDemoUrl, DocumentUrl, StudentId, Status, CreatedAt)
  - `VerificationReview.cs` (ProjectId, LecturerId, Comments, Status, ReviewedAt)
  - `Skill.cs` and `ProjectSkill.cs` (Skill name, category, many-to-many relationship with Projects)
- [ ] Configure model relationships, foreign keys, and constraints in `ApplicationDbContext.cs`.
- [ ] Create and apply initial EF Core Migration:
  ```powershell
  dotnet ef migrations add InitialCreate --project src/VerifyGH.Server
  dotnet ef database update --project src/VerifyGH.Server
  ```
- [ ] Create seed data for testing (sample students, lecturers, skills, and projects).

---

### 4. Boadu-Acheampong Asante Yaw (22152286)

**Role:** Backend Developer  
**Working Directory:** `src/VerifyGH.Server/Controllers/`, `src/VerifyGH.Server/Services/`

- [ ] Implement `ProjectsController.cs`:
  - `GET /api/projects` (List projects with optional filtering by status/skill)
  - `GET /api/projects/{id}` (Get project details by ID)
  - `POST /api/projects` (Create student submission)
  - `PUT /api/projects/{id}` (Update submission before approval)
  - `DELETE /api/projects/{id}` (Delete submission)
- [ ] Implement `VerificationController.cs`:
  - `GET /api/verification/pending` (Fetch pending submissions assigned to current lecturer)
  - `POST /api/verification/{id}/review` (Submit lecturer approval/rejection with comments)
- [ ] Ensure proper input validation and error messages on all controller actions.

---

### 5. Ametefe Kwadwo Elijah (22040783)

**Role:** UI/UX Design  
**Working Directory:** `src/VerifyGH.Client/Layout/`, `src/VerifyGH.Client/wwwroot/`

- [ ] Refine MudBlazor custom theme palette (Ghana-inspired green/gold/dark accents).
- [ ] Design reusable UI components:
  - Status badge chip component (Green for Approved, Amber for Pending, Red for Needs Revision)
  - Project summary card component with author avatar and skill chips
- [ ] Ensure mobile responsiveness and drawer navigation on tablet/mobile screens.
- [ ] Optimize typography, spacing, and micro-interactions for a professional look.

---

### 6. Frank Bless Kofi Tsetse (22027295)

**Role:** Frontend Developer (Student Portal)  
**Working Directory:** `src/VerifyGH.Client/Pages/`

- [ ] Implement `UploadProject.razor`:
  - Input fields: Title, description, GitHub repository URL, live demo link
  - Supervisor dropdown (selecting accredited lecturer)
  - Multi-select skill tagging input
  - Submit button with loading spinner and validation messages
- [ ] Implement `MyProjects.razor`:
  - Display student's own submissions as a responsive grid of MudCards
  - Show review status badges and lecturer feedback if revision is requested

---

### 7. Tieku Justice (22105235)

**Role:** Frontend Developer (Lecturer Portal)  
**Working Directory:** `src/VerifyGH.Client/Pages/`

- [ ] Implement `Verification.razor`:
  - Tabbed or table view of submissions: "Pending Reviews", "Reviewed", "All"
  - Student profile and submission date columns
  - Action button to open "Review Modal"
- [ ] Create Review Modal Dialog:
  - Display student's project code links and attached documents
  - Radio toggle: "Approve" or "Reject / Request Changes"
  - Multiline text field for lecturer comments
  - Submit endorsement button

---

### 8. Opuni Frimpong Asante (22039152)

**Role:** Frontend Developer (Employer Portal & Badges)  
**Working Directory:** `src/VerifyGH.Client/Pages/`

- [ ] Implement `Search.razor`:
  - Search bar for keywords (project name, student name)
  - Filter by skill (e.g., C#, React, SQL, Machine Learning)
  - Filter by university / degree program
  - Candidate profile result cards with "Verified" checkmark
- [ ] Implement `Credentials.razor`:
  - Display verified badges and certificates
  - Shareable public link / QR code generator placeholder

---

### 9. Tenkorang Julius (22017966)

**Role:** API Integration  
**Working Directory:** `src/VerifyGH.Client/Services/`

- [ ] Create HTTP service classes in Blazor client:
  - `AuthService.cs` (Calls login/register endpoints, saves JWT in `ILocalStorageService`)
  - `ProjectService.cs` (Fetches projects, submits new projects)
  - `VerificationService.cs` (Fetches pending list, submits reviews)
- [ ] Wire up **SignalR** client:
  - Initialize `HubConnection` to `https://localhost:7296/hubs/notifications`
  - Listen for `ProjectStatusUpdated` events
  - Trigger MudBlazor `ISnackbar` alert when a project is reviewed in real-time

---

### 10. Osman Ilyas (22099559)

**Role:** Testing & QA  
**Working Directory:** `tests/` or API Testing Suite

- [ ] Write API testing collection (Postman or HTTP file `VerifyGH.http`) covering:
  - Registration & Login with different roles
  - Unauthorized access tests (ensuring students cannot access lecturer endpoints)
  - Project creation and verification flow
- [ ] Test frontend form validations (empty required fields, invalid email, short passwords).
- [ ] Verify SignalR real-time event delivery across two different browser sessions.
- [ ] Log and track bugs discovered during testing.

---

### 11. Eric Manu (22013835)

**Role:** Documentation  
**Working Directory:** `docs/`, `README.md`

- [ ] Prepare the DCIT 318 semester project report:
  - Problem statement and justification for Ghana's graduate market
  - System architecture diagram & ERD diagram
  - API endpoint catalog
  - Screenshots of all completed portals (Student, Lecturer, Employer)
- [ ] Prepare slide presentation deck for project defense.
- [ ] Keep `README.md` updated with setup prerequisites and step-by-step execution guide.

---

### 12. Edwine Nkum Boateng (22061303)

**Role:** DevOps & Deployment  
**Working Directory:** Root repository & CI/CD configs

- [ ] Establish Git branching strategy (`main`, `develop`, feature branches: `feature/auth`, `feature/projects`, `feature/verification`).
- [ ] Manage SQL Server connection environments and local development instructions.
- [ ] Test publishing production bundles (`dotnet publish -c Release`).
- [ ] Prepare deployment script or cloud hosting environment if required for final submission.
