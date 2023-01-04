namespace Coursework.Data.Model;

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } 
    public int Quantity { get; set; }   
    public DateTime LastTakenOut { get; set; }

}
