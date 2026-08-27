using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class ControleDesembolsoConfig : IEntityTypeConfiguration<ControleDesembolso>
{
    public void Configure(EntityTypeBuilder<ControleDesembolso> builder)
    {
        builder.ToTable("CAD_TB002_CONTROLE_DESEMBOLSO");

        builder.HasKey(x => x.CoControleDesembolso)
            .HasName("PK_CAD_TB002_CONTROLE_DESEMBOLSO");

        builder.Property(x => x.CoControleDesembolso)
            .HasColumnName("CO_CONTROLE_DESEMBOLSO")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.CoDesembolso)
            .HasColumnName("CO_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.ResponsavelAnalise)
            .HasColumnName("RESPONSAVEL_ANALISE")
            .HasMaxLength(7);

        builder.Property(x => x.ResponsavelBaixa)
            .HasColumnName("RESPONSAVEL_BAIXA")
            .HasMaxLength(7);

        builder.Property(x => x.ResponsavelDesembolso)
            .HasColumnName("RESPONSAVEL_DESEMBOLSO")
            .HasMaxLength(7);

        builder.Property(x => x.Gestor)
            .HasColumnName("GESTOR")
            .HasMaxLength(7);

        builder.Property(x => x.DtPrazo)
            .HasColumnName("DT_PRAZO")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.StatusDesembolso)
            .HasColumnName("CO_STATUS_DESEMBOLSO")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.DtConclusao)
            .HasColumnName("DT_CONCLUSAO")
            .HasColumnType("date");

        builder.HasOne(x => x.Desembolso)
            .WithOne(x => x.ControleDesembolso)
            .HasForeignKey<ControleDesembolso>(x => x.CoDesembolso)
            .HasConstraintName("FK_CAD_TB001_DESEMBOLSO");

        builder.HasMany(x => x.ValidacaoControleDesembolso)
            .WithOne(x => x.ControleDesembolso)
            .HasForeignKey(x => x.CoControleDesembolso)
            .HasConstraintName("FK_CAD_TB002_CONTROLE_DESEMBOLSO");
    }
}
