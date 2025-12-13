# Project Context: BMS_project

## Overview
**BMS_project** is an ASP.NET Core MVC web application designed for managing Barangay and Sangguniang Kabataan (SK) operations. It facilitates the management of youth profiles, projects, budgets, compliance documents, and announcements.

**Key Technologies:**
*   **Framework:** .NET 9.0 (ASP.NET Core MVC)
*   **Database:** MySQL 8.0+
*   **ORM:** Entity Framework Core 9.0
*   **Authentication:** Cookie-based Authentication
*   **Email:** MailKit (configured for Mailtrap in dev)
*   **Documentation:** Swagger/OpenAPI

## Architecture & Structure
The project follows a standard Model-View-Controller (MVC) architectural pattern.

### Key Directories
*   `BMS_project/Controllers/`: Handles incoming HTTP requests. Organized by functional areas (e.g., `SuperAdminController`, `BarangaySKContoller`, `FederationPResidentController`).
*   `BMS_project/Models/`: Domain entities representing the database structure (e.g., `User`, `Barangay`, `Project`, `Budget`).
*   `BMS_project/ViewModels/`: Models specifically designed for View rendering and form handling (e.g., `ProjectCreationViewModel`, `UserFormViewModel`).
*   `BMS_project/Views/`: Razor views (`.cshtml`) for the UI.
*   `BMS_project/Data/`: Contains `ApplicationDbContext` for Entity Framework Core configuration.
*   `BMS_project/Services/`: Business logic and external service integrations (e.g., `EmailService`, `SystemLogService`).
*   `BMS_project/wwwroot/`: Static assets (CSS, JS, images, uploads).

## Setup & Configuration

### Prerequisites
1.  **.NET 9.0 SDK**
2.  **MySQL Server** running locally or accessible via network.

### Configuration (`appsettings.json`)
Ensure the `DefaultConnection` string points to your MySQL instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;port=3306;Database=kabataan;User=root;Password=;ConvertZeroDateTime=True"
}
```
*Note: The project uses Mailtrap for email testing by default.*

### Database Initialization
The project uses Entity Framework Core Migrations.
To apply migrations and create the database:
```bash
dotnet ef database update --project BMS_project
```

### Running the Application
To start the web server:
```bash
dotnet run --project BMS_project
```
The application typically runs on `https://localhost:7196` or `http://localhost:5253` (check `launchSettings.json`).

### API Documentation
Swagger UI is available in the Development environment at `/swagger/index.html`.

## Development Conventions
*   **Database First/Code First:** The project appears to use Code First with EF Core, mapping entities to specific table names (e.g., `ToTable("youth_member")`).
*   **Localization:** Supports `en-PH` and `fil-PH` cultures.
*   **Dependency Injection:** Services are registered in `Program.cs` (mostly `Scoped`).
*   **Styling:** Bootstrap is used for the UI layout.
