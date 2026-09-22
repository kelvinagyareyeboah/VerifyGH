# VerifyGH — Verified Credential & Portfolio System

> **DCIT 318 - Semester Project Assignment 1**  
> **Category:** Web Applications & Services  
> **Institution:** University of Ghana  

---

## 📌 Project Brief
Ghana produces approximately **300,000 graduates every year**, but many face significant challenges in securing employment due to difficulties employers face in verifying authentic skills, capstone projects, and academic credentials.

**VerifyGH** is a verified credential and digital portfolio platform that empowers students and graduates to upload academic work, capstone projects, certificates, and validated skill evidence. University lecturers and academic supervisors review and officially endorse these submissions, enabling prospective employers to search, filter, and recruit from a trusted pool of verified Ghanaian talent.

---

## 🚀 Frameworks and Technologies

| Layer | Technologies |
|---|---|
| **Backend API** | ASP.NET Core Web API (.NET 10) |
| **Frontend Client** | Blazor WebAssembly (Standalone SPA) |
| **UI Component Suite** | MudBlazor (Material Design) |
| **Authentication & Security** | ASP.NET Identity, JWT Bearer, Role-Based Access Control (RBAC) |
| **Database & ORM** | Microsoft SQL Server, Entity Framework Core 10 |
| **Real-time Engine** | Microsoft ASP.NET Core SignalR (WebSockets) |

---

## 🎯 Target Users & Core Features

### Target Users
* **Students and Graduates:** Upload academic projects, credentials, and portfolios; request supervisor verification; track real-time validation status.
* **Lecturers and Academic Supervisors:** Manage submission verification queues, review project deliverables and code repositories, provide academic feedback, and approve/reject credentials.
* **Employers:** Discover verified graduate profiles, filter by vetted skills, institution, and ratings, and view authentic proof-of-work.
* **System Administrators:** Oversee institutions, departments, and audit logs.

### Core Features
- [x] **Verified Portfolio Submissions:** Upload capstone projects, documentation, code links, and skill evidence.
- [x] **Lecturer Approval Workflow:** Endorse or request revisions on student deliverables with feedback trails.
- [x] **Employer Talent Search:** Dynamic filtering by accredited skills, degree programs, and institutions.
- [x] **Real-time Status Updates:** SignalR push notifications for submission reviews, status changes, and approvals.
- [x] **Role-Based Access Control:** Secure JWT authentication with partitioned roles (`Student`, `Lecturer`, `Employer`, `Admin`).

---

## 👥 Team Members & Role Distribution

| No. | Name | Student ID | Role |
|:---:|---|:---:|---|
| 1 | **Agyare Kelvin Yeboah** | 22159683 | **Project Lead / Backend Architecture** |
| 2 | **Ametefe Kwadwo Elijah** | 22040783 | **UI/UX Design** |
| 3 | **Boadu-Acheampong Asante Yaw** | 22152286 | **Backend Developer** |
| 4 | **Frank Bless Kofi Tsetse** | 22027295 | **Frontend Developer** |
| 5 | **Tieku Justice** | 22105235 | **Frontend Developer** |
| 6 | **Adjei David Boafo** | 22046873 | **Database Design** |
| 7 | **Anthony Gudu** | 22014087 | **Authentication & Security** |
| 8 | **Opuni Frimpong Asante** | 22039152 | **Frontend Developer** |
| 9 | **Osman Ilyas** | 22099559 | **Testing & QA** |
| 10 | **Tenkorang Julius** | 22017966 | **API Integration** |
| 11 | **Eric Manu** | 22013835 | **Documentation** |
| 12 | **Edwine Nkum Boateng** | 22061303 | **DevOps & Deployment** |

---

## 🏗️ Solution Architecture

```text
VerifyGH/
├── VerifyGH.slnx                   # Solution file
├── .gitignore                      # .NET Git ignore
├── README.md                       # Project documentation
│
├── src/
│   ├── VerifyGH.Shared/            # Shared Library (Client & Server)
│   │   ├── Enums/                  # UserRole, VerificationStatus
│   │   └── DTOs/                   # AuthDTOs, ProjectDTOs, ProfileDTOs
│   │
│   ├── VerifyGH.Server/            # ASP.NET Core Web API (Backend)
│   │   ├── Controllers/            # REST API Controllers
│   │   ├── Data/                   # ApplicationDbContext & Migrations
│   │   ├── Hubs/                   # SignalR NotificationHub
│   │   ├── Models/                 # ApplicationUser (Identity)
│   │   └── appsettings.json        # Database & JWT configurations
│   │
│   └── VerifyGH.Client/            # Blazor WebAssembly (Frontend)
│       ├── Layout/                 # MainLayout, MudNavMenu, Themes
│       ├── Pages/                  # Dashboard, Upload, Verification, Search
│       ├── wwwroot/                # MudBlazor CSS, Fonts, Assets
│       └── Program.cs              # MudBlazor & HttpClient configuration
```

---

## ⚙️ Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/) (LocalDB, Express, or Developer)

### 1. Clone & Restore
```bash
git clone https://github.com/kelvinagyareyeboah/VerifyGH.git
cd VerifyGH
dotnet restore VerifyGH.slnx
```

### 2. Run the Backend API (`VerifyGH.Server`)
```bash
dotnet run --project src/VerifyGH.Server
```
* **API Base:** `https://localhost:7296` (or `http://localhost:5019`)
* **OpenAPI Docs:** `https://localhost:7296/openapi/v1.json`
* **SignalR Hub:** `https://localhost:7296/hubs/notifications`

### 3. Run the Frontend (`VerifyGH.Client`)
In a separate terminal:
```bash
dotnet run --project src/VerifyGH.Client
```
* **Web App URL:** `https://localhost:7270` (or `http://localhost:5079`)

---

## 📄 License
This project is licensed under the terms described in the [LICENSE](LICENSE) file.
