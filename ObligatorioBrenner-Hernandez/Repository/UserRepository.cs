using Business.Domain;
using RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepository()
        {
            _users = new List<User>();
        }
        public User Add(User user)
        {
            int id = _users.Any() ? _users.Max(i => i.Id) : 0;
            user.Id = id;
            this._users.Add(user);
            return user;
        }

        public void DeleteUser(int userId)
        {
            var userToDelete = _users.First(u => u.Id == userId);
            _users.Remove(userToDelete);
        }

        public bool ExistUser(string username)
        {
            return _users.Exists(u => u.Username.Equals(username));
        }

        public List<User> GetAll()
        {
            return _users;
        }

        public User GetById(int userId)
        {
            return _users.Exists(u => u.Id == userId) ? _users.First(u => u.Id == userId) : null;
        }

        public User GetByUserName(string username)
        {
            return _users.Find(u => u.Username.Equals(username));
        }

        public bool LoginUser(string username, string password)
        {
            return _users.Exists(u => u.Username.Equals(username) && u.Password.Equals(password));
        }
    }
}
