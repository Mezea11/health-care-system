using System;

namespace App
{
    // Represents a single appointment in the healthcare system
    public class Appointment
    {
        // --- Properties ---
        public int UserId { get; set; } // The ID of the patient or user who wins this appointment
        public DateTime Date { get; set; } // The date and time when the appointment is scheduled
        public string Doctor { get; set; } // The name of the doctor responsible for this appointment
        public string Department { get; set; }// The department ir medical unit where the appointment will take place
        public string Type { get; set; } // The type of appointment (e.g., "Checkup", "Follow-up")
        public string Status { get; set; } = "Pending"; // Default status
        public int? PersonnelId { get; set; } // Optional link to staff
        public bool IsApproved { get; set; } = false; //Indicated whether the appointment has been approved by personnel, Default is false (not approved)

        // Constructor to create a new Appointment with all requied details
        public Appointment() { }

        public Appointment(int userId, DateTime date, string doctor, string department, string type)
        {
            UserId = userId; //Assign the appointment to correct user
            Date = date; // Set the appointment date and time
            Doctor = doctor; // Assign the doctor responsible
            Department = department; // Specify the department or unit
            Type = type; // Specify the type of appointment
            Status = "Pending"; // Default status is "Pending"
            IsApproved = false; // Default approval flas is false
        }

        // --- Methods ---

        // Returns a formatted string for console output
        public string Format()
        {
            return $"Date: {Date:yyyy-MM-dd HH:mm} | Doctor: {Doctor,-15} | Dept: {Department,-15} | Type: {Type,-12} | Status: {Status}";
        }

        // Converts appointment to a single-line string for saving to a TXT file
        public string ToFileString()
        {
            return $"{UserId};{Date:yyyy-MM-dd HH:mm};{Doctor};{Department};{Type};{Status};{(PersonnelId.HasValue ? PersonnelId.Value.ToString() : "")}";
        }

        // Creates an Appointment object from a line of text. Returns null if the line is empty or does not contain enough data
        public static Appointment? FromFileString(string line)
        {
          //Check if the line is null, empty or whitespace
            if (string.IsNullOrWhiteSpace(line))
                return null;

            // Split the line into parts using ';' as separator
            string[] parts = line.Split(';');

            //Validate that we have at least the minimum number of fields
            if (parts.Length < 5)
                return null;

            //Parse required fields
            int userId = int.Parse(parts[0]); // The user/patient ID
            DateTime date = DateTime.Parse(parts[1]); // The appointment date and time
            string doctor = parts[2]; // Doctor's name
            string department = parts[3]; // Department or medical unit
            string type = parts[4]; // Type of appointment

            // Create a new Appointment object with parsed values
            var appointment = new Appointment(userId, date, doctor, department, type);

            // Optinal: Set the appointment status if provided
            if (parts.Length > 5)
                appointment.Status = parts[5];

            // Optional: Set the PersonnelId if provieded and valid
            if (parts.Length > 6 && int.TryParse(parts[6], out int personnelId))
                appointment.PersonnelId = personnelId;

            // Return the created appointment object
            return appointment;
        }
    }
}
