using System.Text;

namespace ProtocolLibrary.Decoders
{
    public class DecoderResponse
    {
        public static object DecodeResponse(byte[] buffer, int command)
        {
            string response = Encoding.UTF8.GetString(buffer);
            return response;
        }
    }
}