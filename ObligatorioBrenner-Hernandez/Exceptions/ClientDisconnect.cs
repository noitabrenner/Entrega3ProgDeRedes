using System;

namespace Exceptions
{
    public class ClientDisconnect : Exception
    {
        private string message;

        public override string Message
        {
            get { return message; }
        }

        public ClientDisconnect()
        {
            this.message = "Usuario desconectado";
        }
    }
}