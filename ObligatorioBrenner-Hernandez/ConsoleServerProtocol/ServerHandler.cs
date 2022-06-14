using Business.Logic;
using Common;
using Common.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    public class ServerHandler
    {
        private static Logic myLogic = Logic.Instance;
        private bool runServer = true;

        private static readonly ISettingsManager MySettingsMgr = new SettingsManager();

        public ServerHandler()
        {
            var readSettingIpAddress = MySettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey);
            var readSettingPort = MySettingsMgr.ReadSetting(ServerConfig.ServerPortConfigKey);

            Console.WriteLine($"Server is starting in IP {readSettingIpAddress} and Port {readSettingPort}..");

            var ipEndPoint = new IPEndPoint(
                IPAddress.Parse(readSettingIpAddress),
                    port: int.Parse(readSettingPort));

            TcpListener tcpListener = new TcpListener(ipEndPoint);
            tcpListener.Start(100);
            Task.Run(() => AcceptClients(tcpListener));

            Console.WriteLine("Bienvenido al sistema CHIPPER");
            string response = "";
            string message = "";
            while (message != "7")
            {
                Console.WriteLine(" \n Seleccione una opcion: \n 1.Ver Lista de usuarios \n 2.Bloquear Usuario \n 3.Desbloquear usuario \n 4.Buscar chips por palabras \n 5.Buscar 5 usuarios con mas seguidores + \n 6.Ver 5 usuarios que mas iniciaron sesion en un intervalo de tiempo" + " \n 7.Exit");

                message = Console.ReadLine();
                if (message == "1")
                {
                    response = myLogic.GetAllUsers();
                    Console.WriteLine(response);
                }
                else if (message == "2")
                {
                    Console.WriteLine("Ingrese nombre de usuario a bloquear");
                    var userToBlock = Console.ReadLine();
                    response = myLogic.BlockUser(userToBlock);
                    Console.WriteLine(response);
                }
                else if (message == "3")
                {
                    Console.WriteLine("Ingrese nombre de usuario a desbloquear");
                    var userToUnlock = Console.ReadLine();
                    response = myLogic.unlockUser(userToUnlock);
                    Console.WriteLine(response);
                }
                else if (message == "4")
                {
                    Console.WriteLine("Ingrese palabras para buscar Chips");
                    var word = Console.ReadLine();
                    response = myLogic.SearchChipsByWord(word);
                    Console.WriteLine(response);
                }
                else if (message == "5")
                {
                    response = myLogic.SearchFiveMostFollowedUsers();
                    Console.WriteLine(response);
                }
                else if (message == "6")
                {
                    Console.WriteLine("Ingrese fecha desde: (DD/MM/AAAA)");
                    var fromDate = Console.ReadLine();
                    Console.WriteLine("Ingrese fecha hasta: (DD/MM/AAAA)");
                    var toDate = Console.ReadLine();
                    response = myLogic.GetFiveMostLoggedInUsers(fromDate, toDate);
                    Console.WriteLine(response);
                }
                else
                {
                    Console.WriteLine("opcion incorrecta ingresada");
                }
                runServer = false;
            }
        }

        private void AcceptClients(TcpListener tcpListener)
        {
            while (runServer)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                NetworkStream _networkStream = tcpClient.GetStream();
                Task.Run(async () => await ServerLogic.HandlerClient(_networkStream));
            }
        }
    }
}