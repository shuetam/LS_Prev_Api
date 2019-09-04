using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Live.Core;
using Microsoft.AspNetCore.Mvc;

namespace Live.Repositories
{
    public interface IUserRepository
    { 
          Task <List<UserDto>> GetAllAsync();
          Task <UserDto> SocialLoginAsync(string userId, string name, string email, string authType);
          
          // facebook: F_
          // Goole: G_

         // https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=

         // https://graph.facebook.com/me?access_token=
          
     }

}