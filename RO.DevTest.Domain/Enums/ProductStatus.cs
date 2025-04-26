using System.ComponentModel;

namespace RO.DevTest.Domain.Enums
{
    public enum ProductStatus
    {
        [Description("Available")]
        Available = 1,
        [Description("OutOfStock")]
        OutOfStock = 2,
        [Description("Discontinued")]
        Discontinued = 3
    }
}