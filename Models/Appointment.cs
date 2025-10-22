using System;

namespace App
{
    // Represents a single appointment in the healthcare system
    public class Appointment
    {
        // --- Properties ---
        public int UserId { get; set; }                 // Patient/User ID
        public DateTime Date { get; set; }             // Appointment date & time
        public string Doctor { get; set; }             // Responsible doctor
        public string Department { get; set; }         // Department or unit
        public string Type { get; set; }               // Appointment type
        public string Status { get; set; } = "Pending";// Status: Pending/Approved/Cancelled
        public int? PersonnelId { get; set; }          // Optional: Assigned personnel
        public bool IsApproved { get; set; } = false;  // Whether personnel approved

        // --- Constructors ---
        public Appointment() { }

        public Appointment(int userId, DateTime date, string doctor, string department, string type)
        {
            UserId = userId;
            Date = date;
            Doctor = doctor;
            Department = department;
            Type = type;
            Status = "Pending";
            IsApproved = false;
        }

        // --- Methods ---

        // Formatted display for console
        public string Format()
        {
            return $"Date: {Date:yyyy-MM-dd HH:mm} | Doctor: {Doctor,-15} | Dept: {Department,-15} | Type: {Type,-12} | Status: {Status}";
        }

        // Convert appointment to a line for saving in text file
        public string ToFileString()
        {
            return $"{UserId};{Date:yyyy-MM-dd HH:mm};{Doctor};{Department};{Type};{Status};{(PersonnelId.HasValue ? PersonnelId.Value.ToString() : "")}";
        }

        // Parse a line from text file into an Appointment object
        public static Appointment? FromFileString(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var parts = line.Split(';');
            if (parts.Length < 6) return null;

            int userId = int.Parse(parts[0]);
            DateTime date = DateTime.Parse(parts[1]);
            string doctor = parts[2];
            string department = parts[3];
            string type = parts[4];
            string status = parts[5];

            var appointment = new Appointment(userId, date, doctor, department, type)
            {
                Status = status
            };

            if (parts.Length > 6 && int.TryParse(parts[6], out int personnelId))
                appointment.PersonnelId = personnelId;

            return appointment;
        }
    }
}
