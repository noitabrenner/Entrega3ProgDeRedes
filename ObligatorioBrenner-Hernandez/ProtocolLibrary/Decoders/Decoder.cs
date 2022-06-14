using Business.Domain;

namespace ProtocolLibrary.Decoders
{
    public class Decoder
    {
        public object Decode(string data, int command)
        {
            string[] objectData = data.Split("%");

            object objectToReturn = null;
            switch (command)
            {
                case CommandConstants.AddUser:

                    User newUser = new User
                    {
                        Username = objectData[0],
                        Password = objectData[1],
                        Name = objectData[2],
                        Photo = objectData[3],
                        Followers = 0,
                        Follows = 0,
                    };
                    objectToReturn = newUser;

                    break;

                case CommandConstants.Login:

                    Sesion newSesion = new Sesion
                    {
                        Username = objectData[0],
                        Password = objectData[1]
                    };
                    objectToReturn = newSesion;
                    break;

                case CommandConstants.CreateChip:
                    Chip newChip = new Chip
                    {
                        ChipMessage = objectData[0],
                        Image1 = objectData[1],
                        Image2 = objectData[2],
                        Image3 = objectData[3],
                    };
                    objectToReturn = newChip;
                    break;
            }
            return objectToReturn;
        }
    }
}