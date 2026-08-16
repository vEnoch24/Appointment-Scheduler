# Appointment Scheduler API

Appointment Scheduler API is an ASP.NET Core Web API that provides user authentication/verification, CRUD operations for appointments, scheduled reminders via background jobs, push notifications, and SMS/email delivery. It is intended as a personal appointment scheduler backend demonstrating common production patterns: EF Core persistence, background processing (Hangfire), MailKit-based email, SMS integration, and push notifications.

What this project is good for
- Building or demoing an appointment/calendar backend with authentication and notification flows.
- Learning integration patterns: EF Core, background jobs, SMTP (MailKit), SMS (Twilio), push notifications (Firebase).
- Bootstrapping a small production-ready API that sends scheduled reminders.

Tech stack
- Language: C# (ASP.NET Core)
- Framework / runtime: .NET 7+ (see csproj to confirm target)
- Notable libraries / tools:
  - Entity Framework Core (DbContext + Migrations)
  - Hangfire (background/scheduled jobs)
  - MailKit (SMTP email sending)
  - Twilio (SMS client implementation present)
  - Firebase Admin (push notification key is present in repo)
  - AutoMapper (profiles directory exists)
  - Swagger / Swashbuckle for API exploration

IMPORTANT security note
This repository currently contains a Firebase admin JSON key file (appointmentpushnotificat-*.json). That is a secret and must be removed from the repository (git rm --cached ...) and rotated immediately. Move such keys to secure storage (Azure Key Vault, environment variables, or a secrets manager). Add sensitive filenames to .gitignore.

Contents / project layout
```
Appointment Scheduler.csproj           # project file
Program.cs                            # app startup + DI / middleware
appsettings.json                      # base configuration (connection strings, providers)
appsettings.Development.json          # dev config
Controllers/
  AppointmentController.cs            # appointment endpoints (CRUD, reminders)
  AuthController.cs                   # register/login/verification/password flows
  PushNotificationController.cs       # push notification endpoint(s)
  WeatherForecastController.cs        # sample dev controller
Data/
  AppointmentDbContext.cs             # EF Core DbContext
  MailData.cs                         # email templates/data model
Dto/
  AppointmentDto.cs
  AppointmentUserDto.cs
  EmailDto.cs
  PushMessageDto.cs
Model/
  Appointment.cs
  AppointmentUser.cs
Services/
  EmailService/
    EmailService.cs                   # MailKit implementation
    IEmailService.cs
  SmsService/
    TwilioClient.cs                   # Twilio SMS client
  PushNotification/
    IPushNotificationService.cs
    PushNotificationService.cs
Migrations/                            # EF Core migrations
Profile/                                # AutoMapper profiles
Repository/                             # repository pattern implementations
RequestPayload/                         # request models used by controllers
View/                                   # view models / DTOs for responses
Properties/
README.md
```

