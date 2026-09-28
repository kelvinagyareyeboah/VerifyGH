# VerifyGH — Verified Credential & Portfolio System

> **DCIT 318 - Semester Project Assignment 1**  
> **Category:** Web Applications & Services  
> **Institution:** University of Ghana  

---

## 📌 Project Brief
Ghana produces approximately **300,000 graduates every year**, but many face significant challenges in securing employment due to difficulties employers face in verifying authentic skills, capstone projects, and academic credentials.

**VerifyGH** is a verified credential and digital portfolio platform that empowers students and graduates to upload academic work, capstone projects, certificates, and validated skill evidence. University lecturers and academic supervisors review and officially endorse these submissions, enabling prospective employers to search, filter, and recruit from a trusted pool of verified Ghanaian talent.

---

## 🚀 Tech Stack

| Layer | Technologies |
|---|---|
| **Backend API** | ASP.NET Core Web API (.NET 10) |
| **Frontend Client** | Blazor WebAssembly (Standalone SPA) |
| **UI Component Suite** | MudBlazor (Material Design) |
| **Authentication & Security** | ASP.NET Identity, JWT Bearer, Role-Based Access Control (RBAC) |
| **Database & ORM** | Microsoft SQL Server Express, Entity Framework Core 10 |
| **Real-time Engine** | ASP.NET Core SignalR (WebSockets) |

---

## 👥 Team Members

| No. | Name | Student ID | Role |
|:---:|---|:---:|---|
| 1 | **Agyare Kelvin Yeboah** | 22159683 | Project Lead / Backend Architecture |
| 2 | **Ametefe Kwadwo Elijah** | 22040783 | UI/UX Design |
| 3 | **Boadu-Acheampong Asante Yaw** | 22152286 | Backend Developer |
| 4 | **Frank Bless Kofi Tsetse** | 22027295 | Frontend Developer |
| 5 | **Tieku Justice** | 22105235 | Frontend Developer |
| 6 | **Adjei David Boafo** | 22046873 | Database Design |
| 7 | **Anthony Gudu** | 22014087 | Authentication & Security |
| 8 | **Opuni Frimpong Asante** | 22039152 | Frontend Developer |
| 9 | **Osman Ilyas** | 22099559 | Testing & QA |
| 10 | **Tenkorang Julius** | 22017966 | API Integration |
| 11 | **Eric Manu** | 22013835 | Documentation |
| 12 | **Edwine Nkum Boateng** | 22061303 | DevOps & Deployment |

---

## 🏗️ Solution Structure

```text
VerifyGH/
├── VerifyGH.slnx
├── README.md
└── src/
    ├── VerifyGH.Shared/          # Shared DTOs and Enums (Client + Server)
    │   ├── DTOs/                 # AuthDTOs, ProjectDTOs
    │   └── Enums/                # UserRole, VerificationStatus
    │
    ├── VerifyGH.Server/          # ASP.NET Core Web API
    │   ├── Controllers/          # AuthController, ProjectsController, VerificationController
    │   ├── Data/                 # ApplicationDbContext, DbInitializer, Migrations/
    │   ├── Hubs/                 # SignalR NotificationHub
    │   ├── Models/               # ApplicationUser, Project, Skill, VerificationReview
    │   ├── Services/             # TokenService, ProjectService
    │   └── appsettings.json      # Connection string & JWT config
    │
    └── VerifyGH.Client/          # Blazor WebAssembly SPA
        ├── Layout/               # PublicLayout, MainLayout, NavMenu
        ├── Pages/                # Home, Login, Register, MyProjects, Verification, Search
        ├── Services/             # AuthService, ProjectService, VerificationService
        └── wwwroot/              # Static assets, appsettings.json
```

---

## ⚙️ Getting Started

### Prerequisites

| Tool | Version | Notes |
|---|---|---|
| [.NET SDK](https://dotnet.microsoft.com/) | 10.0+ | Required |
| SQL Server Express | Any recent | Must be running as `.\SQLEXPRESS` |
| `dotnet-ef` CLI tool | 10.x | See Step 2 |

> **Important:** This project uses **SQL Server Express** (`.\SQLEXPRESS`). If you have a different SQL Server instance, update the connection string in `src/VerifyGH.Server/appsettings.json` before running.

---

### Step 1 — Clone & Restore

```bash
git clone https://github.com/kelvinagyareyeboah/VerifyGH.git
cd VerifyGH
dotnet restore VerifyGH.slnx
```

---

### Step 2 — Install the EF Core CLI tool (once only)

```bash
dotnet tool install --global dotnet-ef
```

---

### Step 3 — Apply the Database Migration

> ⚠️ **Stop the server first if it is already running**, then run:

```bash
dotnet ef database update --project src/VerifyGH.Server --startup-project src/VerifyGH.Server
```

This creates the `VerifyGHDb` database on your SQL Server Express instance with all tables. It also seeds:
- Roles: `Student`, `Lecturer`, `Employer`, `Admin`
- A default admin account
- Sample users, skills, and projects for demo purposes

---

### Step 4 — Run the Backend API

Open a terminal and run:

```bash
dotnet watch --project src/VerifyGH.Server
```

| Endpoint | URL |
|---|---|
| API Base (HTTP) | `http://localhost:5019` |
| API Base (HTTPS) | `https://localhost:7296` |
| OpenAPI Docs | `http://localhost:5019/openapi/v1.json` |
| SignalR Hub | `http://localhost:5019/hubs/notifications` |

---

### Step 5 — Run the Frontend Client

Open a **second terminal** and run:

```bash
dotnet watch --project src/VerifyGH.Client
```

Then open your browser at:

```
http://localhost:5079
```

---

## 🔑 Demo Accounts

These accounts are seeded automatically when the server starts for the first time.

| Role | Email | Password |
|---|---|---|
| Student | `kelvin@st.ug.edu.gh` | `Student@123` |
| Student | `abena@st.ug.edu.gh` | `Student@123` |
| Lecturer | `msoli@ug.edu.gh` | `Lecturer@123` |
| Lecturer | `iwiafe@ug.edu.gh` | `Lecturer@123` |
| Employer | `talent@hubtel.com` | `Employer@123` |
| Admin | `admin@verifygh.edu.gh` | `Admin@123456` |

---

## 🌐 Portals

| Portal | Route | Access |
|---|---|---|
| Landing Page | `/` | Public |
| Sign In | `/login` | Public |
| Register | `/register` | Public |
| Student Dashboard | `/projects/my` | Student |
| Upload Project | `/projects/upload` | Student |
| Lecturer Review Queue | `/verification` | Lecturer |
| Employer Talent Search | `/search` | Employer |
| Credentials | `/credentials` | All |

---

## 🛠️ Troubleshooting

**`Failed to fetch` on login/register**
- Make sure the server is running (`dotnet watch --project src/VerifyGH.Server`)
- Check that `src/VerifyGH.Client/wwwroot/appsettings.json` has `"BackendUrl": "http://localhost:5019/"`

**`Login spins forever`**
- The database migration has not been applied. Run Step 3 above (stop the server first).

**`Build failed — file is locked`**
- `dotnet watch` is already running and locking the exe. Stop it with Ctrl+C, then re-run the command.

**`Cannot connect to SQL Server`**
- Confirm SQL Server Express is running: open **Services** and look for `SQL Server (SQLEXPRESS)` — it must say **Running**.
- If you have a different instance name, update the `DefaultConnection` string in `src/VerifyGH.Server/appsettings.json`.

---

## 📄 License
This project is for academic purposes — DCIT 318 Semester Project, University of Ghana.
