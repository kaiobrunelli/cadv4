using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class ValidacaoControleDesembolsoConfig : IEntityTypeConfiguration<ValidacaoControleDesembolso>
{
    public void Configure(EntityTypeBuilder<ValidacaoControleDesembolso> builder)
    {
        builder.ToTable("CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

        builder.HasKey(x => x.CoValidacaoControle)
            .HasName("PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

        builder.Property(x => x.CoValidacaoControle)
            .HasColumnName("CO_VALIDACAO_CONTROLE")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.CoValidacao)
            .HasColumnName("CO_VALIDACAO")
            .IsRequired();

        builder.Property(x => x.CoControleDesembolso)
            .HasColumnName("CO_CONTROLE_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.DeValidacao)
            .HasColumnName("DE_VALIDACAO")
            .HasMaxLength(500)
            .IsUnicode(false);

        builder.Property(x => x.CampoVinculado)
            .HasColumnName("CAMPO_VINCULADO")
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.Situacao)
            .HasColumnName("SITUACAO")
            .HasConversion<int>();

        builder.Property(x => x.Origem)
            .HasColumnName("ORIGEM")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Mensagem)
            .HasColumnName("MENSAGEM")
            .HasMaxLength(1000)
            .IsUnicode(false);

        builder.Property(x => x.DtValidacao)
            .HasColumnName("DT_VALIDACAO")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasIndex(x => new { x.CoValidacao, x.CoControleDesembolso })
            .HasDatabaseName("IX_CAD_TB004_VALIDACAO_CONTROLE");

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
