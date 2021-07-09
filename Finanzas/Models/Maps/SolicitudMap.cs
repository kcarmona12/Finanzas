using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finanzas.Models.Maps
{
    public class SolicitudMap : IEntityTypeConfiguration<Solicitud>
    {
        public void Configure(EntityTypeBuilder<Solicitud> builder)
        {
            builder.ToTable("Solicitud");
            builder.HasKey(o => o.Id);
        }
    }
}
