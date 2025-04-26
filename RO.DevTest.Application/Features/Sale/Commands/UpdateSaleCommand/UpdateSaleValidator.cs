using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand
{
    public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleCommandValidator()
        {
            RuleFor(x => x.SaleId).NotEmpty().WithMessage("ID da venda é obrigatório.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("ID do usuário é obrigatório.");
            RuleFor(x => x.Items).NotEmpty().WithMessage("É necessário adicionar ao menos um item à venda.");
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(x => x.ProductId).NotEmpty().WithMessage("ID do produto é obrigatório.");
                items.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantidade do produto deve ser maior que zero.");
            });
        }
    }
}
