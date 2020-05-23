using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace kraud.Models
{
    public class LoginModel
    {  
        public string Login { get; set; }

        public string Password { get; set; }
    }
}