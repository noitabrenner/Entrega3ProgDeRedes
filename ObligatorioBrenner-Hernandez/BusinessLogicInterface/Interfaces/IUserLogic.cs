using Business.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicInterface.Interfaces
{
    public interface IUserLogic
    {
        string AddUser(User user);
        User GetByUserName(string userName);
        User GetById(int userId);
        string UpdateUser(int userId, User user);
        string DeleteUser(int id);
        List<User> GetAllUsers();
    }
}
