using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class TrilhaAuditoriaConfig : IEntityTypeConfiguration<TrilhaAuditoria>
{
    public void Configure(EntityTypeBuilder<TrilhaAuditoria> builder)
    {
        builder.ToTable("CAD_TB000_TRILHA_AUDITORIA");

        builder.HasKey(c => c.CoTrilhaAuditoria)
            .HasName("PK_CAD_TB000_TRILHA_AUDITORIA");

        builder.Property(c => c.CoTrilhaAuditoria)
            .HasColumnName("ID_SOLICITACAO")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(c => c.Usuario)
            .HasColumnName("USUARIO")
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(c => c.EnderecoLogicoSolicitante)
            .HasColumnName("ENDERECO_LOGICO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.DataSolicitacao)
            .HasColumnName("DT_SOLICITACAO")
            .IsRequired();

        builder.Property(c => c.Evento)
            .HasColumnName("EVENTO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.DescEvento)
            .HasColumnName("DESC_EVENTO")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.Resposta)
            .HasColumnName("RESPOSTA")
            .HasMaxLength(20);
    }
}
