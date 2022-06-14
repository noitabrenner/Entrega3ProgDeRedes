using Common;
using Common.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    internal class ClientProgram
    {
        private static readonly ISettingsManager MySettingsMgr = new SettingsManager();

        private static async Task Main(string[] args)
        {
            var readSettingIpAddressClient = MySettingsMgr.ReadSetting(ClientConfig.ClientIpConfigKey);
            var readSettingPortClient = MySettingsMgr.ReadSetting(ClientConfig.ClientPortConfigKey);

            var readSettingIpAddressServer = MySettingsMgr.ReadSetting(ClientConfig.ServerIpConfigKey);
            var readSettingPortServer = MySettingsMgr.ReadSetting(ClientConfig.ServerPortConfigKey);

            var clientIpEndPoint = new IPEndPoint(
                            IPAddress.Parse(readSettingIpAddressClient),
                                port: int.Parse(readSettingPortClient));

            TcpClient tcpClient = new TcpClient(clientIpEndPoint);
            await tcpClient.ConnectAsync(IPAddress.Parse(readSettingIpAddressServer), int.Parse(readSettingPortServer));
            Console.WriteLine("Conectado al servidor");

            NetworkStream networkStream = tcpClient.GetStream();
            await ClientLogic.WriteServer(tcpClient, networkStream);
        }
    }
}