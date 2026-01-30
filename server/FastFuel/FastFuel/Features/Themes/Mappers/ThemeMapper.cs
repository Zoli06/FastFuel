using FastFuel.Features.Common;
using FastFuel.Features.Themes.DTOs;
using FastFuel.Features.Themes.Models;

namespace FastFuel.Features.Themes.Mappers;

public class  ThemeMapper : Mapper<Theme, ThemeRequestDto, ThemeResponseDto>
{
    public override ThemeResponseDto ToDto(Theme model)
    {
        return new ThemeResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Background = model.Background,
            Footer = model.Footer,
            ButtonPrimary = model.ButtonPrimary,
            ButtonSecondary = model.ButtonSecondary
        };
    }

    public override Theme ToModel(ThemeRequestDto dto)
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

    public override void UpdateModel(ThemeRequestDto dto, ref Theme model)
    {
        model.Name = dto.Name;
        model.Background = dto.Background;
        model.Footer = dto.Footer;
        model.ButtonPrimary = dto.ButtonPrimary;
        model.ButtonSecondary = dto.ButtonSecondary;
    }
}