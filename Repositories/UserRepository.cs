using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Live.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Live.Repositories
{
    public class UserRepository : IUserRepository
    { 

        private readonly LiveContext _liveContext;
        private readonly IMapper _autoMapper;

        public UserRepository(LiveContext liveContext, IMapper autoMapper )
        {
            this._liveContext = liveContext;
            this._autoMapper = autoMapper;
        }
        
        Task<List<UserDto>> IUserRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }


        public async Task<UserDto> SocialLoginAsync(string userId, string name, string email, string authType)
        {
            User user = _liveContext.Users.FirstOrDefault(x => x.UserEmail == email && x.UserSocialId == userId && x.IsActive);


            if(user == null)
            {
                Console.WriteLine("Save user to database");
                User newUser = new User(userId, name, email, authType);
                await _liveContext.AddAsync(newUser);
                await _liveContext.SaveChangesAsync();
                var userDto = _autoMapper.Map<UserDto>(newUser);
                return userDto;
            }
            else 
            {
                //Console.WriteLine($"login existing user:  {user.UserYoutubes.Count}");
                //user.UserYoutubes.Add(new UserYoutube("dVVsa","dVVVas","dVVfs","gdVVV"));
                user.NextLogin();
                _liveContext.Update(user);
                await _liveContext.SaveChangesAsync();
                
            }
           
            return _autoMapper.Map<UserDto>(user);
        }
    }

}