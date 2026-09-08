using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class MensagemConfig : IEntityTypeConfiguration<Mensagem>
{
    public void Configure(EntityTypeBuilder<Mensagem> builder)
    {
        builder.ToTable("CAD_TB005_MENSAGEM");

        builder.HasKey(x => x.CoMensagem)
            .HasName("PK_CAD_TB005_MENSAGEM");

        builder.Property(x => x.CoMensagem)
            .HasColumnName("CO_MENSAGEM")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.CoValidacao)
            .HasColumnName("CO_VALIDACAO")
            .IsRequired();

        builder.Property(x => x.CoControleDesembolso)
            .HasColumnName("CO_CONTROLE_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.DeMensagem)
            .HasColumnName("DE_MENSAGEM")
            .HasMaxLength(3000)
            .IsUnicode(false);

        builder.Property(x => x.TipoMensagem)
            .HasColumnName("CO_TIPO_MENSAGEM")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CoUsuario)
            .HasColumnName("CO_USUARIO")
            .HasMaxLength(7)
            .IsUnicode(false);

        // Banco real: VARCHAR(100) — não 255.
        builder.Property(x => x.DeUsuario)
            .HasColumnName("DE_USUARIO")
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.UnidadeUsuario)
            .HasColumnName("UNIDADE_USUARIO")
            .IsRequired();

        builder.Property(x => x.DtCriacao)
            .HasColumnName("DT_CRIACAO")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(x => x.Ativo)
            .HasColumnName("ATIVO")
            .HasDefaultValue(true)
            .IsRequired();
    }
}
