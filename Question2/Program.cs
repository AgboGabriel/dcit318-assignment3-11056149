using System;
using System.Collections.Generic;
using System.Linq;

// a. Generic repository
public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return items;
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// b. Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }
}

// c. Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription ID: {Id}, Patient ID: {PatientId}, Medication: {MedicationName}, Date: {DateIssued:d}";
    }
}

// g. HealthSystemApp
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();
    private Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        // Patients
        _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
        _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Charlie Brown", 29, "Male"));

        // Prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Metformin", DateTime.Now.AddDays(-20)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Paracetamol", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(5, 2, "Lisinopril", DateTime.Now.AddDays(-1)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo
            .GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("=== All Patients ===");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.ContainsKey(patientId)
            ? _prescriptionMap[patientId]
            : new List<Prescription>();
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        Console.WriteLine($"\n=== Prescriptions for Patient ID {id} ===");
        var prescriptions = GetPrescriptionsByPatientId(id);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found.");
        }
        else
        {
            foreach (var prescription in prescriptions)
            {
                Console.WriteLine(prescription);
            }
        }
    }
}

// Main program
public class Program
{
    public static void Main()
    {
        var app = new HealthSystemApp();

        app.SeedData();
        app.BuildPrescriptionMap();

        app.PrintAllPatients();

        Console.Write("\nEnter Patient ID to view prescriptions: ");
        if (int.TryParse(Console.ReadLine(), out int patientId))
        {
            app.PrintPrescriptionsForPatient(patientId);
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }
}
