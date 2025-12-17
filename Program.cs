using System;
using System.Collections.Generic;

// ===============================================
// COMPLETE HEALTHCARE OOP DEMO - C# CONSOLE APP

//Patient Class - With encapsulation, computed properties, and validation
public class Patient
{
    // Private backing fields - Hidden from outside world (Encapsulation)
    private string _name;
    private DateTime _dateOfBirth;

    // Public read-only collection - Can add entries, but cannot replace the list
    public List<string> MedicalHistory { get; } = new List<string>();

    // Constructor - Forces name and DOB to be provided at creation
    public Patient(string name, DateTime dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Patient name cannot be empty.");
        if (dateOfBirth > DateTime.Now)
            throw new ArgumentException("Date of birth cannot be in the future.");

        _name = name;
        _dateOfBirth = dateOfBirth;
    }

    // Read-only property - External code can read name, but cannot change it
    public string Name => _name;

    // Computed property - Age calculated automatically every time it's accessed
    public int Age => (DateTime.Now - _dateOfBirth).Days / 365;

    // Computed property - Returns true if patient is under 18
    public bool IsMinor => Age < 18;

    // Computed property - Nice readable summary of medical history
    public string FullMedicalSummary =>
        MedicalHistory.Count > 0
            ? string.Join("; ", MedicalHistory)
            : "No medical history recorded";

    // Safe method to add medical entry with validation
    public void AddMedicalEntry(string entry)
    {
        if (string.IsNullOrWhiteSpace(entry))
            throw new ArgumentException("Medical entry cannot be empty.");

        MedicalHistory.Add($"{DateTime.Now:yyyy-MM-dd}: {entry.Trim()}");
    }
}

//Doctor Class
public class Doctor
{
    // Public properties - Can be read and modified (could be made read-only later)
    public string Name { get; set; }
    public string Specialization { get; set; }

    // Doctor's schedule - External code can add appointments, but not replace the list
    public List<Appointment> Schedule { get; } = new List<Appointment>();

    // Constructor - Requires name and specialization
    public Doctor(string name, string specialization)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Specialization = specialization ?? throw new ArgumentNullException(nameof(specialization));
    }

    // Optional helpful property
    public int TotalAppointments => Schedule.Count;
}
public class Appointment
{
    // Date is controlled - we use private setter and validation method
    public DateTime Date { get; private set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public string Status { get; set; } = "Scheduled";

    // Constructor - Validates everything at creation
    public Appointment(DateTime date, Patient patient, Doctor doctor)
    {
        Patient = patient ?? throw new ArgumentNullException(nameof(patient));
        Doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));

        // Use SetDate to apply all validations from the start
        SetDate(date);

        // Automatically link to doctor's schedule (bidirectional relationship)
        doctor.Schedule.Add(this);
    }

    // Method to change appointment date with full business rules
    public void SetDate(DateTime newDate)
    {
        // Rule 1: Must be in the future
        if (newDate < DateTime.Now)
            throw new ArgumentException("Appointment date must be in the future.");

        // Rule 2: No weekends (clinic closed)
        if (newDate.DayOfWeek == DayOfWeek.Saturday || newDate.DayOfWeek == DayOfWeek.Sunday)
            throw new ArgumentException("Appointments are not available on weekends.");

        // Rule 3: No overlapping appointments (assume 1-hour slots)
        bool hasConflict = Doctor.Schedule
            .Any(a => a != this && Math.Abs((a.Date - newDate).TotalHours) < 1);

        if (hasConflict)
            throw new InvalidOperationException($"Doctor {Doctor.Name} already has an appointment at that time.");

        Date = newDate;
    }

    // Bonus: Convenient reschedule method
    public void Reschedule(DateTime newDate)
    {
        SetDate(newDate);
        Status = "Rescheduled";
    }

    // Optional: Cancel appointment
    public void Cancel()
    {
        Status = "Cancelled";
    }
}

//BillingItem Class - Strong encapsulation with validation
public class BillingItem
{
    private string _description;
    private decimal _amount;

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Description cannot be empty.");
            _description = value;
        }
    }

    public decimal Amount
    {
        get => _amount;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Amount must be positive.");
            _amount = value;
        }
    }

    public BillingItem(string description, decimal amount)
    {
        Description = description;  // Validation runs via setter
        Amount = amount;            // Validation runs via setter
    }

    public override string ToString() => $"{Description}: ${Amount:F2}";
}

// ===============================================
// MAIN PROGRAM - Demo and Testing Area
// ===============================================
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Healthcare OOP Demo ===\n");

        try
        {
            // Create objects
            Patient patient1 = new Patient("Alice Johnson", new DateTime(1995, 8, 20));
            Patient patient2 = new Patient("Bobby Smith", new DateTime(2018, 3, 10)); // Minor

            Doctor doctor = new Doctor("Dr. Sarah Lee", "Cardiology");

            // Add medical history
            patient1.AddMedicalEntry("Allergy to penicillin");
            patient1.AddMedicalEntry("Hypertension diagnosed");

            // Create appointment
            Appointment appointment = new Appointment(
                DateTime.Now.AddDays(7).Date.AddHours(10), // Next week, 10 AM
                patient1,
                doctor
            );

            // Display relationships
            Console.WriteLine($"Patient: {patient1.Name}, Age: {patient1.Age}, Minor: {patient1.IsMinor}");
            Console.WriteLine($"Medical Summary: {patient1.FullMedicalSummary}");
            Console.WriteLine($"Doctor: {doctor.Name} ({doctor.Specialization})");
            Console.WriteLine($"Appointment: {appointment.Date:yyyy-MM-dd hh:mm tt} - Status: {appointment.Status}");
            Console.WriteLine($"Doctor has {doctor.TotalAppointments} appointment(s)\n");

            // Demo BillingItem with validation
            Console.WriteLine("=== Billing Demo ===");
            BillingItem item1 = new BillingItem("Blood Test", 150.00m);
            BillingItem item2 = new BillingItem("ECG", 80.50m);
            Console.WriteLine(item1);
            Console.WriteLine(item2);

            // This will throw exception (uncomment to test)
            // BillingItem invalid = new BillingItem("X-Ray", -50m); // Negative amount
            // BillingItem emptyDesc = new BillingItem("", 100m);    // Empty description

            // Minor patient demo
            Console.WriteLine($"\nPatient 2: {patient2.Name}, Age: {patient2.Age}, IsMinor: {patient2.IsMinor}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}

