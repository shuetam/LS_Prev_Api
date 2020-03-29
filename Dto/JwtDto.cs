using System;
namespace Live.Core 
{
    public class JwtDto
    {
        public string Token {get; set;}
        public DateTime Expire {get; set;}
        public string Roles {get; set;}
    }

}