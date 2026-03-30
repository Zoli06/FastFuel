using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Machines.DTOs;
using FastFuel.Features.Machines.Entities;
using FastFuel.Features.Roles.Common;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Machines.Services;

public class MachineService(
    ApplicationDbContext dbContext,
    IMapper<Machine, MachineRequestDto, MachineResponseDto> mapper,
    UserManager<User> userManager)
    : UserServiceBase<Machine, MachineRequestDto, MachineResponseDto>(dbContext, mapper, userManager)
{
    protected override DbSet<Machine> DbSet { get; } = dbContext.Machines;

    protected override DefaultRole[] DefaultRoles =>
    [
        DefaultRole.Machine
    ];
}