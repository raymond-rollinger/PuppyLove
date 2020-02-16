using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTemplate.Models
{
    public class Profile
    {
        public int profileID;
        public string petName;
        public string city;
        public int accountID;
        public string breed;
        public string gender;
        public string age;
        public string Bio { get => Bio; set => Bio = value; }
    }
}