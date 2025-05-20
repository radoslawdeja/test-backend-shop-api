using AutoMapper;
using Microsoft.AspNetCore.Http;
using Test.Shop.Application.DTO;
using Test.Shop.Application.Services.Interfaces;
using Test.Shop.Application.Validators.Interfaces;
using Test.Shop.Core.Entities;
using Test.Shop.Core.Repositories.Abstractions;

namespace Test.Shop.Application.Services
{
    public sealed class ShopDetailsService(
        IMappedRepository<ShopDetails> repository,
        IShopDetailsAddValidator shopAddValidator,
        IShopDetailsUpdateValidator shopUpdateValidator,
        IMapper mapper) : IShopDetailsService
    {
        /// <summary>
        /// This method get all shops.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ShopDetailsDto>> GetAllAsync()
        {
            return mapper.Map<IEnumerable<ShopDetailsDto>>(await repository.GetAllAsync());
        }

        /// <summary>
        /// This method get only active shops.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ShopDetailsDto>> GetActiveAsync()
        {
            return await repository.GetByExpressionAsync<ShopDetailsDto>(x => x.IsActive);
        }

        /// <summary>
        /// This method update shop (Name, Description, Category).
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IResult> UpdateAsync(ShopDetailsUpdateDto dto)
        {
            // Validate dto
            var validateResult = await shopUpdateValidator.ValidateAsync(dto);
            if (!validateResult.IsValid)
            {
                return Results.BadRequest(validateResult.Errors.Select(x => x.ErrorMessage).Distinct());
            }

            // Get shop by id
            var shopDetail = await repository.GetFirstOrDefaultAsync(x => x.Id == dto.Id);

            // If shop not found
            if (shopDetail is null)
            {
                return Results.NotFound();
            }

            // Map changes into shopDetail
            mapper.Map(dto, shopDetail);

            shopDetail.ModifiedById = 0;
            shopDetail.ModifiedDate = DateTime.Now;

            await repository.UpdateAsync(shopDetail);

            return Results.Ok();
        }

        /// <summary>
        /// This method add new shop.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IResult> AddAsync(ShopDetailsAddDto dto)
        {
            // Validate dto
            var validateResult = await shopAddValidator.ValidateAsync(dto);
            if (!validateResult.IsValid)
            {
                return Results.BadRequest(validateResult.Errors.Select(x => x.ErrorMessage).Distinct());
            }

            // Map dto to ShopDetails
            var shopDetail = mapper.Map<ShopDetails>(dto);
            shopDetail.CreatedById = 0;

            await repository.AddAsync(shopDetail);

            return Results.Ok();
        }

        /// <summary>
        /// This method delete shop
        /// </summary>
        /// <param name="idShop"></param>
        /// <returns></returns>
        public async Task<IResult> DeleteAsync(int idShop)
        {
            if (idShop <= 0)
            {
                return Results.NoContent();
            }

            // Check whether shop not exists
            if (!await repository.ExistsAsync(x => x.Id == idShop))
            {
                return Results.NotFound(idShop);
            }

            await repository.DeleteByExpression(x => x.Id == idShop);

            return Results.Ok();
        }
    }
}
