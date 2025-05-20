using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Test.Shop.Core.Entities;
using Test.Shop.Infrastructure.DAL;
using Test.Shop.Infrastructure.DAL.Repositories;

namespace Test.Shop.Infrastructure.Tests.Unit.DAL.Repositories
{
    public class RepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            using var dbContext = await GetDbContext();
            var repository = new Repository<ShopDetails>(dbContext);

            // Act
            var enitities = await repository.GetAllAsync();

            // Assert
            enitities.Should().NotBeNull().And.HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnPagedEntities()
        {
            // Arrange
            using var dbContext = await GetDbContext();
            var repository = new Repository<ShopDetails>(dbContext);
            var page = 1;
            var pageSize = 10;

            // Act
            var entity = await repository.GetPagedAsync(page, pageSize);

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateRangeAsync_ShouldUpdateEntitiesInDatabase()
        {
            // Arrange
            var entityId = 3;

            using var dbContext = await GetDbContext();
            var repository = new Repository<ShopDetails>(dbContext);

            var existingEntity = new ShopDetails() { Id = entityId, Name = "Shop test", Description = "Descritpion" };
            dbContext.Set<ShopDetails>().Add(existingEntity);
            await dbContext.SaveChangesAsync();

            // Act
            await repository.UpdateRangeAsync([existingEntity]);
            var updatedEntities = await dbContext.Set<ShopDetails>().ToListAsync();
            var entity = updatedEntities?.Find(x => x.Id == entityId);

            // Assert
            updatedEntities.Should().NotBeNull().And.HaveCountGreaterThan(0);
            entity.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByConditionAsync_WithPredicateAndSelector_ShouldReturnData()
        {
            // Arrange
            using var dbContext = await GetDbContext();
            var repository = new Repository<ShopDetails>(dbContext);

            // Act
            var result = await repository.GetByConditionAsync(x => x.CreatedDate > DateTime.MinValue, x => x.Id, true);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CountGroupsAsync_WithConditions_ShouldReturnData()
        {
            // Arrange
            using var dbContext = await GetDbContext();
            var repository = new Repository<ShopDetails>(dbContext);

            // Act
            var result = await repository.CountGroupsAsync(x => x.CategoryId, x => x.Name.ToLower().Contains("shop", StringComparison.OrdinalIgnoreCase), x => x.Any());

            // Assert
            result.Should().BeGreaterThan(0);
        }

        [ExcludeFromCodeCoverage]
        private static async Task<ShopDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ShopDbContext(options);

            var shopCategories = new List<ShopCategory>
            {
                new() { IdShopCategory = 1, Name = "Category 1", IsActive = true },
                new() { IdShopCategory = 2, Name = "Category 2", IsActive = true }
            };

            var shopDetails = new List<ShopDetails>
            {
                new() { Id = 1, Name = "Shop 1", Description = "Shop 1 description", CategoryId = 1 },
                new() { Id = 2, Name = "Shop 2", Description = "Shop 2 description", CategoryId = 2 }
            };

            await dbContext.Set<ShopCategory>().AddRangeAsync(shopCategories);
            await dbContext.Set<ShopDetails>().AddRangeAsync(shopDetails);
            await dbContext.SaveChangesAsync();
            return dbContext;
        }
    }
}
