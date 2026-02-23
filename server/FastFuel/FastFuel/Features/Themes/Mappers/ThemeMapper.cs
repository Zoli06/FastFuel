using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Themes.DTOs;
using FastFuel.Features.Themes.Entities;

namespace FastFuel.Features.Themes.Mappers;

public class ThemeMapper : IMapper<Theme, ThemeRequestDto, ThemeResponseDto>
{
    public ThemeResponseDto ToDto(Theme entity)
    {
        return new ThemeResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Background = entity.Background,
            Footer = entity.Footer,
            ButtonPrimary = entity.ButtonPrimary,
            ButtonSecondary = entity.ButtonSecondary
        };
    }

    public Theme ToEntity(ThemeRequestDto dto)
    {
        return new Theme
        {
            Name = dto.Name,
            Background = dto.Background,
            Footer = dto.Footer,
            ButtonPrimary = dto.ButtonPrimary,
            ButtonSecondary = dto.ButtonSecondary
        };
    }

    public void UpdateEntity(ThemeRequestDto dto, Theme entity)
    {
        entity.Name = dto.Name;
        entity.Background = dto.Background;
        entity.Footer = dto.Footer;
        entity.ButtonPrimary = dto.ButtonPrimary;
        entity.ButtonSecondary = dto.ButtonSecondary;
    }
}