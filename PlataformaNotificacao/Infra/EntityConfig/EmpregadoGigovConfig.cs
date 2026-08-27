using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaNotificacao.Domain;

namespace PlataformaNotificacao.Infra.EntityConfig
{
    public class EmpregadoGigovConfig : IEntityTypeConfiguration<EmpregadoGigov>
    {
        public void Configure(EntityTypeBuilder<EmpregadoGigov> builder)
        {
            builder.ToTable("PLA_NOT_TB003_EMPREGADOS_GIGOV");

            builder.HasKey(x => x.CodigoEmpregado);

            builder.Property(x => x.CodigoEmpregado)
                .HasColumnName("CO_EMPREGADO");

            builder.Property(x => x.Matricula)
                .HasColumnName("MATRICULA")
                .HasMaxLength(7)
                .IsRequired();

            builder.Property(x => x.Nome)
                .HasColumnName("NOME")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.CodigoGigov)
                .HasColumnName("CO_GIGOV")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Ativo)
                .HasColumnName("ATIVO")
                .IsRequired();
        }
    }
}
