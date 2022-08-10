using System;
using System.ComponentModel.DataAnnotations;

namespace auth_web_api.Models.DatabaseObjects
{
    public class User
    {
        public Guid Id { get; set; }
       
        public string Email { get; set; }
        public string Login { get; set; }
        public string HashedPassword { get; set; }
    }
}
