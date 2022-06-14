using ProtocolLibrary;
using System;
using System.Text;

namespace ObligProgDeRedes
{
    public static class ProtocolTest
    {
        private static void Main(string[] args)
        {
            var header = new Header(HeaderConstants.Request, CommandConstants.Login, 15);
            var headerbytes = header.GetRequest();
            Console.WriteLine(Encoding.UTF8.GetString(headerbytes, 0, 9));
            Console.WriteLine(header.DecodeData(headerbytes));
        }
    }
}