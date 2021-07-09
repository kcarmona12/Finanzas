using System.ComponentModel.DataAnnotations;

namespace Finanzas.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
