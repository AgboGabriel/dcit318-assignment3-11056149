using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// a. Record with marker interface
public interface IInventoryEntity
{
    int Id { get; }
}

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// c. Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            using (var stream = new FileStream(_filePath, FileMode.Create))
            {
                JsonSerializer.Serialize(stream, _log);
            }
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No saved data found.");
                return;
            }

            using (var stream = new FileStream(_filePath, FileMode.Open))
            {
                var loaded = JsonSerializer.Deserialize<List<T>>(stream);
                if (loaded != null)
                {
                    _log = loaded;
                    Console.WriteLine("Data loaded successfully.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

// f. Integration Layer
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Mouse", 20, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Keyboard", 10, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Monitor", 7, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Headset", 12, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("No items to display.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

// g. Main
public class Program
{
    public static void Main()
    {
        string filePath = "inventory.json";

        // First session
        var app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();

        // Simulate new session (clearing memory)
        app = new InventoryApp(filePath);
        app.LoadData();
        app.PrintAllItems();
    }
}
