using System.ComponentModel;

namespace RO.DevTest.Domain.Enums;

public enum SaleStatus
{
    [Description("Pending")]
    Pending = 1,
    [Description("Completed")]
    Completed = 2,
    [Description("Canceled")]
    Canceled = 3,
    [Description("Refunded")]
    Refunded = 4
}

