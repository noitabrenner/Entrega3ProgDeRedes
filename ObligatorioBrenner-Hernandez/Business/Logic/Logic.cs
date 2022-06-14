using Business.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Business.Logic
{
    public class Logic
    {
        private Logic()
        { }

        private static Logic instance = null;

        public static Logic Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logic();
                }
                return instance;
            }
        }

        private static object userLock = new object();
        public static List<User> usersList = new List<User>();
        public static int totalChips = 0;

        public string AddUser(User user)
        {
            lock (userLock)
            {
                if (ExistUser(user.Username))
                {
                    return "Nombre de usuario repetido.";
                }
                else
                {
                    usersList.Add(user);
                }
            }
            return "el usuario: " + user.Username + " fue ingresado correctamente";
        }

        public string Login(Sesion sesion)
        {
            string response = "";

            if (usersList.Count() == 0)
            {
                response = "No hay usuarios en el sistema";
            }

            foreach (User user in usersList)
            {
                if (user.Username == sesion.Username && user.Password == sesion.Password)
                {
                    response = checkRestrictions(user);
                    if (response == "Habilitado")
                    {
                        user.ActiveAccount = 1;
                        user.LoggedInTimes.Add(DateTime.Now);
                        return response = sesion.Username + " bienvenido al sistema";
                    }
                    else
                    {
                        return response;
                    }
                }
                else
                {
                    response = "datos incorrectos";
                }
            }
            return response;
        }

        private string checkRestrictions(User user)
        {
            string response = "";
            if (!user.Access)
            {
                response = user.Username + " Usuario bloqueado";
            }
            else
            {
                if (user.ActiveAccount > 0)
                {
                    response = " Esta cuenta ya esta siendo utilizada";
                }
                else
                {
                    response = "Habilitado";
                }
            }
            return response;
        }

        public string LogOut(User user)
        {
            string response = user.Username + " ha cerrado sesion";
            user.ActiveAccount = 0;
            return response;
        }

        public string searchUsers(string username)
        {
            string usersSearch = "";
            foreach (User user in usersList)
            {
                if (user.Username.Contains(username) || user.Name.Contains(username))
                {
                    usersSearch += " usuario: " + user.Username + " nombre: " + user.Name + "\n";
                }
            }
            if (usersSearch == "")
            {
                usersSearch = "No hay busquedas encontradas";
            }
            return usersSearch;
        }

        public string BlockUser(string userToBlock)
        {
            String blockedUser = " \n no se encontro usuario";
            foreach (User user in usersList)
            {
                if (user.Username == userToBlock)
                {
                    user.Access = false;
                    return blockedUser = user.Username + " bloqueado exitosamente";
                }
                else
                {
                    blockedUser = "usuario no encontrado";
                }
            }
            return blockedUser;
        }

        public string unlockUser(string userToUnlock)
        {
            String response = " \n no se encontro usuario";
            foreach (User user in usersList)
            {
                if (user.Username == userToUnlock)
                {
                    if (!user.Access)
                    {
                        response = user.Username + " desbloqueado exitosamente";
                        user.Access = true;
                    }
                    else
                    {
                        response = user.Username + " no estaba bloqueado anteriormente";
                    }
                }
                else
                {
                    response = " usuario no encontrado";
                }
            }
            return response;
        }

        public User getUserByUsername(string username)
        {
            if (usersList.Count() == 0) { return null; }
            foreach (User user in usersList)
            {
                if (user.Username == username)
                {
                    return user;
                }
            }
            return null;
        }

        public User getUserByRealName(string name)
        {
            if (usersList.Count() == 0) { return null; }
            foreach (User user in usersList)
            {
                if (user.Name == name)
                {
                    return user;
                }
            }
            return null;
        }

        public string ShowNotifications(User user)
        {
            if (user == null) { return "No se pueden ver las notificaciones sin estar logueado"; }
            string notifications = "";
            if (user.Notifications.Count == 0)
            {
                return "no hay notificaciones para mostrar";
            }
            else
            {
                foreach (string notification in user.Notifications)
                {
                    notifications += notification + "\n";
                }
            }
            return notifications;
        }

        private void NotifyUsers(Chip chip, User user)
        {
            if (user.FollowersUsers.Count > 0)
            {
                foreach (User userFollower in user.FollowersUsers)
                {
                    userFollower.Notifications.Add(user.Username + " creo un chip: " + chip.ChipMessage);
                }
            }
        }

        public string GetAllUsers()
        {
            lock (userLock)
            {
                string data = "";
                if (usersList.Count == 0)
                {
                    data = "No hay usuarios en el sistema";
                }
                else
                {
                    data = "Lista de usuarios: \n";
                }
                foreach (User user in usersList)
                {
                    data += " Nombre de Usuario:" + user.Username + ", Nombre Real: " + user.Name + ", Cantidad de Seguidores: " + user.Followers + ", Cantidad de Seguidos: " + user.Follows + ", Cantidad de publicaciones " + user.Chips + "\n";
                }
                return data;
            }
        }

        public string FollowUser(string userToFollow, User user)
        {
            if (ExistUser(userToFollow))
            {
                if (userToFollow != user.Username)
                {
                    user.Follows++;
                    getUserByUsername(userToFollow).Followers++;
                    User userFollowed = getUserByUsername(userToFollow);
                    user.FollowedUsers.Add(userFollowed);
                    userFollowed.FollowersUsers.Add(user);
                    return "Estas siguiendo a " + userToFollow;
                }
                return "No se puede seguirse a si mismo!";
            }
            return "Usuario no encontrado";
        }

        private bool ExistUser(string username)
        {
            User user = usersList.Find(user => user.Username == username);
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public string ShowProfile(User user)
        {
            string data = "Nombre de Usuario: " + user.Username + ", Nombre Real: " + user.Name + ", Cantidad de Seguidores: " + user.Followers + ", Cantidad de Seguidos: " + user.Follows + "Lista de chips: " + "\n" + ChipMessagesList(user) + "\n";
            return (data);
        }

        public string SearchFiveMostFollowedUsers()
        {
            string mostFollowed = "Usuarios mas seguidos: \n";
            List<User> SortedList = usersList.OrderBy(u => u.Followers).ToList();
            SortedList.Reverse();
            var FiveMoreFollowed = SortedList.Take(5);

            foreach (User user in FiveMoreFollowed)
            {
                mostFollowed += user.Name + ": Seguidores: " + user.Followers.ToString() + "\n";
            }
            if (mostFollowed == "Usuarios mas seguidos: \n")
                return "No se encontro usuarios";
            else
                return mostFollowed;
        }

        public string GetFiveMostLoggedInUsers(string from, string to)
        {
            if (!IsDate(from) || !IsDate(to))
            {
                return "Formato de fecha incorrecto";
            }

            string[] fromDate = from.Split("/");
            string[] toDate = to.Split("/");

            DateTime fromDateTime = Convert.ToDateTime(from);
            DateTime toDateTime = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            int timesLogged = 0;
            bool noUsersLogged = true;
            foreach (User user in usersList)
            {
                timesLogged = 0;
                foreach (DateTime date in user.LoggedInTimes)
                {
                    if (date.Ticks > fromDateTime.Ticks && date.Ticks < toDateTime.Ticks)
                    {
                        timesLogged++;
                        noUsersLogged = false;
                    }
                }
                user.TimesLoggedInTimeSlot = timesLogged;
            }
            if (!noUsersLogged)
            {
                List<User> SortedList = usersList.OrderBy(u => u.TimesLoggedInTimeSlot).ToList();
                SortedList.Reverse();
                var FiveMoreLogged = SortedList.Take(5);
                string mostLogged = "Usuarios mas loggeados desde: " + from + " hasta " + to + "  \n";

                foreach (User user in FiveMoreLogged)
                {
                    mostLogged += user.Name + ": Inicio sesion: " + user.TimesLoggedInTimeSlot.ToString() + " veces" + "\n";
                }
                return mostLogged;
            }
            else
                return "Ningun usuario inicio sesion en ese periodo de tiempo. ";
        }

        public bool IsDate(string tempDate)
        {
            DateTime fromDateValue;
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(tempDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string CreateChip(Chip chip, User user)
        {
            if (user == null) { return "se necesita loguearse para publicar un chip"; }

            string message = "";

            if (InsideCharactersLimit(chip.ChipMessage))
            {
                Chip newChip = new Chip
                {
                    ChipMessage = chip.ChipMessage,
                    Id = totalChips + 1
                };
                message = "Bien hecho. Chip publicado";
                user.ChipList.Insert(0, newChip);
                user.Chips++;
                totalChips++;
                NotifyUsers(chip, user);
            }
            else
            {
                message = "Tu chip no puede ser de mas de 280 caracteres";
            }
            return message;
        }

        private bool InsideCharactersLimit(string chipMessage)
        {
            int characters = 280;
            if (chipMessage.Length < characters)
                return true;
            else
                return false;
        }

        public string AnswerChip(string userNameToAnswer, string idChipToAnswerString, string chipMessage)
        {
            int n;
            bool isNumeric = int.TryParse(idChipToAnswerString, out n);
            if (!isNumeric)
                return "Debe ingresar un numero.";

            int idChipToAnswer = Int32.Parse(idChipToAnswerString);
            User user = usersList.Find(user => user.Username == userNameToAnswer);

            if (user.ChipList.Count < idChipToAnswer)
                return "no existe ese ID de chip.";

            Chip newChip = new Chip();

            if (InsideCharactersLimit(chipMessage))
            {
                newChip.ChipMessage = chipMessage;
                Chip chipToAnswer = user.ChipList.Find(chip => chip.Id == idChipToAnswer);

                chipToAnswer.Responses.Add(newChip);
                return "Respuesta enviada!";
            }
            else
                return "Su respuesta no debe superar los 280 caracteres!";
        }

        public string SearchChipsByWord(string word)
        {
            string chipsEncontrados = "Chips Encontrados: \n";
            foreach (User user in usersList)
            {
                foreach (Chip chip in user.ChipList)
                {
                    string[] words = chip.ChipMessage.Split(' ');
                    foreach (string chipWord in words)
                    {
                        if (chipWord == word)
                            chipsEncontrados += chip.ChipMessage + "\n"; ;
                    }
                }
            }
            if (chipsEncontrados == "Chips Encontrados: \n")
                return "No se encontro Chip con esas palabras";
            else
                return chipsEncontrados;
        }

        public String ChipMessagesList(User userToShowChips)
        {
            String chipsToReturn = "";
            foreach (User user in usersList)
            {
                if (user == userToShowChips)
                {
                    foreach (Chip chip in user.ChipList)
                    {
                        chipsToReturn += chip.Id + " - " + chip.ChipMessage + "\n    Respuestas del Chip: \n" + chip.GetResponses() + "\n";
                    }

                    return chipsToReturn;
                }
            }
            return chipsToReturn;
        }
    }
}