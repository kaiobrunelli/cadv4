using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class CampoConferenciaConfig : IEntityTypeConfiguration<CampoConferencia>
{
    public void Configure(EntityTypeBuilder<CampoConferencia> builder)
    {
        builder.ToTable("CAD_TB005_CAMPO_CONFERENCIA");

        builder.HasKey(x => x.CoCampo)
            .HasName("PK_CAD_TB005_CAMPO_CONFERENCIA");

        builder.Property(x => x.CoCampo)
            .HasColumnName("CO_CAMPO")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Chave)
            .HasColumnName("CHAVE")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DeCampo)
            .HasColumnName("DE_CAMPO")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.DtCriacao)
            .HasColumnName("DT_CRIACAO")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(x => x.Desativado)
            .HasColumnName("DESATIVADO")
            .IsRequired();

        builder.HasMany(x => x.Conferencias)
            .WithOne(x => x.CampoConferencia)
            .HasForeignKey(x => x.CoCampo)
            .HasConstraintName("FK_CAD_TB005_CAMPO_CONFERENCIA");
    }
}
