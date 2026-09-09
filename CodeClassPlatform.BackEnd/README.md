# Code Class Platform Backend

A production-oriented ASP.NET Core Web API for an educational platform with student, teacher, and admin workflows.

## Features

- JWT-based authentication and role authorization
- SQL Server for local and production persistence
- EF Core data model for accounts, profiles, courses, lectures, quizzes, progress, notifications, and external resources
- Google Drive link registration and verification with canonical URL normalization
- Swagger/OpenAPI integration with JWT auth support
- Dark mode Swagger UI
- BCrypt password hashing
- Login rate limiting
- CORS support for web and mobile clients

## Media Model

This backend does not support direct in-app media uploads.

Teachers and admins register existing Google Drive assets by URL. The system validates the URL, extracts the Google Drive file ID, stores the canonical URL, and verifies the link before use.

Important rule:

- Teachers do not upload media through the application.
- Only external Google Drive links are accepted and stored.

Core concepts:

- `ExternalResource`
- `IGoogleDriveLinkService`
- `GoogleDriveLinkService`
- `ResourcesController`

API endpoints:

- GET /api/resources/{id}
- POST /api/resources
- PUT /api/resources/{id}
- DELETE /api/resources/{id}
- POST /api/resources/{id}/verify

## Supported Google Drive URL shapes

The validator accepts standard Google Drive share URLs such as:

- https://drive.google.com/file/d/{fileId}/view
- https://drive.google.com/open?id={fileId}
- https://drive.google.com/uc?export=view&id={fileId}

The service rejects non-Google Drive URLs and unsafe schemes such as `javascript:`.

## Roles

- STUDENT
- TEACHER
- ADMIN

## Seeded Demo Accounts

The app seeds these local demo credentials automatically on first run:

- Admin: `admin@codeclass.local` / `Admin123!`
- Teacher: `teacher@codeclass.local` / `Teacher123!`
- Student: `student@codeclass.local` / `Student123!`

## Local Setup

1. Update the connection string in `appsettings.json`.
2. Restore dependencies: `dotnet restore`
3. Apply migrations: `dotnet ef database update`
4. Start the API: `dotnet run`

## Environment Variables

Use `appsettings.json` or user secrets for:

- `ConnectionStrings__DefaultConnection`
- `JWT__Key`
- `JWT__Issuer`
- `JWT__Audience`

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HATEM\\SQLEXPRESS;Initial Catalog=CodeClassPlatformDb;Integrated Security=True;TrustServerCertificate=True;"
  },
  "JWT": {
    "Key": "replace-with-long-secret-key",
    "Issuer": "CodeClassPlatform",
    "Audience": "CodeClassPlatformClients"
  }
}
```

## API Conventions

- Base route: `/api`
- Authentication: Bearer JWT
- Swagger is available in development at `/swagger`
- The API uses standard ASP.NET Core controller responses

## Notes

This project intentionally removes the old Cloudflare/R2 upload architecture. The system is now based on externally hosted Google Drive resources with validation and registration workflows, not file upload to the backend itself.
