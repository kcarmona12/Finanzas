using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finanzas.Models.Maps
{
    public class TransaccionMap : IEntityTypeConfiguration<Transaccion>
    {
        public void Configure(EntityTypeBuilder<Transaccion> builder)
        {
            builder.ToTable("Transaccion");
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.Tipo)
                .WithMany()
                .HasForeignKey(o => o.IdTipo);
        }
    }
}
