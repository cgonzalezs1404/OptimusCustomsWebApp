using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Model
{
    public class SessionData
    {
        [Required(ErrorMessage = "Campo Usuario Requerido")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Campo Contraseña Requerido")]
        public string Password { get; set; }
        [DefaultValue (false)]
        public bool RememberMe { get; set; }
    }
}
