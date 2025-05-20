using Microsoft.AspNetCore.Http;
using Test.Shop.Application.DTO;

namespace Test.Shop.Application.Services.Interfaces
{
    public interface IShopDetailsService
    {
        /// <summary>
        /// This method get all shops.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ShopDetailsDto>> GetAllAsync();

        /// <summary>
        /// This method get only active shops.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ShopDetailsDto>> GetActiveAsync();

        /// <summary>
        /// This method update shop (Name, Description, Category).
        /// </summary>
        /// <returns></returns>
        Task<IResult> UpdateAsync(ShopDetailsUpdateDto dto);

        /// <summary>
        /// This method add new shop.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<IResult> AddAsync(ShopDetailsAddDto dto);

        /// <summary>
        /// This method delete shop
        /// </summary>
        /// <param name="idShop"></param>
        /// <returns></returns>
        Task<IResult> DeleteAsync(int idShop);
    }
}
