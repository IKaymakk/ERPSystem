// API/Controllers/RoleController.cs
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Tüm role işlemleri için authentication gerekli
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Yeni rol oluşturur
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleDto dto)
        {
            var result = await _roleService.CreateRoleAsync(dto);
            return Ok(ApiResponse<RoleDto>.Success(result, "Rol başarıyla oluşturuldu."));
        }

        /// <summary>
        /// Sayfalı rol listesini getirir
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResultDto<RoleDto>>>> GetRoles([FromQuery] RoleFilterDto filter)
        {
            var result = await _roleService.GetAllRolesAsync(filter);
            return Ok(ApiResponse<PagedResultDto<RoleDto>>.Success(result));
        }

        /// <summary>
        /// Dropdown için tüm rolleri getirir
        /// </summary>
        [HttpGet("dropdown")]
        public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetRolesForDropdown()
        {
            var result = await _roleService.GetAllRolesForDropdownAsync();
            return Ok(ApiResponse<List<RoleDto>>.Success(result));
        }

        /// <summary>
        /// ID'ye göre rol getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleById(int id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            return Ok(ApiResponse<RoleDto>.Success(result));
        }

        /// <summary>
        /// Rol ve bu role sahip kullanıcıları getirir
        /// </summary>
        [HttpGet("{id}/with-users")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleWithUsers(int id)
        {
            var result = await _roleService.GetRoleWithUsersAsync(id);
            return Ok(ApiResponse<RoleDto>.Success(result));
        }

        /// <summary>
        /// Rolü günceller
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(int id, [FromBody] UpdateRoleDto dto)
        {
            var result = await _roleService.UpdateRoleAsync(id, dto);
            return Ok(ApiResponse<RoleDto>.Success(result, "Rol başarıyla güncellendi."));
        }

        /// <summary>
        /// Rolü siler (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return Ok(ApiResponse<object>.Success(null, "Rol başarıyla silindi."));
        }

        /// <summary>
        /// Kullanıcıya rol atar
        /// </summary>
        [HttpPost("assign")]
        public async Task<ActionResult<ApiResponse<object>>> AssignRoleToUser([FromBody] AssignRoleDto dto)
        {
            await _roleService.AssignRoleToUserAsync(dto);
            return Ok(ApiResponse<object>.Success(null, "Rol başarıyla kullanıcıya atandı."));
        }
    }
}