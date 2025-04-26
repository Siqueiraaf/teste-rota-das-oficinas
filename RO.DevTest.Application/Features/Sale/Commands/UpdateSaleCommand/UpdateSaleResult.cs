using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand
{
    public class UpdateSaleResult
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }
        public List<SaleItemResult> Items { get; set; } = new List<SaleItemResult>();
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class SaleItemResult
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
