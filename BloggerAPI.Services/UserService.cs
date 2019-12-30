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
    public class UserService : IUserService
    {
        private readonly IBloggerDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserService(IBloggerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<User> Add(User user)
        {
            if (user != null)
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }

            throw new ArgumentNullException(nameof(user));

        }

        public async Task<bool> Delete(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(User));


            var dbUser = await _dbContext.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
            if (dbUser == null)
                throw new NotSupportedException($"{nameof(User)} does not exists in our system");


            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;

        }


        public async Task<User> GetUserById(int userId)
        {
            return await _dbContext.Users.Where(x => x.Id == userId)
                .AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> Update(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(User));

            var dbUser = await _dbContext.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
            if (dbUser != null)
            {
                _mapper.Map(user, dbUser);
                await _dbContext.SaveChangesAsync();
                return dbUser;
            }

            throw new NotSupportedException($"{nameof(User)} does not exists in our system");

        }
    }
}
