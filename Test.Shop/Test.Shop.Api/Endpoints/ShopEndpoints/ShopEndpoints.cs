using Microsoft.AspNetCore.Mvc;
using Test.Shop.Application.DTO;
using Test.Shop.Application.Services.Interfaces;

namespace Test.Shop.Api.Endpoints.ShopEndpoints
{
    internal static class ShopEndpoints
    {
      public static IEndpointRouteBuilder MapShopEndpoints(this IEndpointRouteBuilder app)
      {
            var group = app.MapGroup("Shops");

            group.MapGet("/", async (IShopDetailsService shopDetailService) =>
            {
                return Results.Ok(await shopDetailService.GetAllAsync());
            })
            .WithName("Shops")
            .WithDescription("Get all shops")
            .WithOpenApi();

            group.MapGet("/active", async (IShopDetailsService shopDetailService) =>
            {
                return Results.Ok(await shopDetailService.GetActiveAsync());
            })
           .WithName("ActiveShops")
           .WithDescription("Get active shops")
           .WithOpenApi();

            group.MapPut("/", async (
                [FromBody] ShopDetailsUpdateDto shopDetail,
                IShopDetailsService shopDetailService) =>
            {
                return await shopDetailService.UpdateAsync(shopDetail);
            })
           .WithName("UpdateShop")
           .WithDescription("Update shop details")
           .WithOpenApi()
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status404NotFound)
           .Produces(StatusCodes.Status400BadRequest);

            group.MapPost("/", async (
                [FromBody] ShopDetailsAddDto shopDetail,
                IShopDetailsService shopDetailService) =>
            {
                return await shopDetailService.AddAsync(shopDetail);
            })
           .WithName("NewShop")
           .WithDescription("Add new shop details")
           .WithOpenApi()
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status404NotFound)
           .Produces(StatusCodes.Status400BadRequest);

            group.MapDelete("/", async (
               [FromBody] int shopId,
               IShopDetailsService shopDetailService) =>
            {
                return await shopDetailService.DeleteAsync(shopId);
            })
          .WithName("Delete")
          .WithDescription("Delete shop")
          .WithOpenApi()
          .Produces(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status204NoContent)
          .Produces(StatusCodes.Status404NotFound)
          .Produces(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
