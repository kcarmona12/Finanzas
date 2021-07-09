using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finanzas.Models.Maps
{
    public class AmigoMap : IEntityTypeConfiguration<Amigo>
    {
        public void Configure(EntityTypeBuilder<Amigo> builder)
        {
            builder.ToTable("Amigo");
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.Users).
                WithMany().
                HasForeignKey(o => o.IdUS);
        }
    }
}
