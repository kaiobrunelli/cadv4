using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class ValidacaoControleDesembolsoConfig : IEntityTypeConfiguration<ValidacaoControleDesembolso>
{
    public void Configure(EntityTypeBuilder<ValidacaoControleDesembolso> builder)
    {
        builder.ToTable("CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

        builder.HasKey(x => new
        {
            x.CoValidacao,
            x.CoControleDesembolso
        });

        builder.Property(x => x.CoValidacao)
            .HasColumnName("CO_VALIDACAO")
            .IsRequired();

        builder.Property(x => x.CoControleDesembolso)
            .HasColumnName("CO_CONTROLE_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.DeValidacao)
            .HasColumnName("DE_VALIDACAO")
            .HasMaxLength(500);

        builder.Property(x => x.CampoVinculado)
            .HasColumnName("CAMPO_VINCULADO")
            .HasMaxLength(100);

        builder.Property(x => x.Situacao)
            .HasColumnName("SITUACAO")
            .HasConversion<int>();

        builder.HasOne(x => x.Validacao)
            .WithMany(x => x.ValidacaoControleDesembolso)
            .HasForeignKey(x => x.CoValidacao)
            .HasConstraintName("FK_CAD_TB003_VALIDACAO");

        builder.HasOne(x => x.ControleDesembolso)
            .WithMany(x => x.ValidacaoControleDesembolso)
            .HasForeignKey(x => x.CoControleDesembolso)
            .HasConstraintName("FK_CAD_TB002_CONTROLE_DESEMBOLSO");
    }
}
