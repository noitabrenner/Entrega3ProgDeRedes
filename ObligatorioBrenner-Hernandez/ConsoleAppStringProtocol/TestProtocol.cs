using System;
using System.Text;
using ProtocolLibrary;

namespace ObligatorioProgramacionDeRedes

{
    public static class TestProtocol
    {
        static void Main(string[] args)
        {
            var header = new Header(HeaderConstants.Request,CommandConstants.ListUsers,15);
            var headerbytes = header.GetRequest();
            Console.WriteLine(Encoding.UTF8.GetString(headerbytes,0,9));
            Console.WriteLine(header.DecodeData(headerbytes));
            
        }
    }
}