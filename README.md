<div align="center">
  <p>
    <a href="#türkçe">🇹🇷 Türkçe</a> | <a href="#english">🇬🇧 English</a>
  </p>
</div>

---

<a id="türkçe"></a>

<div align="center">

# OsbErpLite

### Organize Sanayi Bölgesi yönetimi için Clean Architecture + CQRS tabanlı hafif ERP sistemi

[Mimari](#tr-mimari) •
[Modüller](#tr-modüller) •
[Kurulum](#tr-kurulum) •
[Güvenlik](#tr-güvenlik)

<br />

<img src="https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
<img src="https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core" />
<img src="https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="Entity Framework Core" />
<img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />

<br />

<img src="https://img.shields.io/badge/MediatR-FF6F00?style=for-the-badge&logoColor=white" alt="MediatR" />
<img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
<img src="https://img.shields.io/badge/Serilog-2E86C1?style=for-the-badge&logoColor=white" alt="Serilog" />
<img src="https://img.shields.io/badge/SAP_OData-0FAAFF?style=for-the-badge&logo=sap&logoColor=white" alt="SAP OData" />

<br />

<p>
  <img src="https://img.shields.io/badge/Architecture-Clean_Architecture-F59E0B?style=flat-square" alt="Clean Architecture" />
  <img src="https://img.shields.io/badge/Pattern-CQRS-2563EB?style=flat-square" alt="CQRS" />
  <img src="https://img.shields.io/badge/Authentication-JWT-111827?style=flat-square" alt="JWT Authentication" />
  <img src="https://img.shields.io/badge/Tests-118_Passing-16A34A?style=flat-square" alt="118 Tests" />
  <img src="https://img.shields.io/badge/API-REST-2563EB?style=flat-square" alt="REST API" />
</p>

</div>

---

## 📖 Proje Hakkında

**OsbErpLite**, Organize Sanayi Bölgesi (OSB) yönetimi için geliştirilen hafif bir ERP sistemidir. Temel iş süreçlerini tek bir platformda toplamayı hedefler.

Proje; **.NET 8**, **ASP.NET Core Web API**, **Entity Framework Core** ve **PostgreSQL** ile **Clean Architecture** ve **CQRS** (MediatR) pattern'leri kullanılarak geliştirilmiştir. SAP S/4HANA Cloud ile OData üzerinden iş ortağı senkronizasyonu desteklenmektedir.

---

<a id="tr-modüller"></a>

## 🧭 Modüller

### Business Partners (İş Ortakları)

- Müşteri, tedarikçi ve çift yönlü iş ortağı yönetimi
- SAP S/4HANA Cloud'dan OData ile senkronizasyon (upsert)
- Soft delete ile cascade kontrol

### Products & Stock (Ürün ve Stok)

- Ürün tanımları ve SKU yönetimi
- Depo (Warehouse) yönetimi
- Manuel stok hareketi girişi (In/Out)
- Sipariş tamamlamada otomatik stok hareketi

### Purchase Orders (Satın Alma)

- Sipariş oluşturma (Draft status)
- Onay akışı: Draft → Approved → Completed / Rejected
- Tamamlamada otomatik stock-in hareketi

### Sales Orders (Satış)

- Sipariş oluşturma (Draft status)
- Onay akışı: Draft → Approved → Completed / Rejected
- Tamamlamada otomatik stock-out hareketi

### Human Resources (İnsan Kaynakları)

- Departman ve pozisyon yönetimi
- Çalışan yönetimi (TC Kimlik No algoritma validasyonu)
- İzin talebi onay akışı: Pending → Approved / Rejected
- Soft delete cascade kontrol (bağlı çalışan/izin varsa engelleme)

### Dashboard (İstatistikler)

- Genel özet: aktif çalışan, iş ortağı, bekleyen sipariş/izin sayıları
- Düşük stoklu ürün sayısı (konfigüre edilebilir eşik)
- Toplam stok değeri (sadece Admin/Manager)
- Aylık stok hareket istatistikleri (son 6 ay, grafik için hazır)
- Departman bazlı çalışan dağılımı (pasta grafik için hazır)

### Authentication & Authorization

- JWT Bearer authentication (access token 15dk + refresh token 7gün)
- Refresh token rotation ve revoke
- Role-based authorization (Admin, Manager, Employee)
- Kullanıcı yönetimi (CRUD, şifre sıfırlama)

---

## 🛠 Kullanılan Teknolojiler

| Alan | Teknoloji | Versiyon |
| --- | --- | --- |
| Runtime | .NET | 8.0 |
| API | ASP.NET Core Web API | 8.0 |
| ORM | Entity Framework Core | 8.x |
| Veritabanı | PostgreSQL (Npgsql) | 8.x |
| CQRS / Mediator | MediatR | 14.x |
| Validation | FluentValidation | 12.x |
| Mapping | AutoMapper | 16.x |
| Authentication | JWT Bearer | 8.x |
| Logging | Serilog (File Sink) | 10.x |
| Container | Docker & Docker Compose | - |
| API Dokümantasyonu | Swashbuckle (Swagger) | 6.x |
| SAP Entegrasyonu | OData REST Client | - |

---

<a id="tr-mimari"></a>

## 🏗 Mimari

Clean Architecture ve CQRS pattern'leri ile dört katmanlı yapı kullanılmaktadır.

### Katman Bağımlılık Yönü

```
Api → Application → Domain
Infrastructure → Application → Domain
```

> **Domain** katmanı hiçbir katmana ve hiçbir NuGet paketine (EF Core dahil) bağımlı değildir. Bu kural kesindir.

| Katman | Sorumluluk |
| --- | --- |
| `OsbErpLite.Domain` | Entities, Value Objects, Enums, Domain Events |
| `OsbErpLite.Application` | CQRS (Commands/Queries), MediatR handler'lar, DTO'lar, Validator'lar, Repository interface'leri |
| `OsbErpLite.Infrastructure` | EF Core DbContext, Repository implementasyonları, SAP OData client |
| `OsbErpLite.Api` | Controller'lar, JWT auth, middleware, Swagger, Program.cs |

### Request Akışı

```
Client
  ↓
ASP.NET Core Controller
  ↓
MediatR Pipeline (Validation Behavior)
  ↓
Command/Query Handler
  ↓
Repository (EF Core)
  ↓
PostgreSQL
```

### Proje Yapısı

```
OsbErpLite/
├── OsbErpLite.sln
├── global.json
├── docker-compose.yml
├── .env.example
├── OsbErpLite.Api/
│   ├── Controllers/
│   ├── Extensions/
│   ├── Middleware/
│   ├── Common/
│   ├── Services/
│   └── Program.cs
├── OsbErpLite.Application/
│   ├── Common/
│   │   ├── Behaviors/
│   │   └── Interfaces/
│   ├── BusinessPartners/
│   ├── Products/
│   ├── Warehouses/
│   ├── StockMovements/
│   ├── PurchaseOrders/
│   ├── SalesOrders/
│   ├── Identity/
│   └── HumanResources/
│       ├── Departments/
│       ├── Positions/
│       ├── Employees/
│       └── LeaveRequests/
├── OsbErpLite.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Common/
├── OsbErpLite.Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── Repositories/
│   └── ExternalServices/
│       └── Sap/
└── tests/
    ├── OsbErpLite.Domain.Tests/
    └── OsbErpLite.Application.Tests/
```

---

## 💡 Neden Clean Architecture + CQRS?

Bu projeyi geliştirirken bilinçli olarak Clean Architecture ve CQRS pattern'lerini tercih ettim. Nedenleri şunlar:

**Clean Architecture** ile katmanlar arasında net bir bağımlılık yönü oluşturdum. Domain katmanı hiçbir dış kütüphaneye bağımlı değil — bu sayede iş kuralları framework'ten bağımsız yaşıyor. Yarın EF Core yerine Dapper kullansam, ya da PostgreSQL yerine başka bir veritabanına geçsem, Domain ve Application katmanlarına dokunmam gerekmiyor.

**CQRS (MediatR)** ile her iş parçasını kendi Command/Query handler'ına izole ettim. Bu yaklaşımın somut faydaları:
- Her handler tek bir iş yapıyor (Single Responsibility)
- Controller'lar sadece request'i MediatR'a yönlendiriyor, business logic içermiyor
- FluentValidation'ı MediatR pipeline behavior olarak ekleyebildim — validation logic handler'dan ayrı yaşıyor
- Yeni bir özellik eklemek = yeni bir handler eklemek, mevcut koda dokunmadan (Open/Closed)

**Alternatif olarak neden N-Tier veya basit Service pattern kullanmadım?**

N-Tier yapıda service sınıfları zamanla şişiyor — `BusinessPartnerService` içinde 15+ method, hepsi aynı yerde. CQRS ile `CreateBusinessPartnerHandler`, `GetAllBusinessPartnersHandler` gibi küçük, odaklanmış sınıflar var. Test yazmak çok daha kolay çünkü her handler'ın bağımlılıkları minimal ve net.

Bu mimari, özellikle modül sayısı arttıkça (şu an 5 modül) ve her modülde onay akışları gibi karmaşık iş kuralları olduğunda kendini gösteriyor. Sipariş onay akışı (Draft → Approved → Completed) ve izin talebi akışı gibi state machine'ler her biri kendi handler'ında temiz bir şekilde yaşıyor.

---

<a id="tr-güvenlik"></a>

## 🔐 Güvenlik

### Authentication ve Authorization

- JWT Bearer authentication
- Access token (15 dakika) + Refresh token (7 gün) ile token rotation
- Role-based authorization: Admin, Manager, Employee
- Refresh token revoke (logout)

### API Güvenliği

- Global exception handling middleware
- FluentValidation ile merkezi validation
- Rate limiting (auth ve SAP sync endpoint'leri)
- CORS konfigürasyonu
- Standart `ApiResponse<T>` response contract
- Swagger yalnızca Development ortamında

### Secret Yönetimi

Gerçek değerler repository içine yazılmaz.

- Tüm secret'lar `.env` dosyasından okunur
- `.env` her zaman `.gitignore`'da tutulur
- Repository yalnızca `.env.example` dosyasını takip eder

---

## 🔌 API Özeti

### Authentication

```
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token
POST /api/auth/logout
GET  /api/auth/users
GET  /api/auth/users/{id}
PUT  /api/auth/users/{id}
DELETE /api/auth/users/{id}
PUT  /api/auth/users/{id}/reset-password
PUT  /api/auth/me/change-password
```

### Business Partners

```
POST   /api/businesspartners
GET    /api/businesspartners
GET    /api/businesspartners/{id}
PUT    /api/businesspartners/{id}
DELETE /api/businesspartners/{id}
```

### Products

```
POST   /api/products
GET    /api/products
GET    /api/products/{id}
PUT    /api/products/{id}
DELETE /api/products/{id}
```

### Warehouses

```
POST   /api/warehouses
GET    /api/warehouses
GET    /api/warehouses/{id}
PUT    /api/warehouses/{id}
DELETE /api/warehouses/{id}
```

### Stock Movements

```
POST /api/stockmovements
GET  /api/stockmovements/product/{productId}
```

### Purchase Orders

```
POST /api/purchaseorders
GET  /api/purchaseorders
GET  /api/purchaseorders/{id}
PUT  /api/purchaseorders/{id}/approve
PUT  /api/purchaseorders/{id}/reject
PUT  /api/purchaseorders/{id}/complete
```

### Sales Orders

```
POST /api/salesorders
GET  /api/salesorders
GET  /api/salesorders/{id}
PUT  /api/salesorders/{id}/approve
PUT  /api/salesorders/{id}/reject
PUT  /api/salesorders/{id}/complete
```

### Human Resources

```
POST   /api/departments
GET    /api/departments
GET    /api/departments/{id}
PUT    /api/departments/{id}
DELETE /api/departments/{id}

POST   /api/positions
GET    /api/positions
GET    /api/positions/{id}
PUT    /api/positions/{id}
DELETE /api/positions/{id}

POST   /api/employees
GET    /api/employees
GET    /api/employees/{id}
PUT    /api/employees/{id}
DELETE /api/employees/{id}

POST /api/leaverequests
GET  /api/leaverequests
GET  /api/leaverequests/{id}
PUT  /api/leaverequests/{id}/approve
PUT  /api/leaverequests/{id}/reject
```

### Dashboard

```
GET /api/dashboard/summary                  [Authorize]
GET /api/dashboard/stock-movements          [Authorize(Roles = "Admin,Manager")]
GET /api/dashboard/department-distribution  [Authorize]
```

### SAP Integration

```
POST /api/sap/sync-business-partners
```

### Health Check

```
GET /health
```

### Standart Response

```json
{
  "success": true,
  "message": null,
  "data": {}
}
```

Validation, authorization, not-found ve exception response'ları da aynı contract üzerinden döner.

---

<a id="tr-kurulum"></a>

## 🚀 Kurulum

### Gereksinimler

- .NET 8 SDK
- PostgreSQL
- Entity Framework Core CLI
- Docker Desktop (opsiyonel)

EF Core CLI kurulumu:

```bash
dotnet tool install --global dotnet-ef
```

### Repository'yi Klonlama

```bash
git clone https://github.com/Deniz-Kmr/OsbErpLite.git
cd OsbErpLite
dotnet restore
```

### Environment Ayarları

```bash
cp .env.example .env
```

`.env` dosyasındaki placeholder değerleri kendi değerlerinizle değiştirin:

```
POSTGRES_DB=your_db_name
POSTGRES_USER=your_username
POSTGRES_PASSWORD=your_password
CONNECTION_STRING=Host=localhost;Port=5433;Database=your_db_name;Username=your_username;Password=your_password
JWT_SECRET_KEY=your_secret_key_here
SAP_API_BASE_URL=https://sandbox.api.sap.com/s4hanacloud/sap/opu/odata/sap/API_BUSINESS_PARTNER/
SAP_API_KEY=your_sap_api_key_here
```

### Docker Compose ile Çalıştırma

```bash
docker compose up -d --build
```

Durum ve log kontrolü:

```bash
docker compose ps
docker compose logs api
docker compose logs postgres
```

Local adresler:

```
API:     http://localhost:5019
Swagger: http://localhost:5019/swagger
Health:  http://localhost:5019/health
```

Containerları durdurma:

```bash
docker compose down
```

### Local Geliştirme (Docker Olmadan)

PostgreSQL'i ayrı çalıştırın, ardından:

```bash
dotnet ef database update --project OsbErpLite.Infrastructure --startup-project OsbErpLite.Api
```

```bash
dotnet run --project OsbErpLite.Api
```

### Migration Oluşturma

```bash
dotnet ef migrations add MigrationName --project OsbErpLite.Infrastructure --startup-project OsbErpLite.Api
```

---

## 🧪 Testler

Proje Domain ve Application katmanları için 118 unit test içermektedir.

```bash
dotnet test
```

Test kapsamı:

- Entity business logic
- TC Kimlik No validasyonu
- CQRS handler'lar
- SAP sync upsert testleri (mock ile)

---

## 📊 API Dokümantasyonu

Swagger UI, Development ortamında otomatik olarak açılır:

```
http://localhost:5019/swagger
```

Swagger UI üzerinden:

- Tüm endpoint'lerin açıklamaları ve parametre detayları görüntülenebilir
- **Authorize** butonu ile JWT token girilerek korumalı endpoint'ler test edilebilir
- Request/response model şemaları incelenebilir

---

## 🌿 Git Akışı

```
main
develop
feature/*
fix/*
```

Kurallar:

- `main` branch'ine doğrudan push yapılmaz, stabil kod için korunur
- Günlük geliştirme `develop` üzerinden ilerler
- Her yeni iş için `feature/xxx` branch'i açılır, iş bitince `develop`'a merge edilir
- Commit formatı: Conventional Commits (`feat:`, `fix:`, `refactor:`, `docs:`, `chore:`)
- Küçük ve anlamlı commitler kullanılır

---

## 📌 Güncel Durum

Tamamlanan başlıklar:

- Clean Architecture + CQRS altyapısı
- PostgreSQL domain, DbContext ve migration
- JWT authentication (access + refresh token rotation)
- Role-based authorization
- Tüm modüller için CRUD endpoint'leri
- Sipariş onay akışı (Draft → Approved → Completed / Rejected)
- İzin talebi onay akışı (Pending → Approved / Rejected)
- Otomatik stok hareketi (sipariş tamamlamada)
- Soft delete ve cascade kontrol
- TC Kimlik No algoritma validasyonu
- SAP S/4HANA Cloud OData entegrasyonu
- Global exception handling middleware
- FluentValidation pipeline behavior
- Rate limiting
- CORS konfigürasyonu
- Health check endpoint
- Dashboard istatistik endpoint'leri (özet, stok hareket, departman dağılımı)
- Serilog ile dosya loglama
- Docker Compose (PostgreSQL + API)
- Swagger/OpenAPI dokümantasyonu (XML comments, JWT auth)
- Unit ve integration testler (130 test)
- CreatedBy/UpdatedBy audit alanları

---

<h2 id="english">🇬🇧 English</h2>

<div align="center">

# OsbErpLite

### Lightweight ERP system for Organized Industrial Zone management, built with Clean Architecture + CQRS

[Architecture](#en-architecture) •
[Modules](#en-modules) •
[Getting Started](#en-getting-started) •
[Security](#en-security)

<br />

<img src="https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
<img src="https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core" />
<img src="https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="Entity Framework Core" />
<img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />

<br />

<img src="https://img.shields.io/badge/MediatR-FF6F00?style=for-the-badge&logoColor=white" alt="MediatR" />
<img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
<img src="https://img.shields.io/badge/Serilog-2E86C1?style=for-the-badge&logoColor=white" alt="Serilog" />
<img src="https://img.shields.io/badge/SAP_OData-0FAAFF?style=for-the-badge&logo=sap&logoColor=white" alt="SAP OData" />

<br />

<p>
  <img src="https://img.shields.io/badge/Architecture-Clean_Architecture-F59E0B?style=flat-square" alt="Clean Architecture" />
  <img src="https://img.shields.io/badge/Pattern-CQRS-2563EB?style=flat-square" alt="CQRS" />
  <img src="https://img.shields.io/badge/Authentication-JWT-111827?style=flat-square" alt="JWT Authentication" />
  <img src="https://img.shields.io/badge/Tests-118_Passing-16A34A?style=flat-square" alt="118 Tests" />
  <img src="https://img.shields.io/badge/API-REST-2563EB?style=flat-square" alt="REST API" />
</p>

</div>

---

## 📖 About the Project

**OsbErpLite** is a lightweight ERP system designed for Organized Industrial Zone (OSB) management. It aims to consolidate core business processes into a single platform.

The project is built with **.NET 8**, **ASP.NET Core Web API**, **Entity Framework Core**, and **PostgreSQL** using **Clean Architecture** and **CQRS** (MediatR) patterns. It supports business partner synchronization from SAP S/4HANA Cloud via OData.

---

<a id="en-modules"></a>

## 🧭 Modules

### Business Partners

- Customer, supplier, and dual-role partner management
- SAP S/4HANA Cloud synchronization via OData (upsert)
- Soft delete with cascade validation

### Products & Stock

- Product definitions and SKU management
- Warehouse management
- Manual stock movements (In/Out)
- Automatic stock movements on order completion

### Purchase Orders

- Order creation (Draft status)
- Approval workflow: Draft → Approved → Completed / Rejected
- Automatic stock-in on completion

### Sales Orders

- Order creation (Draft status)
- Approval workflow: Draft → Approved → Completed / Rejected
- Automatic stock-out on completion

### Human Resources

- Department and position management
- Employee management (Turkish National ID algorithm validation)
- Leave request approval workflow: Pending → Approved / Rejected
- Soft delete cascade validation (blocks if linked employees/requests exist)

### Dashboard (Statistics)

- Summary: active employees, business partners, pending orders/leave requests
- Low stock product count (configurable threshold)
- Total stock value (Admin/Manager only)
- Monthly stock movement statistics (last 6 months, chart-ready)
- Department employee distribution (pie chart-ready)

### Authentication & Authorization

- JWT Bearer authentication (access token 15min + refresh token 7days)
- Refresh token rotation and revoke
- Role-based authorization (Admin, Manager, Employee)
- User management (CRUD, password reset)

---

## 🛠 Technology Stack

| Area | Technology | Version |
| --- | --- | --- |
| Runtime | .NET | 8.0 |
| API | ASP.NET Core Web API | 8.0 |
| ORM | Entity Framework Core | 8.x |
| Database | PostgreSQL (Npgsql) | 8.x |
| CQRS / Mediator | MediatR | 14.x |
| Validation | FluentValidation | 12.x |
| Mapping | AutoMapper | 16.x |
| Authentication | JWT Bearer | 8.x |
| Logging | Serilog (File Sink) | 10.x |
| Containerization | Docker & Docker Compose | - |
| API Documentation | Swashbuckle (Swagger) | 6.x |
| SAP Integration | OData REST Client | - |

---

<a id="en-architecture"></a>

## 🏗 Architecture

The project follows Clean Architecture with CQRS pattern across four layers.

### Layer Dependency Direction

```
Api → Application → Domain
Infrastructure → Application → Domain
```

> The **Domain** layer has zero dependencies on any other layer or NuGet package (including EF Core). This rule is absolute.

| Layer | Responsibility |
| --- | --- |
| `OsbErpLite.Domain` | Entities, Value Objects, Enums, Domain Events |
| `OsbErpLite.Application` | CQRS (Commands/Queries), MediatR handlers, DTOs, Validators, Repository interfaces |
| `OsbErpLite.Infrastructure` | EF Core DbContext, Repository implementations, SAP OData client |
| `OsbErpLite.Api` | Controllers, JWT auth, middleware, Swagger, Program.cs |

### Request Flow

```
Client
  ↓
ASP.NET Core Controller
  ↓
MediatR Pipeline (Validation Behavior)
  ↓
Command/Query Handler
  ↓
Repository (EF Core)
  ↓
PostgreSQL
```

### Project Structure

```
OsbErpLite/
├── OsbErpLite.sln
├── global.json
├── docker-compose.yml
├── .env.example
├── OsbErpLite.Api/
│   ├── Controllers/
│   ├── Extensions/
│   ├── Middleware/
│   ├── Common/
│   ├── Services/
│   └── Program.cs
├── OsbErpLite.Application/
│   ├── Common/
│   │   ├── Behaviors/
│   │   └── Interfaces/
│   ├── BusinessPartners/
│   ├── Products/
│   ├── Warehouses/
│   ├── StockMovements/
│   ├── PurchaseOrders/
│   ├── SalesOrders/
│   ├── Identity/
│   └── HumanResources/
│       ├── Departments/
│       ├── Positions/
│       ├── Employees/
│       └── LeaveRequests/
├── OsbErpLite.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Common/
├── OsbErpLite.Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── Repositories/
│   └── ExternalServices/
│       └── Sap/
└── tests/
    ├── OsbErpLite.Domain.Tests/
    └── OsbErpLite.Application.Tests/
```

---

## 💡 Why Clean Architecture + CQRS?

I deliberately chose Clean Architecture and CQRS patterns for this project. Here's why:

**Clean Architecture** establishes a clear dependency direction between layers. The Domain layer has zero dependencies on any external library — business rules live independently of the framework. If I were to swap EF Core for Dapper tomorrow, or migrate to a different database, the Domain and Application layers would remain untouched.

**CQRS (MediatR)** isolates each piece of work into its own Command/Query handler. The concrete benefits:
- Each handler does exactly one thing (Single Responsibility)
- Controllers only route requests to MediatR — no business logic leaks in
- FluentValidation plugs into the MediatR pipeline as a behavior — validation logic lives separately from handlers
- Adding a new feature means adding a new handler without modifying existing code (Open/Closed)

**Why not N-Tier or a simple Service pattern?**

In N-Tier, service classes tend to bloat over time — a `BusinessPartnerService` with 15+ methods, all in one place. With CQRS, there are small, focused classes like `CreateBusinessPartnerHandler` and `GetAllBusinessPartnersHandler`. Testing becomes much easier because each handler's dependencies are minimal and explicit.

This architecture particularly shines as the module count grows (currently 5 modules) and each module contains complex business rules like approval workflows. State machines such as the order approval flow (Draft → Approved → Completed) and the leave request flow each live cleanly in their own handler.

---

<a id="en-security"></a>

## 🔐 Security

### Authentication and Authorization

- JWT Bearer authentication
- Access token (15 minutes) + Refresh token (7 days) with token rotation
- Role-based authorization: Admin, Manager, Employee
- Refresh token revoke (logout)

### API Security

- Global exception handling middleware
- Centralized validation via FluentValidation
- Rate limiting (auth and SAP sync endpoints)
- CORS configuration
- Standard `ApiResponse<T>` response contract
- Swagger restricted to Development environment

### Secret Management

Real credentials must never be committed.

- All secrets are read from `.env` file
- `.env` is always kept in `.gitignore`
- The repository tracks `.env.example` only

---

## 🔌 API Overview

### Authentication

```
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token
POST /api/auth/logout
GET  /api/auth/users
GET  /api/auth/users/{id}
PUT  /api/auth/users/{id}
DELETE /api/auth/users/{id}
PUT  /api/auth/users/{id}/reset-password
PUT  /api/auth/me/change-password
```

### Business Partners

```
POST   /api/businesspartners
GET    /api/businesspartners
GET    /api/businesspartners/{id}
PUT    /api/businesspartners/{id}
DELETE /api/businesspartners/{id}
```

### Products

```
POST   /api/products
GET    /api/products
GET    /api/products/{id}
PUT    /api/products/{id}
DELETE /api/products/{id}
```

### Warehouses

```
POST   /api/warehouses
GET    /api/warehouses
GET    /api/warehouses/{id}
PUT    /api/warehouses/{id}
DELETE /api/warehouses/{id}
```

### Stock Movements

```
POST /api/stockmovements
GET  /api/stockmovements/product/{productId}
```

### Purchase Orders

```
POST /api/purchaseorders
GET  /api/purchaseorders
GET  /api/purchaseorders/{id}
PUT  /api/purchaseorders/{id}/approve
PUT  /api/purchaseorders/{id}/reject
PUT  /api/purchaseorders/{id}/complete
```

### Sales Orders

```
POST /api/salesorders
GET  /api/salesorders
GET  /api/salesorders/{id}
PUT  /api/salesorders/{id}/approve
PUT  /api/salesorders/{id}/reject
PUT  /api/salesorders/{id}/complete
```

### Human Resources

```
POST   /api/departments
GET    /api/departments
GET    /api/departments/{id}
PUT    /api/departments/{id}
DELETE /api/departments/{id}

POST   /api/positions
GET    /api/positions
GET    /api/positions/{id}
PUT    /api/positions/{id}
DELETE /api/positions/{id}

POST   /api/employees
GET    /api/employees
GET    /api/employees/{id}
PUT    /api/employees/{id}
DELETE /api/employees/{id}

POST /api/leaverequests
GET  /api/leaverequests
GET  /api/leaverequests/{id}
PUT  /api/leaverequests/{id}/approve
PUT  /api/leaverequests/{id}/reject
```

### Dashboard

```
GET /api/dashboard/summary                  [Authorize]
GET /api/dashboard/stock-movements          [Authorize(Roles = "Admin,Manager")]
GET /api/dashboard/department-distribution  [Authorize]
```

### SAP Integration

```
POST /api/sap/sync-business-partners
```

### Health Check

```
GET /health
```

### Standard Response

```json
{
  "success": true,
  "message": null,
  "data": {}
}
```

Validation, authorization, not-found, and exception responses follow the same contract.

---

<a id="en-getting-started"></a>

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL
- Entity Framework Core CLI
- Docker Desktop (optional)

Install the EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

### Clone the Repository

```bash
git clone https://github.com/Deniz-Kmr/OsbErpLite.git
cd OsbErpLite
dotnet restore
```

### Environment Configuration

```bash
cp .env.example .env
```

Replace the placeholder values in `.env` with your own:

```
POSTGRES_DB=your_db_name
POSTGRES_USER=your_username
POSTGRES_PASSWORD=your_password
CONNECTION_STRING=Host=localhost;Port=5433;Database=your_db_name;Username=your_username;Password=your_password
JWT_SECRET_KEY=your_secret_key_here
SAP_API_BASE_URL=https://sandbox.api.sap.com/s4hanacloud/sap/opu/odata/sap/API_BUSINESS_PARTNER/
SAP_API_KEY=your_sap_api_key_here
```

### Running with Docker Compose

```bash
docker compose up -d --build
```

Check status and logs:

```bash
docker compose ps
docker compose logs api
docker compose logs postgres
```

Local addresses:

```
API:     http://localhost:5019
Swagger: http://localhost:5019/swagger
Health:  http://localhost:5019/health
```

Stop containers:

```bash
docker compose down
```

### Local Development (Without Docker)

Start PostgreSQL separately, then:

```bash
dotnet ef database update --project OsbErpLite.Infrastructure --startup-project OsbErpLite.Api
```

```bash
dotnet run --project OsbErpLite.Api
```

### Creating Migrations

```bash
dotnet ef migrations add MigrationName --project OsbErpLite.Infrastructure --startup-project OsbErpLite.Api
```

---

## 🧪 Tests

The project contains 118 unit tests covering Domain and Application layers.

```bash
dotnet test
```

Test coverage includes:

- Entity business logic
- Turkish National ID validation
- CQRS handlers
- SAP sync upsert tests (with mocks)

---

## 📊 API Documentation

Swagger UI is automatically available in the Development environment:

```
http://localhost:5019/swagger
```

Through Swagger UI you can:

- Browse endpoint descriptions and parameter details
- Test protected endpoints using the **Authorize** button with a JWT token
- Inspect request/response model schemas

---

## 🌿 Git Workflow

```
main
develop
feature/*
fix/*
```

Rules:

- Do not push directly to `main`; it is reserved for stable releases
- Daily development happens on `develop`
- Each task gets a `feature/xxx` branch, merged into `develop` when complete
- Commit format: Conventional Commits (`feat:`, `fix:`, `refactor:`, `docs:`, `chore:`)
- Use small, meaningful commits

---

## 📌 Current Status

Completed milestones:

- Clean Architecture + CQRS foundation
- PostgreSQL domain, DbContext, and migrations
- JWT authentication (access + refresh token rotation)
- Role-based authorization
- Full CRUD endpoints for all modules
- Order approval workflow (Draft → Approved → Completed / Rejected)
- Leave request approval workflow (Pending → Approved / Rejected)
- Automatic stock movements on order completion
- Soft delete with cascade validation
- Turkish National ID algorithm validation
- SAP S/4HANA Cloud OData integration
- Global exception handling middleware
- FluentValidation pipeline behavior
- Rate limiting
- CORS configuration
- Health check endpoint
- Dashboard statistics endpoints (summary, stock movements, department distribution)
- Serilog file logging
- Docker Compose (PostgreSQL + API)
- Swagger/OpenAPI documentation (XML comments, JWT auth)
- Unit and integration tests (130 tests)
- CreatedBy/UpdatedBy audit fields
