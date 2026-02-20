using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Models;

namespace FastFuel.Features.Users.Mappers;

public class UserMapper : IMapper<User, UserRequestDto, UserResponseDto>
{
    public UserResponseDto ToDto(User model)
    {
        return new UserResponseDto
        {
            Id = model.Id,
            Name = model.UserName,
            Email = model.Email,
            UserName = model.UserName,
            ThemeId = model.ThemeId
        };
    }

    public User ToModel(UserRequestDto dto)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            UserName = dto.UserName,
            ThemeId = dto.ThemeId
        };
    }

    public void UpdateModel(UserRequestDto dto, ref User model)
    {
        model.Name = dto.Name;
        model.Email = dto.Email;
        model.UserName = dto.UserName;
        model.ThemeId = dto.ThemeId;
    }
}