using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloggerAPI.Services
{
    public class UserService : IUserService, IDisposable
    {
        private BloggerDbContext _dbContext;
        private IMapper _mapper;

        public UserService(BloggerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<User> Add(User User)
        {
            _dbContext.Users.Add(User);
            await _dbContext.SaveChangesAsync();
            return User;
        }

        public async Task<bool> Delete(User user)
        {
            if (user != null)
            {
                var dbUser = _dbContext.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
                if (dbUser != null)
                {
                    _dbContext.Users.Remove(user);
                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    throw new NotSupportedException("user does not exists in our system");

                }
            }
            else
            {
                throw new ArgumentNullException(nameof(user));
            }
            return false;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> Update(User user)
        {
            if (user != null)
            {
                var dbUser = await _dbContext.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
                if (dbUser != null)
                {
                    _mapper.Map(user, dbUser);
                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        return dbUser;
                    }
                    else
                    {
                        throw new Exception("Unexpected Exception !! concern developer");
                    }
                }
                else
                {
                    throw new NotSupportedException("user does not exists in our system");

                }
            }
            else
            {
                throw new ArgumentNullException(nameof(user));
            }

        }
    }
}
