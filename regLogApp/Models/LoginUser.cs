using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AzureDb;

namespace regLogApp.Models
{
    public class LoginUser
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool IsValid(string username, string password)
        {
            using (var context = new ervinEntities())
            {
                if (Enumerable.Any(context.Users, user => user.UserName == username && user.Password == password))
                {
                    return true;
                }
            }

            return false;
        }
    }
}