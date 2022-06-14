using FileManager;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLibrary
{
    public class Protocol
    {
        private static readonly FileStreamHandler _fileStreamHandler = new FileStreamHandler();
        private static readonly FileHandler _fileHandler = new FileHandler();

        public static async Task SendDataAsync(NetworkStream networkStream, int command, string message)
        {
            var header = new Header(HeaderConstants.Request, command, message.Length);
            var data = header.GetRequest();
            var sentBytes = 0;
            await networkStream.WriteAsync(data, sentBytes, data.Length - sentBytes);
            var bytesMessage = Encoding.UTF8.GetBytes(message);
            await networkStream.WriteAsync(bytesMessage, sentBytes, bytesMessage.Length - sentBytes);
        }

        public static async Task SendDataResponseAsync(NetworkStream networkStream, int command, string message)
        {
            var header = new Header(HeaderConstants.Response, command, message.Length);
            var data = header.GetRequest();
            var sentBytes = 0;
            await networkStream.WriteAsync(data, sentBytes, data.Length - sentBytes);
            sentBytes = 0;
            var bytesMessage = Encoding.UTF8.GetBytes(message);
            await networkStream.WriteAsync(bytesMessage, sentBytes, bytesMessage.Length - sentBytes);
        }

        public static async Task<Header> ReceiveFixDataAsync(NetworkStream networkStream, Header header)
        {
            int headerLength = HeaderConstants.Request.Length + HeaderConstants.CommandLength + HeaderConstants.DataLength;
            byte[] buffer = await ReceiveDataAsync(networkStream, headerLength);
            header.DecodeData(buffer);
            return header;
        }

        public static async Task<string> ReceiveVariableDataAsync(NetworkStream networkStream, int length)
        {
            byte[] value = await ReceiveDataAsync(networkStream, length);
            string response = Encoding.UTF8.GetString(value);
            return response;
        }

        public static async Task<byte[]> ReceiveDataAsync(NetworkStream networkStream, int length)
        {
            var iRecv = 0;
            byte[] buffer = new byte[length];
            while (iRecv < length)
            {
                int localRecv = await networkStream.ReadAsync(buffer, iRecv, length - iRecv);
                if (localRecv == 0)
                {
                    throw new Exception("Server is closing");
                }
                iRecv += localRecv;
            }
            return buffer;
        }

        public static long GetParts(long fileSize)
        {
            var parts = fileSize / HeaderConstants.MaxPacketSize;
            return parts * HeaderConstants.MaxPacketSize == fileSize ? parts : parts + 1;
        }

        public static async Task SendFileAsync(NetworkStream networkStream, string path, int command)
        {
            try
            {
                long fileSize = _fileHandler.GetFileSize(path);
                var fileSizeData = BitConverter.GetBytes(fileSize).Length;
                string fileName = _fileHandler.GetFileName(path);
                var header = new Header(HeaderConstants.Request, command, fileName.Length + fileSizeData);

                await SendDataAsync(networkStream, command, $"{fileName},{fileSize}");

                long parts = GetParts(fileSize);
                long offset = 0;
                long currentPart = 1;

                while (fileSize > offset)
                {
                    byte[] data;
                    if (currentPart == parts)
                    {
                        var lastPartSize = (int)(fileSize - offset);
                        data = await _fileStreamHandler.ReadDataAsync(path, offset, lastPartSize);
                        offset += lastPartSize;
                    }
                    else
                    {
                        data = await _fileStreamHandler.ReadDataAsync(path, offset, HeaderConstants.MaxPacketSize);
                        offset += HeaderConstants.MaxPacketSize;
                    }
                    await networkStream.WriteAsync(data, 0, data.Length);
                    currentPart++;
                }
            }
            catch (System.Exception)
            {
                Console.Write("archivo incorrecto");
            }
        }

        public static async Task ReceiveFileAsync(NetworkStream networkStream, Header header)
        {
            string dataFile = await ReceiveVariableDataAsync(networkStream, header.IDataLength);
            string[] fileInfo = dataFile.Split(",");
            var fileName = fileInfo[0].ToString();
            var fileSize = long.Parse(fileInfo[1]);

            long parts = GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = await ReceiveDataAsync(networkStream, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await ReceiveDataAsync(networkStream, HeaderConstants.MaxPacketSize);
                    offset += HeaderConstants.MaxPacketSize;
                }

                await _fileStreamHandler.WriteDataAsync(fileName, data);
                currentPart++;
            }
        }
    }
}