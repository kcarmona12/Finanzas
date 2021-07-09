using System;
using System.ComponentModel.DataAnnotations;

namespace Finanzas.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int IdCuenta { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int IdTipo { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public DateTime Fecha { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public decimal Monto { get; set; }

        public Tipo Tipo { get; set; }
    }
}
