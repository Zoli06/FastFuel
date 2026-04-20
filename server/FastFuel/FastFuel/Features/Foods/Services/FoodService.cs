using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Foods.Services;

public class FoodService(FastFuelDbContext dbContext, IMapper<Food, FoodRequestDto, FoodResponseDto> mapper)
    : CrudService<Food, FoodRequestDto, FoodResponseDto>(dbContext, mapper)
{
    protected override DbSet<Food> DbSet { get; } = dbContext.Foods;

    protected override Create<Food, FoodRequestDto, FoodResponseDto> CreateOperation =>
        new Create(DbContext, DbSet, Mapper);

    protected override Update<Food, FoodRequestDto, FoodResponseDto> UpdateOperation =>
        new Update(DbContext, DbSet, Mapper);

    private class Create(
        FastFuelDbContext dbContext,
        DbSet<Food> dbSet,
        IMapper<Food, FoodRequestDto, FoodResponseDto> mapper)
        : Create<Food, FoodRequestDto, FoodResponseDto>(dbContext, dbSet, mapper)
    {
        protected override Task SaveEntityAsync(FoodRequestDto requestDto, Food entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            return entity.Price < 0
                ? throw new ValidationAppException("Price cannot be negative.")
                : base.SaveEntityAsync(requestDto, entity, userId, cancellationToken);
        }
    }

    private class Update(
        FastFuelDbContext dbContext,
        DbSet<Food> dbSet,
        IMapper<Food, FoodRequestDto, FoodResponseDto> mapper)
        : Update<Food, FoodRequestDto, FoodResponseDto>(dbContext, dbSet, mapper)
    {
        protected override Task SaveEntityAsync(uint id, FoodRequestDto requestDto, Food entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            return entity.Price < 0
                ? throw new ValidationAppException("Price cannot be negative.")
                : base.SaveEntityAsync(id, requestDto, entity, userId, cancellationToken);
        }
    }
}