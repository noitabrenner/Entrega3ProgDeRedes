using System;
using System.Collections.Generic;

namespace Business.Domain
{
    public class Chip
    {
        public int Id { get; set; }
        public string ChipMessage { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public List<Chip> Responses;

        public Chip()
        {
            ChipMessage = "";
            Responses = new List<Chip>();
            Image1 = "";
            Image2 = "";
            Image3 = "";
        }

        public string GetResponses()
        {
            String responsesToReturn = "";

            foreach (Chip response in this.Responses)
            {
                responsesToReturn += "    - " + response.ChipMessage + "\n";
            }

            return responsesToReturn;
        }
    }
}