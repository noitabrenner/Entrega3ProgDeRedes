using System;
using System.Collections.Generic;

namespace Business.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public int Followers { get; set; }
        public int Follows { get; set; }
        public int Chips { get; set; }
        public List<Chip> ChipList { get; set; }
        public List<User> FollowedUsers { get; set; }
        public List<User> FollowersUsers { get; set; }
        public List<String> Notifications { get; set; }
        public bool Access { get; set; }
        public int ActiveAccount { get; set; }
        public List<DateTime> LoggedInTimes { get; set; }
        public int TimesLoggedInTimeSlot { get; set; }
        public User(string userName, string name, string password)
        {
            this.Username = userName;
            this.Password = password;
            this.Name = name;
        }


        public User()
        {
            this.Username = "";
            this.Password = "";
            this.Name = "";
            this.Photo = "";
            this.Access = true;
            this.ChipList = new List<Chip>();
            this.FollowedUsers = new List<User>();
            this.FollowersUsers = new List<User>();
            this.Notifications = new List<String>();
            this.LoggedInTimes = new List<DateTime>();
            this.TimesLoggedInTimeSlot = 0;
        }
    }
}