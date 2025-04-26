using FluentValidation;
using RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;

namespace RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand
{
    // Corrigir o tipo que o validador está aplicando
    public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleCommandValidator()
        {
            RuleFor(sale => sale.Items)
                .NotEmpty().WithMessage("A venda deve conter ao menos um item");

            RuleForEach(sale => sale.Items).SetValidator(new CreateSaleItemValidator());

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Método de pagamento inválido.")
                .NotNull().WithMessage("O método de pagamento é obrigatório.");
        }
    }

    // Alterado para validar SaleItemDto em vez de SaleItemResult
    public class CreateSaleItemValidator : AbstractValidator<SaleItemDto>
    {
        public CreateSaleItemValidator()
        {
            RuleFor(item => item.ProductId)
                .NotEmpty().WithMessage("O ID do produto é obrigatório");

            RuleFor(item => item.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero");
        }
    }
}
