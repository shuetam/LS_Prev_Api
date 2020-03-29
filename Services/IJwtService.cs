
using System;
using Live.Core;

namespace Live.Services
{
public interface IJwtService
{
     JwtDto CreateToken (Guid UserId, string role);
}

}