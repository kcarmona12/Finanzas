using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finanzas.Models
{
    public class Solicitud
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdAmigo { get; set; }
        public string Mensaje { get; set; }
    }
}
