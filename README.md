# HealthCareSystem – Console Application

## Overview

This project is a minimal **C# console application** that simulates a simplified health care management system.  
It demonstrates basic role-based access and user interaction in a console environment, built with a focus on **clarity and simplicity** rather than completeness or optimization.

The application includes three user roles:

- **Admin** – can create new users and view all users.
- **Personnel** – can view schedules and approve mock bookings.
- **Patient** – can view a mock medical journal and request mock appointments.

All data is mocked and stored in memory (no files or databases).

---

## Features

- Simple login and logout system
- Role-based menus and permissions
- Hardcoded list of users
- Admin can create new users
- Mock actions for patients and personnel

---

## How to Run

1. Open the project in **Visual Studio** or another C# IDE.
2. dotnet build
3. dotnet run

Log in using one of the predefined accounts:

Username: patient <br>
Password: 123 <br>

Username: personell <br>
Password: 123 <br>

Username: admin <br>
Password: 123 <br>
