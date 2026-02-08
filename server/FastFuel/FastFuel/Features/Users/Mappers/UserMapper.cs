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
            Name = model.Name,
            Email = model.Email,
            Username = model.Username,
            ThemeId = model.Theme.Id
        };
    }

    public User ToModel(UserRequestDto dto)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Username = dto.Username,
            ThemeId = dto.ThemeId
        };
    }

    public void UpdateModel(UserRequestDto dto, ref User model)
    {
        model.Name = dto.Name;
        model.ThemeId = dto.ThemeId;
        model.Username = dto.Username;
        model.Email = dto.Email;
    }
}