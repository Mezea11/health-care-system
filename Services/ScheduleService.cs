using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace App
{
  // ==========================================
  // ScheduleService
  // Handles patient appointments and personnel shifts
  // JSON files: Data/schedules.json, Data/shifts.json
  // ==========================================
  public class ScheduleService
  {
    private readonly string _appointmentsFile = "Data/schedules.json";
    private readonly string _shiftsFile = "Data/shifts.json";

    // ===============================
    // APPOINTMENTS
    // ===============================

    // Load schedule for a specific user/patient
    public Schedule LoadSchedule(int userId)
    {
      if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");

      Schedule schedule = new Schedule(userId);

      if (!File.Exists(_appointmentsFile)) return schedule;

      string json = File.ReadAllText(_appointmentsFile);
      List<Appointment> allAppointments = JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();

      var userAppointments = allAppointments
          .Where(a => a.UserId == userId)
          .OrderBy(a => a.Date)
          .ToList();

      foreach (var app in userAppointments)
        schedule.AddAppointment(app);

      return schedule;
    }

    // Save or update an appointment
    public void SaveAppointment(Appointment appointment)
    {
      if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");

      List<Appointment> allAppointments = new List<Appointment>();

      if (File.Exists(_appointmentsFile))
      {
        string existingJson = File.ReadAllText(_appointmentsFile);
        allAppointments = JsonSerializer.Deserialize<List<Appointment>>(existingJson) ?? new List<Appointment>();

        // Replace existing appointment if same user/date
        var index = allAppointments.FindIndex(a => a.UserId == appointment.UserId && a.Date == appointment.Date);
        if (index >= 0)
          allAppointments[index] = appointment;
        else
          allAppointments.Add(appointment);
      }
      else
      {
        allAppointments.Add(appointment);
      }

      string newJson = JsonSerializer.Serialize(allAppointments, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(_appointmentsFile, newJson);
    }

    // Remove appointment
    public void RemoveAppointment(int userId, DateTime date)
    {
      if (!File.Exists(_appointmentsFile)) return;

      string json = File.ReadAllText(_appointmentsFile);
      List<Appointment> allAppointments = JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();

      allAppointments.RemoveAll(a => a.UserId == userId && a.Date == date);

      string updatedJson = JsonSerializer.Serialize(allAppointments, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(_appointmentsFile, updatedJson);
    }

    // Load all appointments (for personnel filtering, etc.)
    public List<Appointment> LoadAllAppointments()
    {
      if (!File.Exists(_appointmentsFile)) return new List<Appointment>();

      string json = File.ReadAllText(_appointmentsFile);
      if (string.IsNullOrWhiteSpace(json)) return new List<Appointment>();

      return JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();
    }

    // Load appointments assigned to a specific personnel
    public List<Appointment> LoadPersonnelSchedule(int personnelId)
    {
      var all = LoadAllAppointments();
      return all.Where(a => a.PersonnelId == personnelId)
                .OrderBy(a => a.Date)
                .ToList();
    }

    // ===============================
    // SHIFTS
    // ===============================

    // Load all shifts
    public List<Shift> LoadAllShifts()
    {
      if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");

      if (!File.Exists(_shiftsFile)) return new List<Shift>();

      string json = File.ReadAllText(_shiftsFile);
      return JsonSerializer.Deserialize<List<Shift>>(json) ?? new List<Shift>();
    }

    // Load shifts for a specific personnel
    public List<Shift> LoadShiftsForPersonnel(int personnelId)
    {
      var allShifts = LoadAllShifts();
      return allShifts.Where(s => s.PersonnelId == personnelId)
                      .OrderBy(s => s.Start)
                      .ToList();
    }

    // Save a shift
    public void SaveShift(Shift shift)
    {
      var allShifts = LoadAllShifts();
      allShifts.Add(shift);

      string json = JsonSerializer.Serialize(allShifts, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(_shiftsFile, json);
    }
    public List<Shift> GetColleaguesForShift(Shift currentShift)
    {
      // Säkerställ att mapp finns
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");

      if (!File.Exists(_shiftsFile))
        return new List<Shift>();

      string json = File.ReadAllText(_shiftsFile);
      if (string.IsNullOrWhiteSpace(json))
        return new List<Shift>();

      List<Shift> allShifts = JsonSerializer.Deserialize<List<Shift>>(json) ?? new List<Shift>();

      // Hitta alla skift som överlappar detta skift (men inte samma personal)
      var colleagues = allShifts
          .Where(s => s.PersonnelId != currentShift.PersonnelId &&
                      ((s.Start >= currentShift.Start && s.Start < currentShift.End) ||
                       (s.End > currentShift.Start && s.End <= currentShift.End) ||
                       (s.Start <= currentShift.Start && s.End >= currentShift.End)))
          .OrderBy(s => s.Start)
          .ToList();

      return colleagues;
    }


    // ===============================
    // CLASSES
    // ===============================
    public class Shift
    {
      public DateTime Start { get; set; }
      public DateTime End { get; set; }
      public int PersonnelId { get; set; }
    }

  }
}
