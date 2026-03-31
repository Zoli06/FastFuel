using FastFuel.Features.Admins.DTOs;
using FastFuel.Features.Admins.Entities;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.Common;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Admins.Services;

public class AdminService(
    FastFuelDbContext dbContext,
    IMapper<Admin, AdminRequestDto, AdminResponseDto> mapper,
    UserManager<User> userManager)
    : UserServiceBase<Admin, AdminRequestDto, AdminResponseDto>(dbContext, mapper, userManager)
{
    protected override DbSet<Admin> DbSet { get; } = dbContext.Admins;

    protected override DefaultRole[] DefaultRoles =>
    [
        DefaultRole.Admin
    ];
}