using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Model
{
    public class UsuarioModel
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string RFC { get; set; }
        [Required]
        public string RazonSocial { get; set; }
        [Required]
        [EmailAddress]
        public string Mail { get; set; }
        [Required]
        [Phone]
        public string Telefono { get; set; }
        [Required]
        public string DireccionFiscal { get; set; }
        [Required]
        public string Password { get; set; }
        public int IdUsuario { get; set; }
        public int IdPrivilegio { get; set; }
        public string TipoUsuario { get; set; }
    }
}
