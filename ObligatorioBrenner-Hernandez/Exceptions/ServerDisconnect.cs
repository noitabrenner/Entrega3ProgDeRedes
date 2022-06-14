using System;

namespace Exceptions
{
    public class ServerDisconnect : Exception
    {
        private string message;

        public override string Message
        {
            get { return message; }
        }

        public ServerDisconnect()
        {
            this.message = "el server fue desconectado";
        }
    }
}