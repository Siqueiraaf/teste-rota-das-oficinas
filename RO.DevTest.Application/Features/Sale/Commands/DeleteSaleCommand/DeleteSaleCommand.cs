using MediatR;

namespace RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand
{
    public class DeleteSaleCommand : IRequest<Unit>
    {
        public Guid SaleId { get; set; }
        public Guid UserId { get; set; }
    }
}
