# HealthCareSystem – Console Application

## Overview

This project is a robust **C# Console Application** simulating a healthcare management system designed with a strong emphasis on **security, privacy, 
and granular role-based access control (RBAC)**. It adheres to a detailed and realistic set of requirements, common in public sector healthcare environments.

The system has progressed significantly from a minimal prototype to a feature-rich application. All critical data, including users, permissions, and appointments, 
is stored persistently in **CSV files** within the data/ directory. The focus of the development has been on creating a 
**stable and well-documented foundation rather than achieving rapid feature-completeness**.

#### Core Principles
**Security & Privacy**: Users are strictly limited to the data and functions necessary for their assigned roles.

**Persistent Storage**: Data persistence is handled via CSV files in the data/ folder, replacing the initial in-memory storage.

**Structured Data**: Utilizes dedicated classes (User, Appointment, Location, Schedule) and Enums for clear, maintainable logic.

**Granular RBAC**: Admins can delegate fine-grained permissions to other administrators

---

#### Data Structures (Key Classes)
The application relies on several key data structures to manage the system's complexity:

User: Stores core user data, including hashed password, Role, PersonelRole, a list of specific Permissions, and assigned locations/personnel IDs.

Location: Represents a physical facility (HospitalName) within a geographical area (Region).

Appointment: Represents a scheduled event, including patient ID (UserId), Date, Doctor, Department, and Status (e.g., Pending, Approved).

Schedule: A container class that aggregates all Appointment objects belonging to a specific user (Patient or Personnel), facilitating easy schedule viewing and management.

---

#### User Roles and Key Features
The system defines three primary roles, with Admin having advanced delegation capabilities:
| Role | Core Responsibilities & Access |
| --- | ----------- |
| Admin | Manages the permission system, personnel accounts, locations, and approves/denies new patient registrations. |
| Personnel | Views and manages patient medical journals (including setting read permissions), registers/modifies appointments, and views location schedules. |
| Patient | Requests registration, views personal medical journal, and requests/views appointments. |

- Role	Core Responsibilities & Access
Admin	Manages the permission system, personnel accounts, locations, and approves/denies new patient registrations.
Personnel	Views and manages patient medical journals (including setting read permissions), registers/modifies appointments, and views location schedules.
Patient	Requests registration, views personal medical journal, and requests/views appointments

---
####  Implemented Features
##### General Access
- Secure Login and Logout system (using password hashing/salting).

- Ability to Request registration as a patient.

- Ability to Request registration as an admin.

- Any logged-in user can View their personal Schedule.

##### Administration (Admin)
- Permission Management: Delegate authority to other admins to handle the permission system, registrations, personnel accounts, and location additions.

- Registration Management: Accept or Deny patient registration requests.

- Personnel & Location: Create accounts for personnel and Add new locations.

##### Personnel
- Appointment Management: Register, Modify, and Approve incoming appointment requests.

- Journal Access: View patient journal entries and mark entries with different levels of read permissions.

- Schedule Management: View the schedule of a specific location.

###### Patient
- Journal Access: View their own medical journal entries.

- Booking: Request an appointment with specific details.

---
### Quality Assurance
#### Unit Tests (XUnit)
To ensure the stability and reliability of the core logic, the project utilizes XUnit for unit testing. The test suite specifically validates critical aspects of the User data model and security features:

Authentication & Security:

- Verification that passwords are hashed and salted upon user creation.

- Testing successful (True) and failed (False) login attempts using TryLogin.

Registration Flow:

- Confirmation that the registration status correctly moves from Pending to Accepted or Denied.

Granular Permissions:

- Testing the mechanics of granting and revoking specific permissions (e.g., AddLocation, AddAdmin).

- Validation that access checks (HasPermission) work correctly and that permissions lists default to Permissions.None when empty.


---

## 🚀 How to Run

1. **Requirements**:
   - [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) or later
   - Windows, Linux, or macOS
   - Clone the project:
```bash
     git clone git@github.com:Mezea11/health-care-system.git
```
2. **Build and Run**:
   1 Open a terminal in the project root directory or open it in an IDE.
    2 Run the following commands: 
     ```bash
     
    dotnet build 
   
    dotnet run
     ```

### Predefined Test Accounts

If the data file (data/users.csv) does not exist, the application will automatically create initial test users.

| Username | Role | Password|
| --- | ----------- | ------ |
| patient | Patient | 123 |
| personnel | Personnel | 123 |
| admin | Admin | 123 |
| superadmin | SuperAdmin | 123 |


#### Running the Unit Tests
The dedicated unit tests, likely located in a separate test project (e.g., HealthCareSystem.Tests), can be executed using the .NET CLI. This is essential to confirm that all core business and security logic is functioning correctly as intended.

Navigate to the project's root directory in your terminal and run:

```bash
     # Command to discover and run all unit tests in the solution
     dotnet test --logger "console;verbosity=detailed"
```
Using verbosity=detailed will provide a more descriptive output for each test run, making it clearer what the test suite is handling, instead of just the summary.

<!-- 1. Open the project in **Visual Studio** or another C# IDE.
2. dotnet build
3. dotnet run -->

##### installing XUnit 

```bash
# 1. Add the core testing framework (xUnit)
dotnet add package xunit

# 2. Add the Visual Studio test runner (required to run tests in VS and with 'dotnet test')
dotnet add package xunit.runner.visualstudio

# 3. Add the Microsoft Test SDK (required for test discovery and execution)
dotnet add package Microsoft.NET.Test.Sdk

```

---
