namespace Coursework.Data.Model;

public class ItemRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Item Item { get; set; }
    public int RequestedQuantity { get; set; }
    public User ApprovedBy { get; set; }
    public User RequestedBy { get; set; }
    public DateTime ApprovedDate { get; set; }
    public Status Status { get; set; }
}
