using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class ConferenciaControleDesembolsoConfig : IEntityTypeConfiguration<ConferenciaControleDesembolso>
{
    public void Configure(EntityTypeBuilder<ConferenciaControleDesembolso> builder)
    {
        builder.ToTable("CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO");

        builder.HasKey(x => x.CoConferencia)
            .HasName("PK_CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO");

        builder.Property(x => x.CoConferencia)
            .HasColumnName("CO_CONFERENCIA")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.CoControleDesembolso)
            .HasColumnName("CO_CONTROLE_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.CoCampo)
            .HasColumnName("CO_CAMPO")
            .IsRequired();

        builder.Property(x => x.DeCampo)
            .HasColumnName("DE_CAMPO")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Situacao)
            .HasColumnName("CO_SITUACAO")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Mensagem)
            .HasColumnName("MENSAGEM")
            .HasMaxLength(1000);

        builder.Property(x => x.DtConferencia)
            .HasColumnName("DT_CONFERENCIA")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(x => x.ControleDesembolso)
            .WithMany(x => x.Conferencias)
            .HasForeignKey(x => x.CoControleDesembolso)
            .HasConstraintName("FK_CAD_TB006_CONTROLE_DESEMBOLSO");

        builder.HasOne(x => x.CampoConferencia)
            .WithMany(x => x.Conferencias)
            .HasForeignKey(x => x.CoCampo)
            .HasConstraintName("FK_CAD_TB006_CAMPO_CONFERENCIA");
    }
}
