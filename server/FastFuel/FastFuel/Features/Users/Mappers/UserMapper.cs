using FastFuel.Features.Common;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Models;
using FastFuel.Features.Themes.Mappers;

namespace FastFuel.Features.Users.Mappers;

public class UserMapper : Mapper<User, UserRequestDto, UserResponseDto>
{
    private readonly ThemeMapper _themeMapper = new ThemeMapper();

    public override UserResponseDto ToDto(User model)
    {
        return new UserResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Theme = _themeMapper.ToDto(model.Theme)
        };
    }

    public override User ToModel(UserRequestDto dto)
    {
        return new User
        {
            Name = dto.Name,
            Theme = _themeMapper.ToModel(dto.Theme)
        };
    }

    public override void UpdateModel(UserRequestDto dto, ref User model)
    {
        model.Name = dto.Name;
        model.Theme = _themeMapper.ToModel(dto.Theme);
    }
}