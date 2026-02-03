using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Models;

namespace FastFuel.Features.Users.Mappers;

public class UserMapper : Mapper<User, UserRequestDto, UserResponseDto>
{
    public override UserResponseDto ToDto(User model)
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

    public override User ToModel(UserRequestDto dto)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Username = dto.Username,
            ThemeId = dto.ThemeId
        };
    }

    public override void UpdateModel(UserRequestDto dto, ref User model)
    {
        model.Name = dto.Name;
        model.ThemeId = dto.ThemeId;
        model.Username = dto.Username;
        model.Email = dto.Email;
    }
}