using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Role;
using ERPSystem.Core.Entities;
using ERPSystem.Core.Exceptions;
using ERPSystem.Core.Interfaces;

namespace ERPSystem.Application.Services;

public class RoleService : GenericService<Role>, IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository roleRepository, IUserRepository userRepository, IMapper mapper) : base(roleRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
    {
        if (await _roleRepository.ExistsByNameAsync(dto.Name))
            throw new BusinessException("Bu Rol Adı Zaten Kullanımda");

        var role = _mapper.Map<Role>(dto);
        var createdRole = await _roleRepository.AddAsync(role);

        return _mapper.Map<RoleDto>(createdRole);
    }

    public async Task<PagedResultDto<RoleDto>> GetAllRolesAsync(RoleFilterDto filter)
    {
        var roles = await _roleRepository.GetPagedRolesAsync(filter);
        var roleDtos = _mapper.Map<List<RoleDto>>(roles.Items); // ✅ Items property'sini map et
        return new PagedResultDto<RoleDto>
        {
            Items = roleDtos,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = filter.PageSize
        };
    }

    public async Task<RoleDto> GetRoleByIdAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id, x => x.Users);
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(int roleId, UpdateRoleDto dto)
    {
        var existingRole = await _roleRepository.GetByIdAsync(roleId, x => x.Users);

        if (dto.Name != existingRole.Name && await _roleRepository.ExistsByNameAsync(dto.Name, roleId))
            throw new BusinessException("Bu Rol Adı Zaten Kullanımda");

        _mapper.Map(dto, existingRole);
        await _roleRepository.UpdateAsync(existingRole);

        return _mapper.Map<RoleDto>(existingRole);
    }

    public async Task DeleteRoleAsync(int roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);

        if (await _roleRepository.IsRoleAssignedToUsersAsync(roleId))
            throw new BusinessException("Bu rol kullanıcılara atanmış durumda. Önce kullanıcıların rollerini değiştirin.");

        await _roleRepository.DeleteAsync(roleId);
    }

    public async Task<List<RoleDto>> GetAllRolesForDropdownAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return _mapper.Map<List<RoleDto>>(roles.OrderBy(x => x.Name));
    }

    public async Task AssignRoleToUserAsync(AssignRoleDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user is null)
            throw new BusinessException("Kullanıcı Bulunamadı");

        var role = await _roleRepository.GetByIdAsync(dto.RoleId);
        if (role is null)
            throw new BusinessException("Rol Bulunamadı");

        user.RoleId = dto.RoleId;
        await _userRepository.UpdateAsync(user);
    }

    public async Task<RoleDto> GetRoleWithUsersAsync(int roleId)
    {
        var role = await _roleRepository.GetRoleWithUsersAsync(roleId);
        return _mapper.Map<RoleDto>(role);
    }

}
