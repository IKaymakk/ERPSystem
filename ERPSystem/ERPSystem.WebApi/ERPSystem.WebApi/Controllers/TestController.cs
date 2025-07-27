using ERPSystem.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("test-error")]
        public IActionResult ThrowTestError()
        {
            throw new BusinessException("Test amaçlı bir hata fırlatıldı.");
        }
    }
}
