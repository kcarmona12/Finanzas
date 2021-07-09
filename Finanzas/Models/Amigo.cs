using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finanzas.Models
{
    public class Amigo
    {
        public int Id { get; set; }
        public int IdUP { get; set; }
        public int IdUS { get; set; }

        public User Users { get; set; }
    }
}
