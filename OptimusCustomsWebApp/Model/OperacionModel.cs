using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Model
{
    public class OperacionModel
    {
        public int IdOperacion { get; set; }
        [Required]
        public int IdTipoOperacion { get; set; }
        public string TipoOperacion { get; set; }
        //[Required] Comentado por sustitucion de id de usuario activo.
        public int? IdUsuario { get; set; }
        public string RazonSocial { get; set; }
        public int? IdFactura { get; set; }
        public bool ExisteFactura { get; set; }
        public bool ExistePruebaEntrega { get; set; }
        public bool ExisteComprobantePago { get; set; }
        public bool ExisteComplementoPago { get; set; }
        [Required]
        public string NumOperacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
