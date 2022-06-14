using Business.Domain;
using BusinessLogicInterface.Interfaces;
using RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Logic
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserRepository _userRepository;

        public UserLogic (IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public string AddUser(User user)
        {
          //agregar lock
                if (_userRepository.ExistUser(user.Username))
                {
                return "Nombre de usuario repetido";  
                }
                else
                {
                _userRepository.Add(user);
                }    
            return "el usuario: " + user.Username + " fue ingresado correctamente";
        }


        public string DeleteUser(int id)
        {
            User user = GetById(id);
            if (user == null)
            {
                return $"no se encontro usuario con el username: {user.Username}";
            }

            _userRepository.DeleteUser(id);
            return user.Username + " ha sido eliminado correctamente";
        }

        public List<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public User GetById(int userId)
        {
            return _userRepository.GetById(userId);
        }

        public User GetByUserName(string userName)
        {
            return _userRepository.GetByUserName(userName);
        }

        public string UpdateUser(int id, User user)
        {
            User userToModify = _userRepository.GetById(id);
            if (userToModify == null)
            {
                return "no se encontro usuario a modificar";
            }
            userToModify.Username = user.Username;
            userToModify.Name = user.Name;
            userToModify.Password = user.Password;

            return "el usuario " + id + "fue modificado con exito";
        }
    }
}
