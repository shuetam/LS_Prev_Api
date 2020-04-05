using Live.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Live.Mapper;
using System.Diagnostics;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using Live.Core;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Serilog;

namespace Live.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    
	[Route("api/[controller]")]
    public class SocialUserController : LiveController
    {
        private readonly  IUserRepository _userRepository;
        
        public SocialUserController (IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }


        private string GetAuthResponse(string socialUrl)
        {
            string json = "error";
            try
            {
                WebRequest request = WebRequest.Create(socialUrl); 
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse(); 
                Stream dataStream = response.GetResponseStream();   
                StreamReader reader = new StreamReader(dataStream);   
                json = reader.ReadToEnd();  
                reader.Close();
                response.Close();
                return json;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return json;
            }
        }
        
        
		[HttpPost("user")]
        public async Task<IActionResult> SocialLogin([FromBody] SocialLogin socialLogin)
        {

        string googlePattern = "^G_"; 
        string facebookPattern = "^F_";
        string error = "error";
           

        var googleReg = new Regex(googlePattern);
        var facebookReg = new Regex(facebookPattern);

            if(googleReg.IsMatch(socialLogin.Token))
            {
            ///implement google auth and login
                string googleToken = googleReg.Replace(socialLogin.Token, "");
                string url = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + googleToken;
                string userResponse =  GetAuthResponse(url);
                try 
                {
                GoogleAuth userGoogle = JsonConvert.DeserializeObject<GoogleAuth>(userResponse);
                    var user = await _userRepository.SocialLoginAsync(userGoogle.sub, userGoogle.name, socialLogin.Email, "Google");
                    return Json(user);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Log.Error($"Google login Error: {e.Message}");
                    Log.Error(e.StackTrace);
                    return Json(error);
                }
                }
                if(facebookReg.IsMatch(socialLogin.Token))
                {
                    //Console.WriteLine("Facebook!");
                ///implement facebook auth and login
                    string facebookToken = facebookReg.Replace(socialLogin.Token, "");
                    string url = "https://graph.facebook.com/me?access_token=" + facebookToken;

                    string userResponse =  GetAuthResponse(url);
                    try 
                    {
                        FacebookAuth userFacebook = JsonConvert.DeserializeObject<FacebookAuth>(userResponse);
                         var user = await _userRepository.SocialLoginAsync(userFacebook.id, userFacebook.name, socialLogin.Email, "Facebook");
                        return Json(user);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Log.Error($"Facebook login Error: {e.Message}");
                        Log.Error(e.StackTrace);
                        return Json(error);
                    }


                }

            return Json(error);

      
        }
    }
}