How it fits together (runtime shape)
- Requests arrive at controllers (Controllers/*). Authentication/authorization are handled by middleware configured in Program.cs.
- Controllers call into repository/service layers (Repository/, Services/) to perform work.
- Persistence uses EF Core through AppointmentDbContext (Data/AppointmentDbContext.cs). Migrations live in Migrations/.
- Background jobs and scheduled reminders use Hangfire (configured in Program.cs) and invoke services such as EmailService and SmsService.
- Notifications:
  - Email: MailKit-based service (Services/EmailService).
  - SMS: Twilio client present (Services/SmsService/TwilioClient.cs).
  - Push: Firebase Admin integration (Services/PushNotification).

Getting started — prerequisites
- .NET SDK (recommended 7.x or as targeted by the csproj)
- SQL Server or Azure SQL (connection string in appsettings)
- Optional for push notifications: Firebase service account JSON (do NOT commit to repo)
- Optional for SMS: Twilio account (Account SID, Auth Token, From number)
- Optional for email: SMTP server credentials (MailKit-compatible)

Configuration
The project uses appsettings.json and environment-specific overrides (appsettings.Development.json). Key configuration areas you will need to provide:

- ConnectionStrings:DefaultConnection — EF Core DB connection string
- JwtSettings (or similar) — secret, issuer, expiry (used for authentication)
- Mail settings — SMTP host, port, username, password, sender name/email
- Twilio: AccountSid, AuthToken, FromNumber (or other SMS provider credentials)
- Firebase: path to service account JSON (or JSON content stored securely)
- Hangfire storage connection (if using a separate DB/Redis for Hangfire)

Example (appsettings keys you might see)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=AppointmentDb;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "Secret": "REPLACE_WITH_LONG_RANDOM_SECRET",
    "Issuer": "AppointmentScheduler",
    "ExpiryMinutes": 60
  },
  "Mail": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "smtp-user",
    "Password": "smtp-pass",
    "From": "no-reply@example.com"
  },
  "Twilio": {
    "AccountSid": "ACxxxxx",
    "AuthToken": "xxxxx",
    "From": "+1234567890"
  },
  "Firebase": {
    "ServiceAccountPath": "path/to/serviceAccount.json"
  }
}
```
(Inspect appsettings.json and Program.cs to confirm exact key names used in the code.)

Remove secrets from repo
- Delete any committed credential files from the repository history and add them to .gitignore.
- Rotate the keys after removal.

Run locally (short path)
1. Clone
   git clone https://github.com/vEnoch24/Appointment-Scheduler.git
2. Configure
   - Copy appsettings.Development.json to configure local values or use environment variables.
   - Provide a database connection string and create the database if needed.
   - Provide SMTP, Twilio, or Firebase credentials (only locally or via secure config).
3. Apply migrations
   - dotnet tool install --global dotnet-ef  (if not already installed)
   - dotnet ef database update
   (or run the app and allow migrations to run automatically if configured)
4. Build & run
   - dotnet build
   - dotnet run
5. Visit Swagger UI to explore endpoints
   - Typically: https://localhost:5001/swagger or http://localhost:5000/swagger (check Program.cs for exact URL/bindings)

Docker (if desired)
- No Dockerfile is included at repo root (check the repo); create one that restores, builds, and runs the project and ensure environment vars/secrets are injected by your container orchestration or CI.

Database migrations
- Migrations folder exists — use EF Core CLI:
  - dotnet ef migrations add <Name>
  - dotnet ef database update

Exposed API endpoints (controllers observed)
- AppointmentController (Controllers/AppointmentController.cs)
  - GET /api/Appointment/GetAllAppointments
  - GET /api/Appointment/GetAnAppointment?id={id}
  - POST /api/Appointment/CreateAppointment
  - PUT /api/Appointment/EditAppointment
  - DELETE /api/Appointment/DeleteAppointment?id={id}
  - POST /api/Appointment/Remainder  (send reminder email & SMS when appointment time approaches)
  - POST /api/Appointment/Sends-Sms (sends SMS to provided number)
- AuthController (Controllers/AuthController.cs)
  - POST /api/Auth/register          (creates user, sends verification)
  - POST /api/Auth/login             (authenticates user; returns token)
  - POST /api/Auth/SendVerificationEmail
  - POST /api/Auth/SendMail
  - POST /api/Auth/Forgot-Password
  - POST /api/Auth/Reset-Password
  - GET  /api/Auth/Verify
  - GET  /api/Auth/GetUserById?email={email}
  - GET  /api/Auth/GetUserByEmail?id={id}
  - DELETE /api/Auth/DeleteUser
- PushNotificationController (Controllers/PushNotificationController.cs)
  - POST endpoints for sending push messages (uses Services/PushNotification)
- WeatherForecastController (sample/test endpoint)

Sample requests

Create appointment (JSON body)
```json
{
  "startTime": "2026-02-16T10:08:02Z",
  "endTime": "2026-02-16T11:08:02Z",
  "appointmentTitle": "New appointment",
  "numberOfAttendees": 5,
  "location": "New York",
  "appointmentUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

cURL — register
```bash
curl -X POST "https://localhost:5001/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"P@ssw0rd","fullName":"First Last"}'
```

cURL — create appointment (with Bearer token)
```bash
curl -X POST "https://localhost:5001/api/Appointment/CreateAppointment" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT_TOKEN>" \
  -d '@appointment.json'
```

Notifications
- Email: implemented via Services/EmailService/EmailService.cs using MailKit.
- SMS: Twilio client code present at Services/SmsService/TwilioClient.cs. Provide Twilio credentials in configuration.
- Push: PushNotificationService uses the Firebase admin SDK; the repo currently contains a JSON key file which must be removed and stored securely.

Background jobs / reminders
- Hangfire is referenced in the project (see Program.cs). Hangfire is used to schedule recurring jobs and to queue job(s) that send reminders/notifications.
- Configure Hangfire persistent storage in production (SQL or Redis) and secure the Hangfire dashboard.

Testing
- Swagger UI is available for manual testing.
- Use tools like Postman or curl to exercise endpoints.
- Add unit and integration tests (none found at top-level; consider adding a tests/ project).

Developer notes & recommendations
- Secrets management: remove credential files and use environment variables or a secrets manager. Rotate exposed keys immediately.
- Add a LICENSE (if this is not intended to be private) and CONTRIBUTING.md to help other contributors.
- Add a .dockerignore and Dockerfile if you want to publish a container image.
- Consider centralizing configuration keys & using strongly typed options (IOptions<T>) for JwtSettings, Mail, Twilio, Firebase, etc.
- Add integration tests for the notification pipeline (mock external providers).
- Consider rate-limiting on public endpoints and validation for user inputs.

Known TODOs / improvements (suggested)
- Replace any hard-coded secrets with configuration references.
- Add logging and structured correlation IDs for requests.
- Implement refresh token flow for authentication (if long-lived sessions needed).
- Add healthchecks and readiness/liveness endpoints for production orchestration.
- Add CI workflow (.github/workflows) to run tests and build artifacts.

Contributing
- Fork the repository, create a feature branch, implement changes, and open a pull request describing the change.
- Ensure no secrets are committed. Add tests for new behaviors.

License
- No license file found in the repository (add LICENSE to clarify terms).

Questions you might want to ask next
- Which environment variables or configuration keys are required exactly by Program.cs and the services (I can list exact keys if you want)?
- Do you want me to remove the committed Firebase JSON, add it to .gitignore, and produce a small script/guide to rotate those keys?
- Should I add a Dockerfile + docker-compose that runs the API, a SQL Server container, and a Hangfire dashboard for local development?

---
If you’d like, I can:
- Produce a ready-to-paste, fully-expanded README.md using this content (with a short "Quickstart" section tailored to your project configuration).
- Create a .gitignore entry and a short note and commit (or show the exact git commands) that removes the Firebase JSON from the repo and adds instructions to rotate the credential.
