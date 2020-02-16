using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTemplate.Models
{
    public class Account
    {
        public int accountID;
        public string userName;
        public string ownerName;
        public string bio;
        public string password;
        public bool IsAdmin;
    }
}