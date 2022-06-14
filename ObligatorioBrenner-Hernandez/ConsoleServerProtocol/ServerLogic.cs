using Business.Domain;
using Business.Logic;
using BusinessLogicInterface.Interfaces;
using Exceptions;
using ProtocolLibrary;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    internal class ServerLogic
    {
        public ServerLogic()
        { }

        private static ServerLogic instance = null;
        private static Logic myLogic = Logic.Instance;
        private readonly IUserLogic _userLogic;


        public static ServerLogic Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerLogic();
                }
                return instance;
            }
        }

        public static async Task HandlerClient(NetworkStream networkStream)
        {
            User userLogged = null;
            User currentUser = null;
            string option = "";
            while (option != "exit")
            {
                try
                {
                    var header = new Header();
                    await Protocol.ReceiveFixDataAsync(networkStream, header);
                    switch (header.ICommand)
                    {
                        case CommandConstants.Login:
                            currentUser = LoginAsync(networkStream, header, userLogged).Result;
                            break;

                        case CommandConstants.AddUser:
                            await AddUserAsync(networkStream, header);
                            break;

                        case CommandConstants.SearchUser:
                            await SearchUserAsync(networkStream, header, currentUser);
                            break;

                        case CommandConstants.Profile:
                            await showProfileAsync(networkStream, header, currentUser);
                            break;

                        case CommandConstants.AnswerAChip:
                            await AnswerAChipAsync(networkStream, header, currentUser);
                            break;

                        case CommandConstants.CreateChip:
                            await CreateChipAsync(networkStream, header, currentUser);
                            break;

                        case CommandConstants.FollowUser:
                            await FollowUserAsync(networkStream, header, currentUser);
                            break;

                        case CommandConstants.Notifications:
                            await ShowNotificationsAsync(networkStream, header, currentUser);
                            break;

                        case CommandConstants.SendImage:
                            await SendImageAsync(networkStream, header);
                            break;

                        case CommandConstants.Logout:
                            currentUser = LogoutAsync(networkStream, header, currentUser).Result;
                            break;
                    }
                }
                catch (ClientDisconnect e)
                {
                    Console.WriteLine(currentUser.Username + e.Message);
                }
            }
        }

        private static bool UserIsLogged(User userLogged)
        {
            if (userLogged == null)
            {
                return false;
            }
            return true;
        }

        private static async Task AnswerAChipAsync(NetworkStream networkStream, Header header, User userLogged)
        {
            string response = "";
            string request = "";
            string userNameToAnswer = "";
            string idChipToAnswer = "0";
            string chipMessage = "";

            request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);

            string[] objectData = request.Split("%");

            userNameToAnswer = objectData[0];
            idChipToAnswer = objectData[1];
            chipMessage = objectData[2];

            if (!UserIsLogged(userLogged))
                response = "Debe iniciar sesion";
            else
                response = myLogic.AnswerChip(userNameToAnswer, idChipToAnswer, chipMessage);

            await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
        }

        private static async Task<User> LogoutAsync(NetworkStream networkStream, Header header, User userLogged)
        {
            string response = "";
            if (UserIsLogged(userLogged))
            {
                string request = "";
                request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                response = myLogic.LogOut(userLogged);
                userLogged = null;
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
            else
            {
                response = "Inicie sesion para esta funcionalidad";
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
            return userLogged;
        }

        private static async Task ShowNotificationsAsync(NetworkStream networkStream, Header header, User userLogged)
        {
            string response = "";
            if (UserIsLogged(userLogged))
            {
                string request = "";
                request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                response = myLogic.ShowNotifications(userLogged);
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
            else
            {
                response = "Inicie sesion para esta funcionalidad";
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
        }

        private static async Task FollowUserAsync(NetworkStream networkStream, Header header, User userLogged)
        {
            string response = "";
            if (UserIsLogged(userLogged))
            {
                string request = "";
                request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                response = myLogic.FollowUser(request, userLogged);
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
            else
            {
                response = "Inicie sesion para esta funcionalidad";
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
        }

        private static async Task SearchUserAsync(NetworkStream networkStream, Header header, User userlogged)
        {
            if (UserIsLogged(userlogged))
            {
                string response = "";
                string request = "";
                request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                response = myLogic.searchUsers(request);
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
            else
            {
                string response = "Inicie sesion para esta funcionalidad";
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
        }

        private static async Task showProfileAsync(NetworkStream networkStream, Header header, User userlogged)
        {
            string response = "";
            if (UserIsLogged(userlogged))
            {
                string request = "";
                User user = null;
                request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);

                user = myLogic.getUserByUsername(request);
                response = myLogic.ShowProfile(user);
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
            else
            {
                response = "Inicie sesion para esta funcionalidad";
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
        }

        private static async Task<User> LoginAsync(NetworkStream networkStream, Header header, User userLogged)
        {
            string response = "";
            string request = "";
            ProtocolLibrary.Decoders.Decoder decoder = new ProtocolLibrary.Decoders.Decoder();
            request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
            Sesion sesion = (Sesion)decoder.Decode(request, CommandConstants.Login);
            User user = myLogic.getUserByUsername(sesion.Username);
            userLogged = user;
            response = myLogic.Login(sesion);
            await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            return userLogged;
        }

        private static async Task AddUserAsync(NetworkStream networkStream, Header header)
        {
            string response = "";
            string request = "";
            ProtocolLibrary.Decoders.Decoder decoder = new ProtocolLibrary.Decoders.Decoder();
            request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);

            User user = (User)decoder.Decode(request, CommandConstants.AddUser);
            response = myLogic.AddUser(user);
            await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
        }

        private static async Task SendImageAsync(NetworkStream networkStream, Header header)
        {
            string response = "";
            await Protocol.ReceiveFileAsync(networkStream, header);
            response = "Imagen agregada";
            await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
        }

        private static async Task CreateChipAsync(NetworkStream networkStream, Header header, User userLogged)
        {
            string response = "";
            if (UserIsLogged(userLogged))
            {
                string request = "";
                ProtocolLibrary.Decoders.Decoder decoder = new ProtocolLibrary.Decoders.Decoder();
                request = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                Chip chip = (Chip)decoder.Decode(request, CommandConstants.CreateChip);
                response = myLogic.CreateChip(chip, userLogged);
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
            else
            {
                response = "Inicie sesion para esta funcionalidad";
                await Protocol.SendDataResponseAsync(networkStream, CommandConstants.Success, response);
            }
        }
    }
}