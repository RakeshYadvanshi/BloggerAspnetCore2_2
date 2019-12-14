using BloggerAPI.DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAPI.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        IEnumerable<User> GetUserById();
        User Add(User User);
        User Update(User User);
        bool Delete(User User);
    }
}
