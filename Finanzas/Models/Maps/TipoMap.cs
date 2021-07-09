using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanzas.Models.Maps
{
    public class TipoMap : IEntityTypeConfiguration<Tipo>
    {
        public void Configure(EntityTypeBuilder<Tipo> builder)
        {
            builder.ToTable("Tipo");
            builder.HasKey(o => o.Id);
        }
    }
}
