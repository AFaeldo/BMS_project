# Project Context: BMS_project

## Overview
**BMS_project** is an ASP.NET Core MVC web application designed for managing **Barangay and Sangguniang Kabataan (SK)** operations. It acts as a comprehensive management system for youth profiles, projects, budgets, compliance documents, and announcements.

**Key Technologies:**
*   **Framework:** .NET 9.0 (ASP.NET Core MVC)
*   **Database:** MySQL 8.0+
*   **ORM:** Entity Framework Core 9.0 (Pomelo.EntityFrameworkCore.MySql)
*   **Authentication:** Cookie-based Authentication
*   **Email:** MailKit (configured for Mailtrap in development)
*   **Documentation:** Swagger/OpenAPI (available in Development)

## Architecture & Structure
The project follows a standard Model-View-Controller (MVC) architectural pattern.

### Key Directories
*   `Controllers/`: Handles incoming HTTP requests. Note the folder structure groupings:
    *   `SuperAdminController/`: Administration logic.
    *   `BarangaySKContoller/`: Logic for Barangay and SK operations (Note: folder name has a typo `Contoller`).
    *   `FederationPResidentController/`: Logic for the Federation President (Note: folder name has a typo `PResident`).
*   `Models/`: Domain entities representing the database structure.
    *   Entities are explicitly mapped to `snake_case` table names in `DbContext` (e.g., `User` -> `user`, `YouthMember` -> `youth_member`).
*   `ViewModels/`: Models specifically designed for View rendering and form handling (e.g., `ProjectCreationViewModel`, `UserFormViewModel`).
*   `Views/`: Razor views (`.cshtml`) for the UI.
*   `Data/`: Contains `ApplicationDbContext` for Entity Framework Core configuration.
*   `Services/`: Business logic and external service integrations (`EmailService`, `SystemLogService`, `TermService`).
*   `wwwroot/`: Static assets (CSS, JS, images, uploads).

## Setup & Configuration

### Prerequisites
1.  **.NET 9.0 SDK**
2.  **MySQL Server** running locally or accessible via network.

### Configuration (`appsettings.json`)
The application requires a valid connection string to a MySQL database.
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;port=3306;Database=kabataan;User=root;Password=;ConvertZeroDateTime=True"
}
```
*Note: The project is configured to use Mailtrap for email testing.*

### Database Initialization
The project uses Entity Framework Core Migrations.
To apply migrations and update/create the database:
```bash
dotnet ef database update --project BMS_project
```

### Running the Application
To start the web server:
```bash
dotnet run --project BMS_project
```
or simply `dotnet run` from the project root.

The application typically runs on:
*   `https://localhost:7196`
*   `http://localhost:5253`
(Check `Properties/launchSettings.json` for exact ports).

### API Documentation
Swagger UI is available in the Development environment, typically at `/swagger/index.html`.

## Development Conventions
*   **Database Mapping:** The project uses Code First with EF Core but explicitly maps entities to specific `snake_case` table names using `ToTable("table_name")` in `ApplicationDbContext`.
*   **Localization:** The app supports `en-PH` and `fil-PH` cultures, configured in `Program.cs`.
*   **Dependency Injection:** Services are registered in `Program.cs`, mostly with `Scoped` lifetime.
*   **Styling:** Bootstrap is used for the UI layout.
*   **Folder Naming:** Be aware of existing typos in Controller subdirectories when navigating (`BarangaySKContoller`, `FederationPResidentController`).
