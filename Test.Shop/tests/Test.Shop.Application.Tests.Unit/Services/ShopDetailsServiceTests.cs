using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System.Linq.Expressions;
using Test.Shop.Application.DTO;
using Test.Shop.Application.Services;
using Test.Shop.Application.Validators.Interfaces;
using Test.Shop.Core.Entities;
using Test.Shop.Core.Repositories.Abstractions;

namespace Test.Shop.Application.Tests.Unit.Services
{
    public class ShopDetailsServiceTests
    {
        private readonly Fixture _fixture;
        private readonly ShopDetailsService _shopDetailsService;
        private readonly Mock<IMappedRepository<ShopDetails>> _repositoryMock;
        private readonly Mock<IShopDetailsAddValidator> _shopDetailsAddValidatorMock;
        private readonly Mock<IShopDetailsUpdateValidator> _shopDetailsUpdateValidatorMock;
        private readonly Mock<IMapper> _mapperMock;

        public ShopDetailsServiceTests()
        {
            _fixture = new Fixture();

            _repositoryMock = new Mock<IMappedRepository<ShopDetails>>();
            _shopDetailsAddValidatorMock = new Mock<IShopDetailsAddValidator>();
            _shopDetailsUpdateValidatorMock = new Mock<IShopDetailsUpdateValidator>();
            _mapperMock = new Mock<IMapper>();

            _shopDetailsService = new ShopDetailsService(
                _repositoryMock.Object,
                _shopDetailsAddValidatorMock.Object,
                _shopDetailsUpdateValidatorMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_WithData_RunOnceAndReturnCorrectData()
        {
            // Arrange
            var data = _fixture
                .Build<ShopDetails>()
                .CreateMany(1);

            var shopDetail = data.First();

            var dto = new List<ShopDetailsDto>
            {
                new()
                {
                    Id = shopDetail.Id,
                    Name = shopDetail.Name,
                    Description = shopDetail.Description,
                    CategoryId = shopDetail.CategoryId
                }
            };

            // Mock GetAllAsync
            _repositoryMock
                .Setup(x => x.GetAllAsync(default))
                .ReturnsAsync(data);

            // Mock mapper
            _mapperMock
                .Setup(x => x.Map<IEnumerable<ShopDetailsDto>>(It.IsAny<IEnumerable<ShopDetails>>()))
                .Returns(dto);

            // Act
            var result = await _shopDetailsService.GetAllAsync();

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveCount(1);
            result.First().Id.Should().Be(shopDetail.Id);
            _repositoryMock.Verify(x => x.GetAllAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithEmptyId_ReturnBadRequest()
        {
            // Arrange
            var dto = new ShopDetailsUpdateDto
            {
                Id = 0
            };

            // Mock validator method ValidateAsync
            _shopDetailsUpdateValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<ShopDetailsUpdateDto>(), default))
                .ReturnsAsync(_fixture.Build<ValidationResult>().Create());

            // Act
            var result = await _shopDetailsService.UpdateAsync(dto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequest<IEnumerable<string>>>();
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ShopDetails>(), true, default), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WithShopNotFound_ReturnNotFound()
        {
            // Arrange
            var dto = _fixture
                .Build<ShopDetailsUpdateDto>()
                .Create();

            ShopDetails? shopDetails = null;

            // Mock validator method ValidateAsync
            _shopDetailsUpdateValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<ShopDetailsUpdateDto>(), default))
                .ReturnsAsync(new ValidationResult());

            // Mock GetFirstOrDefaultAsync method
            _repositoryMock
                .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<ShopDetails, bool>>>(), default))
                .ReturnsAsync(shopDetails);

            // Act
            var result = await _shopDetailsService.UpdateAsync(dto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NotFound>();
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ShopDetails>(), true, default), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WithCorrectData_ReturnOk()
        {
            // Arrange
            var dto = _fixture
                .Build<ShopDetailsUpdateDto>()
                .Create();

            var shopDetails = new ShopDetails
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId
            };

            // Mock validator method ValidateAsync
            _shopDetailsUpdateValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<ShopDetailsUpdateDto>(), default))
                .ReturnsAsync(new ValidationResult());

            // Mock GetFirstOrDefaultAsync method
            _repositoryMock
                .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<ShopDetails, bool>>>(), default))
                .ReturnsAsync(shopDetails);

            // Act
            var result = await _shopDetailsService.UpdateAsync(dto);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Ok>();
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ShopDetails>(), true, default), Times.Once);
        }
    }
}
