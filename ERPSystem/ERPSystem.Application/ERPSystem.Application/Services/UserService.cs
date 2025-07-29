using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.User;
using ERPSystem.Core.Entities;
using ERPSystem.Core.Exceptions;
using ERPSystem.Core.Interfaces;

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
}
