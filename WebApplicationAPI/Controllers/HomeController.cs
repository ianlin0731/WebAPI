using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.JwtServices;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ToDoListContext _context;

        public HomeController(ToDoListContext context)
        {
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] UserLoginTable model)
        {
            var ListOne = from u in _context.UserLoginTables
                                      where u.UserName == model.UserName && u.UserPassword == model.UserPassword
                                      select u;

            UserLoginTable user = ListOne.FirstOrDefault();

            if (user == null)
            {
                return NotFound(new {message="查無此人"});
            }
            else
            {
                string token = TokenService.CreateToken(user);
                return Ok(new
                {
                    message="成功",
                    user,
                    token,
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public string Get()
        {
            return "You are Anonymous（匿名登入）.  -- 透過[HttpGet]";
        }

        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Authenticated()
        {
            return String.Format("Authenticated - 您好， {0} ", User.Identity.Name);
        }

        [HttpPost]
        [Route("test")]
        [Authorize(Roles = "1")]
        public string Tester()
        {
            return "*****回家作業*****  [HomeLogin] API控制器 -- You are a Tester -- Roles = 1";
            //   ( 請參閱圖片  HomeWork-4.jpg )
        }

    }


}
