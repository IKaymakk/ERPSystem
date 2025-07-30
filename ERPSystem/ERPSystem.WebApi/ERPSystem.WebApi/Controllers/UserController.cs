using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Services;
using ERPSystem.Core.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    #region Post

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        await _service.CreateUserAsync(dto);
        return Ok();
    }

    #endregion

    #region Get

    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetUsers([FromQuery] UserFilterDto filter)
    {
        var result = await _service.GetAllUsersAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _service.GetUserByIdAsync(id);
        return Ok(result);
    }

    #endregion


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
    {
        return Ok(await _service.UpdateUserAsync(id, dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
}