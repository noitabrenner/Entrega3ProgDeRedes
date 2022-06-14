using Business.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryInterface
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User Add(User user);
        User GetById(int userId);
        User GetByUserName(string username);
        Boolean ExistUser(string username);
        void DeleteUser(int id);
        bool LoginUser(string username, string password);
    }
}
