# Project: BMS_project

## Project Overview

This is an ASP.NET Core web application named `BMS_project` that appears to be a Barangay Management System. It's built using .NET 9 and utilizes Entity Framework Core for data access with a MySQL database. The application has a role-based authentication system with three roles: `SuperAdmin`, `FederationPresident`, and `BarangaySk`. Each role has a specific set of permissions and functionalities.

### Key Technologies:

*   **Backend:** C#, ASP.NET Core 9
*   **Database:** MySQL with Entity Framework Core
*   **Frontend:** Razor Pages
*   **Authentication:** Cookie-based Authentication

### Architecture:

The project follows a standard ASP.NET Core MVC architecture:

*   **Models:** Defines the data structures of the application.
*   **Views:** Contains the Razor pages for the user interface.
*   **Controllers:** Handles user requests, interacts with the models, and renders the views.
*   **Data:** Contains the `ApplicationDbContext` for database interactions.

## Building and Running

### Prerequisites:

*   .NET 9 SDK
*   MySQL Server

### Running the application:

1.  **Configure the database connection:**
    *   Open `appsettings.json`.
    *   Modify the `DefaultConnection` string to point to your MySQL database.

2.  **Apply database migrations:**
    *   Open a terminal in the project root.
    *   Run `dotnet ef database update`. This will create the database and the necessary tables.

3.  **Run the application:**
    *   Run `dotnet run` in the project root.
    *   The application will be accessible at the URL specified in the `Properties/launchSettings.json` file (usually `https://localhost:5001` or `http://localhost:5000`).

## Development Conventions

*   **Naming:** The project uses PascalCase for classes and methods, and camelCase for local variables.
*   **Authentication:** The application uses role-based authorization. Controllers and actions are decorated with the `[Authorize(Roles = "...")]` attribute to restrict access.
*   **Database:** The project uses Entity Framework Core with a code-first approach. Database schema changes are managed through migrations.
*   **Routing:** The application uses conventional routing. The default route is `{controller=Home}/{action=Index}/{id?}`.
