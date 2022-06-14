namespace ProtocolLibrary
{
    public static class HeaderConstants
    {
        public static string Request = "REQ";
        public static string Response = "RES";
        public static int CommandLength = 2;
        public static int DataLength = 4;
        public static int FixedFileNameLength = 4;
        public static int FixedFileSizeLength = 8;
        public const int MaxPacketSize = 32768;
    }
}