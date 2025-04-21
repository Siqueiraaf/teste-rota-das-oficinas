namespace RO.DevTest.Domain.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!; // admin ou customer
    public decimal TotalAmount { get; set; }
    public DateTime SaleDate { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
