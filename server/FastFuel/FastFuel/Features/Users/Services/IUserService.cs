using FastFuel.Features.Common.Services;
using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Users.Services;

public interface IUserService<in TUserRequestDto, TUserResponseDto> : ICrudService<TUserRequestDto, TUserResponseDto>
    where TUserRequestDto : UserRequestDto
    where TUserResponseDto : UserResponseDto;