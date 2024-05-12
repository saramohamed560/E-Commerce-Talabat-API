using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi =true)]//Ask swagger to un document this controller
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            if (code == 401)
                return NotFound(new ApiResponse(401));
            else if (code == 404)
                return NotFound(new ApiResponse(404));
            else
                return StatusCode(code);



        }
    }
}
