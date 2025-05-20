namespace Test.Shop.Core.Entities
{
    /// <summary>
    /// Shop category
    /// </summary>
    public class ShopCategory
    {
        public int IdShopCategory { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set;  }
    }
}
