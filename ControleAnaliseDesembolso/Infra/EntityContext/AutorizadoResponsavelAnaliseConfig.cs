using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class AutorizadoResponsavelAnaliseConfig : IEntityTypeConfiguration<AutorizadoResponsavelAnalise>
{
    public void Configure(EntityTypeBuilder<AutorizadoResponsavelAnalise> builder)
    {
        builder.ToTable("CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE");

        builder.HasKey(x => x.Matricula)
            .HasName("PK_CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE");

        builder.Property(x => x.Matricula)
            .HasColumnName("MATRICULA")
            .HasMaxLength(7)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.DtInclusao)
            .HasColumnName("DT_INCLUSAO")
            .HasColumnType("datetime")
            .IsRequired();
    }
}
