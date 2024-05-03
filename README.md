# Appointment Scheduler Api

Appointment Scheduler Api is a ASP.NET Core Web API project designed to demonstrate CRUD operations, user verification and authentication, scheduled email and sms sending.

## Installation

1. Clone the repository:
2. Navigate to the project directory:

## Configuration

1. Open `appsettings.json` and modify the database connection string if needed.
2. Optionally, configure any other settings relevant to your environment.

## Usage

### Endpoints
#### Appointment
- `GET /api/Appointment/GetAllAppointments`: Retrieves a list of all appointments.
- `GET /api/Appointment/GetAnAppointment`: Retrieves details of a specific appointment by ID.
- `POST /api/Appointment/CreateAppointment`: Creates a new appointment.
- `PUT /api/Appointment/EditAppointment`: Updates an existing appointment.
- `DELETE /api/Appointment/DeleteAppointment`: Deletes an appointment.
- `POST /api/Appointment/Remainder`: Sends an email and sms remainder when the time for an appointment is close.
- `POST /api/Appointment/Sends-Sms`: Sends Sms to a user provided number.

#### User
- `POST /api/Auth/register`: Creates a new user and sends a verification email.
- `POST /api/Auth/login`: Authenticates and redirects user to main page.
- `POST /api/Auth/SendVerificationEmail`: Sends a verification email.
- `POST /api/Auth/SendMail`: Sends email to a provided email.
- `POST /api/Auth/Forgot-Password`: Provides a user with a temporary forgot-password token.
- `POST /api/Auth/Reset-Password`: Resets user password.
- `GET /api/Auth/Verify`: Verifies user email.
- `GET /api/Auth/GetUserById`: Retrieves details of a specific user by email.
- `GET /api/Auth/GetUserByEmail`: Retrieves details of a specific user by Id.
- `Delete /api/Auth/DeleteUser`: Deletes a user.
 
##### Sample Request (POST  /api/Appointment/CreateAppointment)
```json
{
  "startTime": "2024-02-16T10:08:02.038Z",
  "endTime": "2024-02-16T10:08:02.038Z",
  "appointmentTitle": "new appointment",
  "numberOfAttendees": 5,
  "location": "new york",
  "appointmentUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

## Testing
1. Swagger is implemented for testing the api
2. Alternatively use tools like Postman or curl to send requests to the API.

## Technologies and NuGet Packages
- ASP.NET Core
- Entity Framework Core
- AutoMapper
- Hangfire
- Mailkit
- Vonage
- Firebase
