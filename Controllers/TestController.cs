using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Web.Http.Cors;
using Live.Repositories;
using Live.Core;
using Live.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Live.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
	[Route("api/[controller]")]

    public class TestController : LiveController
    {
        private readonly  IUserRepository _userRepository;
         private readonly  IUserDesktopRepository _desktopRepository;
         private readonly IJwtService _jwtService;
       
         public TestController (IUserRepository userRepository, IUserDesktopRepository de, IJwtService jwtService)
        {
            this._userRepository = userRepository;
            this._desktopRepository = de;
          
             this._jwtService = jwtService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken()
        {

            var user = await _userRepository.SocialLoginAsync("109822981168617016570", "", "mateuszbieda.mb@gmail.com", "Google");
            var JwtToken = _jwtService.CreateToken(new Guid(user.UserId), "user");
            
            return Json(JwtToken);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("get")]
        public async Task<IActionResult> GetIcons()
        {
            
            var icons = await _desktopRepository.GetAllIconsForUserAsync(this.UserId, "");
            return Json(icons);
        }


    }
}
