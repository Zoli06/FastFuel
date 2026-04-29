using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Menus.Services;

public class MenuService(FastFuelDbContext dbContext, IMapper<Menu, MenuRequestDto, MenuResponseDto> mapper)
    : CrudService<Menu, MenuRequestDto, MenuResponseDto>(dbContext, mapper)
{
    protected override DbSet<Menu> DbSet { get; } = dbContext.Menus;

    protected override Create<Menu, MenuRequestDto, MenuResponseDto> CreateOperation =>
        new Create(DbContext, DbSet, Mapper);

    protected override Update<Menu, MenuRequestDto, MenuResponseDto> UpdateOperation =>
        new Update(DbContext, DbSet, Mapper);

    private class Create(
        FastFuelDbContext dbContext,
        DbSet<Menu> dbSet,
        IMapper<Menu, MenuRequestDto, MenuResponseDto> mapper)
        : Create<Menu, MenuRequestDto, MenuResponseDto>(dbContext, dbSet, mapper)
    {
        protected override Task SaveEntityAsync(MenuRequestDto requestDto, Menu entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            return entity.Price < 0
                ? throw new ValidationAppException("Price cannot be negative.")
                : base.SaveEntityAsync(requestDto, entity, userId, cancellationToken);
        }
    }

    private class Update(
        FastFuelDbContext dbContext,
        DbSet<Menu> dbSet,
        IMapper<Menu, MenuRequestDto, MenuResponseDto> mapper)
        : Update<Menu, MenuRequestDto, MenuResponseDto>(dbContext, dbSet, mapper)
    {
        protected override Task SaveEntityAsync(uint id, MenuRequestDto requestDto, Menu entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            return entity.Price < 0
                ? throw new ValidationAppException("Price cannot be negative.")
                : base.SaveEntityAsync(id, requestDto, entity, userId, cancellationToken);
        }
    }
}