using Exceptions;
using FileManager;
using FileManager.Interfaces;
using ProtocolLibrary;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    public class ClientLogic
    {
        private static readonly IFileHandler _fileHandler = new FileHandler();
        private static bool connected = true;

        private ClientLogic()
        { }

        private static ClientLogic instance = null;

        public static ClientLogic Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientLogic();
                }
                return instance;
            }
        }

        public static async Task WriteServer(TcpClient tcpClient, NetworkStream networkStream)
        {
            Console.WriteLine("Bienvenido al aplicacion CHIPPER: la mejor red social");
            ShowBriefMenuAsync();
            Header header = new Header();
            try
            {
                while (connected)
                {
                    int option = 0;
                    try
                    {
                        option = GetNumber(Console.ReadLine());
                    }
                    catch (System.FormatException)
                    {
                        Console.WriteLine("Ingrese un numero");
                    }
                    switch (option)
                    {
                        case 0:
                            ShowMenuAsync();
                            break;

                        case 1:
                            await LoginAsync(networkStream, header);
                            ShowMenuAsync();
                            break;

                        case 2:
                            await AddUserAsync(networkStream, header);
                            ShowBriefMenuAsync();
                            break;

                        case 3:
                            await SearchAnUserAsync(networkStream, header);
                            ShowMenuAsync();
                            break;

                        case 4:
                            await FollowAnUserAsync(networkStream, header);
                            ShowMenuAsync();
                            break;

                        case 5:
                            await CreateChipAsync(networkStream, header);
                            ShowMenuAsync();
                            break;

                        case 6:
                            await ShowNotificationsAsync(networkStream, header);
                            ShowMenuAsync();
                            break;

                        case 7:
                            await LogoutAsync(tcpClient, networkStream, header);
                            ShowBriefMenuAsync();
                            break;

                        case 10:
                            connected = false;
                            break;

                        default:
                            Console.WriteLine("Opcion invalida. Ingrese 0 para volver al menu");
                            break;
                    }
                }
            }
            catch (ServerDisconnect s)
            {
                connected = false;
                tcpClient.Close();
                networkStream.Close();
                Console.WriteLine(s.Message);
            }

            Console.WriteLine("Exiting Application");
        }

        private static void ShowBriefMenuAsync()
        {
            Console.WriteLine(" \n Menu: ");
            Console.WriteLine("1: login -> Inicia sesion");
            Console.WriteLine("2: sign up -> Registro de usuario");
            Console.WriteLine("Luego de iniciar sesion podra usar la aplicacion");
        }

        private static void ShowMenuAsync()
        {
            Console.WriteLine(" \n Volver al menu?  \n - Ingrese: Menu o 0");
            string option = Console.ReadLine();
            if (option == "Menu" || option == "MENU" || option == "0")
            {
                Console.WriteLine(" \n Menu: ");
                Console.WriteLine("1: login -> Inicia sesion");
                Console.WriteLine("2: sign up -> Registro de usuario");
                Console.WriteLine("3: busqueda de usuario - ver perfil - responder chips");
                Console.WriteLine("4: seguir a un usuario");
                Console.WriteLine("5: escribir una publicacion");
                Console.WriteLine("6: ver notificaciones");
                Console.WriteLine("7: logout -> Cerrar sesion");
                Console.WriteLine("10: exit -> Desconectarte del servidor");
                Console.WriteLine("Ingrese su opcion: ");
            }
            else
            {
                Console.WriteLine(" \n Comando no reconocido. Para volver al menu, ingrese 0");
            }
        }

        private static async Task LogoutAsync(TcpClient tcpClient, NetworkStream networkStream, Header header)
        {
            Console.WriteLine("Estas seguro que quieres cerrar sesion?SI/NO");
            var option = Console.ReadLine();
            try
            {
                if (option == "SI")
                {
                    string response = "";
                    await Protocol.SendDataAsync(networkStream, CommandConstants.Logout, "");
                    header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                    response = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                    Console.WriteLine(response);
                }
            }
            catch (SocketException)
            { }
        }

        private static async Task ShowNotificationsAsync(NetworkStream networkStream, Header header)
        {
            try
            {
                string response = "";
                await Protocol.SendDataAsync(networkStream, CommandConstants.Notifications, "");
                header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                response = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                Console.WriteLine(response);
            }
            catch (SocketException)
            {
                connected = false;
            }
        }

        private static async Task AddUserAsync(NetworkStream networkStream, Header header)
        {
            string userName;
            string password;
            string realName;
            string photo = "/";

            Console.WriteLine("Ingrese nombre real");
            realName = Console.ReadLine();
            Console.WriteLine("Ingrese nombre de usuario");
            userName = Console.ReadLine();
            Console.WriteLine("Ingrese contrasena");
            password = Console.ReadLine();
            Console.WriteLine("ingrese la ruta de la foto");
            photo = Console.ReadLine();

            string response1 = "";
            try
            {
                await Protocol.SendDataAsync(networkStream, CommandConstants.AddUser, $"{userName}%{password}%{realName}%{photo}");
                Console.WriteLine(response1);
                header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                response1 = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);

                string path = string.Empty;
                if (response1 != "Nombre de usuario repetido.")
                {
                    while (path != null && path.Equals(string.Empty) && !_fileHandler.FileExists(path))
                    {
                        path = photo;
                    }

                    string response2 = "";
                    await Protocol.SendFileAsync(networkStream, path, CommandConstants.SendImage);
                    header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                    response2 = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                    Console.WriteLine(response2);
                }

                Console.WriteLine(response1);
            }
            catch (SocketException)
            {
                connected = false;
            }
        }

        private static async Task LoginAsync(NetworkStream networkStream, Header header)
        {
            Console.WriteLine("Ingrese usuario");
            var user = Console.ReadLine();
            Console.WriteLine("Ingrese contrasena");
            var password = Console.ReadLine();
            string response = "";
            try
            {
                await Protocol.SendDataAsync(networkStream, CommandConstants.Login, $"{user}%{password}");

                header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                response = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                Console.WriteLine(response);
            }
            catch (SocketException)
            {
                connected = false;
            }
        }

        private static async Task CreateChipAsync(NetworkStream networkStream, Header header)
        {
            Console.WriteLine("Ingrese la publicacion a enviar:");
            var text = Console.ReadLine();
            List<string> images = new List<string>();
            string image = "";

            Console.WriteLine("A continuacion puede ingresar imagenes.  \n");
            int number = 3;
            int option = 1;
            while (number > 0)
            {
                Console.WriteLine("Ingrese ruta de imagen " + option + " En caso de no querer, presione enter");
                image = Console.ReadLine();
                images.Add(image);
                number--;
                option++;
            }
            string response1 = "";
            try
            {
                await Protocol.SendDataAsync(networkStream, CommandConstants.CreateChip, $"{text}%{images[0]}%{images[1]}%{images[2]}");
                header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                response1 = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                Console.WriteLine(response1);
                string path1 = images[0];
                string path2 = images[1];
                string path3 = images[2];

                if (response1 != "Tu chip no puede ser de mas de 280 caracteres")
                {
                    while (path1 != null && !_fileHandler.FileExists(path1) && path1 != "")
                    {
                        path1 = images[0];
                        path2 = images[1];
                        path3 = images[2];
                    }

                    string response2 = "";
                    if (images[0] != "")
                    {
                        await Protocol.SendFileAsync(networkStream, path1, CommandConstants.SendImage);
                        header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                        response2 = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                        Console.WriteLine(response2);
                    }
                    if (images[1] != "")
                    {
                        await Protocol.SendFileAsync(networkStream, path2, CommandConstants.SendImage);
                        header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                        response2 = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                        Console.WriteLine(response2);
                    }
                    if (images[2] != "")
                    {
                        await Protocol.SendFileAsync(networkStream, path3, CommandConstants.SendImage);
                        header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                        response2 = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                        Console.WriteLine(response2);
                    }
                }
            }
            catch (SocketException)
            {
                connected = false;
            }
        }

        private static async Task FollowAnUserAsync(NetworkStream networkStream, Header header)
        {
            string response = "";
            Console.WriteLine("Ingrese el nombre de usuario a seguir:");
            var userToFollow = Console.ReadLine();
            try
            {
                await Protocol.SendDataAsync(networkStream, CommandConstants.FollowUser, $"{userToFollow}");
                header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                response = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                Console.WriteLine(response);
            }
            catch (SocketException)
            { }
        }

        private static async Task SearchAnUserAsync(NetworkStream networkStream, Header header)
        {
            string user = "";
            string response = "";
            Console.WriteLine("ingrese nombre de usuario o nombre real para buscar");
            user = Console.ReadLine();

            await ProtocolLibrary.Protocol.SendDataAsync(networkStream, CommandConstants.SearchUser, $"{user}");

            try
            {
                header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                response = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                Console.WriteLine(response);
            }
            catch (SocketException)
            {
                connected = false;
            }
            if (response != "No hay busquedas encontradas" && response != "Inicie sesion para esta funcionalidad")
            {
                await ShowProfileAsync(networkStream, header);
            }
        }

        private static async Task ShowProfileAsync(NetworkStream networkStream, Header header)
        {
            string response = "";
            Console.WriteLine("ver perfil de algun de estos usuarios? SI/NO");
            string option = Console.ReadLine();
            if (option == "SI")
            {
                Console.WriteLine("Ingrese nombre de usuario para ver perfil");
                string userProfile = Console.ReadLine();
                await ProtocolLibrary.Protocol.SendDataAsync(networkStream, CommandConstants.Profile, $"{userProfile}");
                header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                response = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                Console.WriteLine(response);
                if (response != "se necesita estar logueado")
                {
                    await AnswerAChipAsync(networkStream, header, userProfile);
                }
            }
        }

        private static async Task AnswerAChipAsync(NetworkStream networkStream, Header header, string username)
        {
            string response = "";
            Console.WriteLine("Quiere responder un chip? SI/NO");
            string option = Console.ReadLine();
            if (option == "SI")
            {
                Console.WriteLine("Seleccione numero de chip a responder");
                string idChip = Console.ReadLine();

                Console.WriteLine("Escriba la respuesta a su chip");
                string chipMessage = Console.ReadLine();
                try
                {
                    await ProtocolLibrary.Protocol.SendDataAsync(networkStream, CommandConstants.AnswerAChip, $"{username}%{idChip}%{chipMessage}");
                    header = await Protocol.ReceiveFixDataAsync(networkStream, header);
                    response = await Protocol.ReceiveVariableDataAsync(networkStream, header.IDataLength);
                    Console.WriteLine(response);
                }
                catch (SocketException)
                {
                    connected = false;
                }
            }
        }

        private static int GetNumber(string num)
        {
            int number = Int32.Parse(num);
            return number;
        }
    }
}