using System;
using System.Text;

namespace ProtocolLibrary
{
    public class Header
    {
        private byte[] _direction;
        private byte[] _command;
        private byte[] _dataLength;

        private String _sDirection;
        private int _iCommand;
        private int _iDataLength;

        public string SDirection
        {
            get => _sDirection;
            set => _sDirection = value;
        }

        public int ICommand
        {
            get => _iCommand;
            set => _iCommand = value;
        }

        public int IDataLength
        {
            get => _iDataLength;
            set => _iDataLength = value;
        }

        public Header()
        {
        }

        public Header(string direction, int command, int datalength)
        {
            _direction = Encoding.UTF8.GetBytes(direction);
            var stringCommand = command.ToString("D2");
            _command = Encoding.UTF8.GetBytes(stringCommand);
            var stringData = datalength.ToString("D4");
            _dataLength = Encoding.UTF8.GetBytes(stringData);
        }

        public static int GetLength()
        {
            return HeaderConstants.FixedFileNameLength + HeaderConstants.FixedFileSizeLength;
        }

        public byte[] GetRequest()
        {
            var header = new byte[HeaderConstants.Request.Length + HeaderConstants.CommandLength + HeaderConstants.DataLength];
            Array.Copy(_direction, 0, header, 0, 3);
            Array.Copy(_command, 0, header, HeaderConstants.Request.Length, 2);
            Array.Copy(_dataLength, 0, header, HeaderConstants.Request.Length + HeaderConstants.CommandLength, 4);
            return header;
        }

        public byte[] Create(string fileName, long fileSize)
        {
            var header = new byte[GetLength()];
            var fileNameData = BitConverter.GetBytes(Encoding.UTF8.GetBytes(fileName).Length);
            if (fileNameData.Length != HeaderConstants.FixedFileNameLength)
                throw new Exception("There is something wrong with the file name");
            var fileSizeData = BitConverter.GetBytes(fileSize);

            Array.Copy(fileNameData, 0, header, 0, HeaderConstants.FixedFileNameLength);
            Array.Copy(fileSizeData, 0, header, HeaderConstants.FixedFileNameLength, HeaderConstants.FixedFileSizeLength);

            return header;
        }

        public bool DecodeData(byte[] data)
        {
            try
            {
                _sDirection = Encoding.UTF8.GetString(data, 0, HeaderConstants.Request.Length);
                var command =
                    Encoding.UTF8.GetString(data, HeaderConstants.Request.Length, HeaderConstants.CommandLength);
                _iCommand = int.Parse(command);
                var dataLength = Encoding.UTF8.GetString(data,
                    HeaderConstants.Request.Length + HeaderConstants.CommandLength, HeaderConstants.DataLength);
                _iDataLength = int.Parse(dataLength);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Catched: " + e.Message);
                return false;
            }
        }
    }
}