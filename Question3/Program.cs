using System;
using System.Collections.Generic;

// a. Marker interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// b. Product classes
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"[Electronics] ID: {Id}, Name: {Name}, Qty: {Quantity}, Brand: {Brand}, Warranty: {WarrantyMonths} months";
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"[Grocery] ID: {Id}, Name: {Name}, Qty: {Quantity}, Expiry: {ExpiryDate:d}";
    }
}

// e. Custom Exceptions
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// d. Generic inventory repository
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        _items.Remove(id);
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException($"Quantity cannot be negative: {newQuantity}");
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        _items[id].Quantity = newQuantity;
    }
}

// f. WareHouseManager
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new();
    private InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        // Electronics
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12));

        // Groceries
        _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
        _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(2)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine(item);
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock updated: {item.Name}, New Qty: {item.Quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    // Accessors for testing in Main
    public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
    public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
}

// Main Program
public class Program
{
    public static void Main()
    {
        var manager = new WareHouseManager();
        manager.SeedData();

        Console.WriteLine("=== Grocery Items ===");
        manager.PrintAllItems(manager.GroceriesRepo);

        Console.WriteLine("\n=== Electronic Items ===");
        manager.PrintAllItems(manager.ElectronicsRepo);

        Console.WriteLine("\n=== Testing Exceptions ===");

        // Duplicate item
        try
        {
            manager.GroceriesRepo.AddItem(new GroceryItem(1, "Yogurt", 10, DateTime.Now.AddDays(5)));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Duplicate Error: {ex.Message}");
        }

        // Remove non-existent item
        manager.RemoveItemById(manager.ElectronicsRepo, 999);

        // Invalid quantity update
        try
        {
            manager.ElectronicsRepo.UpdateQuantity(1, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Invalid Quantity Error: {ex.Message}");
        }
    }
}
