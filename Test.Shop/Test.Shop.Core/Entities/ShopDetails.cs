namespace Test.Shop.Core.Entities
{
    /// <summary>
    /// Shop details
    /// </summary>
    public class ShopDetails
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public ShopCategory ShopCategory { get; set; } = null!;
    }
}
