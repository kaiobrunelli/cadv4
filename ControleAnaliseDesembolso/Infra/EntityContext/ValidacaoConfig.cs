using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class ValidacaoConfig : IEntityTypeConfiguration<Validacao>
{
    public void Configure(EntityTypeBuilder<Validacao> builder)
    {
        builder.ToTable("CAD_TB003_VALIDACAO");

        builder.HasKey(x => x.CoValidacao)
            .HasName("PK_CAD_TB003_VALIDACAO");

        builder.Property(x => x.CoValidacao)
            .HasColumnName("CO_VALIDACAO")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.DeValidacao)
            .HasColumnName("DE_VALIDACAO")
            .HasMaxLength(500);

        builder.Property(x => x.DtCriacao)
            .HasColumnName("DT_CRIACAO")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(x => x.CampoVinculado)
            .HasColumnName("CAMPO_VINCULADO")
            .HasMaxLength(100);

        builder.Property(x => x.UsuarioExclusao)
            .HasColumnName("USUARIO_EXCLUSAO")
            .HasMaxLength(7);

        builder.Property(x => x.DtExclusao)
            .HasColumnName("DT_EXCLUSAO")
            .HasColumnType("datetime");

        builder.Property(x => x.Desativado)
            .HasColumnName("DESATIVADO")
            .IsRequired();

        builder.HasMany(x => x.ValidacaoControleDesembolso)
            .WithOne(x => x.Validacao)
            .HasForeignKey(x => x.CoValidacao)
            .HasConstraintName("FK_CAD_TB003_VALIDACAO");
    }
}
