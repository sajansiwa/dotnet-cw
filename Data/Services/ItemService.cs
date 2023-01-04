using System.Text.Json;
using Coursework.Data.Model; 

namespace Coursework.Data.Services;

public static class ItemService
{
    public static void SaveAllItems(List<Item> todos)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string appItemsFilePath = Utils.GetItemsFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(todos);
        File.WriteAllText(appItemsFilePath, json);
    }

    public static List<Item> GetAllItems()
    {
        string itemFilePath = Utils.GetItemsFilePath();
        if (!File.Exists(itemFilePath))
        {
            return new List<Item>();
        }

        var json = File.ReadAllText(itemFilePath);

        return JsonSerializer.Deserialize<List<Item>>(json);
    }

    public static List<Item> CreateItem(Guid userId, Guid id, string name, int quantityInInventory)
    {
        if(quantityInInventory <= 0)
        {
            throw new Exception("Quantity must be greater than 0.");
        }
        
        List<Item> items = GetAllItems();
        items.Add(new Item
        {
            Name = name,
            Quantity = quantityInInventory,


        });
        SaveAllItems(items);
        return items;
    }

    public static List<Item> DeleteItem(Guid id)
    {
        List<Item> items = GetAllItems();
        Item item = items.FirstOrDefault(x => x.Id == id);

        if (item == null)
        {
            throw new Exception("Item's not found.");
        }

        items.Remove(item);
        SaveAllItems(items);
        return items;
    }

    public static List<Item> UpdateQuantity(Guid id, int QuantityInInventory)
    {
        List<Item> items = GetAllItems();
        Item todoToUpdate = items.FirstOrDefault(x => x.Id == id);

        if (todoToUpdate == null)
        {
            throw new Exception("Item's not found.");
        }

        todoToUpdate.Quantity += QuantityInInventory;
        SaveAllItems(items);
        return items;

    }
}
