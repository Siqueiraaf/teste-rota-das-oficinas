using System.ComponentModel;

namespace RO.DevTest.Domain.Enums;

public enum PaymentMethod
{
    [Description("CreditCard")]
    CreditCard = 1,
    [Description("DebitCard")]
    DebitCard = 2,
    [Description("Pix")]
    Pix = 3,
    [Description("Boleto")]
    Boleto = 4,
    [Description("Cash")]
    Cash = 5
}

