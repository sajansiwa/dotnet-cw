using Coursework.Data.Model; 
namespace Coursework.Data.Services;

public static class ItemService
{
    private static void SaveAllItems(List<Item> todos)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(todos);
        File.WriteAllText(todosFilePath, json);
    }

    public static List<TodoItem> GetAll(Guid userId)
    {
        string todosFilePath = Utils.GetTodosFilePath(userId);
        if (!File.Exists(todosFilePath))
        {
            return new List<TodoItem>();
        }

        var json = File.ReadAllText(todosFilePath);

        return JsonSerializer.Deserialize<List<TodoItem>>(json);
    }

    public static List<TodoItem> Create(Guid userId, Guid id, string Item, DateTime TakenOutOn, int QuantityInInventory)
    {
        if (TakenOutOn < DateTime.Today)
        {
            throw new Exception("Due date must be in the future.");
        }

        List<TodoItem> todos = GetAll(userId);
        todos.Add(new TodoItem
        {

            CreatedBy = userId,
            Item = Item,
            TakenOutOn = TakenOutOn,
            QuantityInInventory = QuantityInInventory,


        });
        SaveAll(userId, todos);
        return todos;
    }

    public static List<TodoItem> Delete(Guid userId, Guid id)
    {
        List<TodoItem> todos = GetAll(userId);
        TodoItem todo = todos.FirstOrDefault(x => x.Id == id);

        if (todo == null)
        {
            throw new Exception("Item's not found.");
        }

        todos.Remove(todo);
        SaveAll(userId, todos);
        return todos;
    }

    public static void DeleteByUserId(Guid userId)
    {
        string todosFilePath = Utils.GetTodosFilePath(userId);
        if (File.Exists(todosFilePath))
        {
            File.Delete(todosFilePath);
        }
    }

    public static List<TodoItem> Update(Guid userId, Guid id, string Item, DateTime TakenOutOn, int QuantityInInventory)
    {
        List<TodoItem> todos = GetAll(userId);
        TodoItem todoToUpdate = todos.FirstOrDefault(x => x.Id == id);

        if (todoToUpdate == null)
        {
            throw new Exception("Item's not found.");
        }

        todoToUpdate.Item = Item;
        todoToUpdate.TakenOutOn = TakenOutOn;
        todoToUpdate.QuantityInInventory = QuantityInInventory;
        SaveAll(userId, todos);
        return todos;

    }
}
