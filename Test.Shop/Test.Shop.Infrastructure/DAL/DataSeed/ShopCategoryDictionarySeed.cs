using Test.Shop.Core.Entities;

namespace Test.Shop.Infrastructure.DAL.DataSeed
{
    /// <summary>
    /// Shop category data seed
    /// </summary>
    public static class ShopCategoryDictionarySeed
    {
        public static readonly List<ShopCategory> CategoryDictionary =
        [
            new ShopCategory { Name = "Testowy sklep nr 1" },
            new ShopCategory { Name = "Testowy sklep nr 2" },
            new ShopCategory { Name = "Testowy sklep nr 3" },
            new ShopCategory { Name = "Testowy sklep nr 4" }
        ];
    }
}
