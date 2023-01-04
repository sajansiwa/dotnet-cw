using System.Text.Json;
using Coursework.Data.Model;


namespace Coursework.Data.Services;

public static class ItemRequestsService
{
    private static void SaveAllItemRequests(List<ItemRequest> todos)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string itemRequestsFilePath = Utils.GetItemRequestsFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(todos);
        File.WriteAllText(itemRequestsFilePath, json);
    }

    public static List<ItemRequest> GetAllItemRequests()
    {
        string todosFilePath = Utils.GetItemRequestsFilePath();
        if (!File.Exists(todosFilePath))
        {
            return new List<ItemRequest>();
        }

        var json = File.ReadAllText(todosFilePath);

        return JsonSerializer.Deserialize<List<ItemRequest>>(json);
    }

    public static List<ItemRequest> Create(Item Item, User requestedBy, int QuantityInInventory)
    {
        if (Item.Quantity == 0)
        {
            throw new Exception("Item is not available to request.");
        }
        if (Item.Quantity < QuantityInInventory)
        {
            throw new Exception("Quantity of item requested cannot be provided.");
        }
        List<ItemRequest> todos = GetAllItemRequests();
        todos.Add(new ItemRequest
        {
            Item = Item,
            RequestedBy = requestedBy,
            RequestedQuantity = QuantityInInventory,
            Status = Status.Pending
        });

        SaveAllItemRequests(todos);
        return todos;
    }

    public static List<ItemRequest> ApproveItemRequest(Guid ItemRequestId, User approvedBy)
    {
        List<ItemRequest> requestedItems = GetAllItemRequests();
        ItemRequest singleItemRequest = requestedItems.FirstOrDefault(x => x.Id == ItemRequestId);

        DateTime approvalDate = DateTime.Now;

        singleItemRequest.ApprovedBy = approvedBy;
        singleItemRequest.ApprovedDate = approvalDate;
        singleItemRequest.Status = Status.Approved;

        List<Item> items = ItemService.GetAllItems();
        Item itemToUpdate = items.FirstOrDefault(x => x.Id == singleItemRequest.Item.Id);

        itemToUpdate.LastTakenOut = approvalDate;
        itemToUpdate.Quantity -= singleItemRequest.RequestedQuantity;

        ItemService.SaveAllItems(items);
        SaveAllItemRequests(requestedItems);
        return requestedItems;
    }
    public static List<ItemRequest> RejectItemRequest(Guid ItemRequestId)
    {
        List<ItemRequest> requestedItems = GetAllItemRequests();
        ItemRequest singleItemRequest = requestedItems.FirstOrDefault(x => x.Id == ItemRequestId);

        singleItemRequest.ApprovedBy = null;
        singleItemRequest.Status = Status.Rejected;

        SaveAllItemRequests(requestedItems);
        return requestedItems;
    }
}

