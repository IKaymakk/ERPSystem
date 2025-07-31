using AutoMapper;
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.User;
using ERPSystem.Core.Entities;
using ERPSystem.Core.Exceptions;
using ERPSystem.Core.Interfaces;
using ERPSystem.Core.MappingProfiles.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services;

public class UserService : GenericService<User>, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IPasswordService passwordService, IMapper mapper) : base(userRepository)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _mapper = mapper;
    }

    public async Task CreateUserAsync(CreateUserDto dto)
    {
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
            throw new BusinessException("Bu e-posta adresi zaten kullanımda.");

        if (await _userRepository.ExistsByUsernameAsync(dto.Username))
            throw new BusinessException("Bu kullanıcı adı zaten kullanımda.");

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = _passwordService.HashPassword(dto.Password);

        await _userRepository.AddAsync(user);
    }

    public async Task<PagedResultDto<UserDto>> GetAllUsersAsync(UserFilterDto filter)
    {
        var pagedUsers = await _userRepository.GetPagedUsersAsync(filter);

        return pagedUsers.ToPagedResult<User, UserDto>(_mapper);
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id, x=>x.Role);
        var mappedUser = _mapper.Map<UserDto>(user);
        return mappedUser;
    }
    public async Task<UpdateUserDto> UpdateUserAsync(int userId, UpdateUserDto dto)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
            throw new BusinessException("Kullanıcı bulunamadı.");

        if (!string.IsNullOrEmpty(dto.Username) &&
            dto.Username != existingUser.Username)
        {
            if (await _userRepository.ExistsByUsernameAsync(dto.Username, userId))
                throw new BusinessException("Bu kullanıcı adı zaten kullanımda.");
        }

        _mapper.Map(dto, existingUser);
        await _userRepository.UpdateAsync(existingUser);

        var updatedDto = _mapper.Map<UpdateUserDto>(existingUser);
        return updatedDto;
    }

}
