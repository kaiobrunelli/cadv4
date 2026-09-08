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
            .HasMaxLength(7)
            .IsUnicode(false);

        builder.Property(x => x.ResponsavelBaixa)
            .HasColumnName("RESPONSAVEL_BAIXA")
            .HasMaxLength(7)
            .IsUnicode(false);

        builder.Property(x => x.ResponsavelDesembolso)
            .HasColumnName("RESPONSAVEL_DESEMBOLSO")
            .HasMaxLength(7)
            .IsUnicode(false);

        builder.Property(x => x.Gestor)
            .HasColumnName("GESTOR")
            .HasMaxLength(7)
            .IsUnicode(false);

        builder.Property(x => x.DtPrazo)
            .HasColumnName("DT_PRAZO")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.StatusDesembolso)
            .HasColumnName("CO_STATUS_DESEMBOLSO")
            .IsRequired()
            .HasConversion<int>();

        // Banco real: DATETIME — não DATE (precisa preservar a hora de DateTime.Now).
        builder.Property(x => x.DtConclusao)
            .HasColumnName("DT_CONCLUSAO")
            .HasColumnType("datetime");

        builder.Property(x => x.MotivoCancelamento)
            .HasColumnName("MOTIVO_CANCELAMENTO")
            .HasMaxLength(3000)
            .IsUnicode(false);

        builder.Property(x => x.DtUltimaConferencia)
            .HasColumnName("DT_ULTIMA_CONFERENCIA")
            .HasColumnType("datetime");

        // NUMERO_DRP/DV_DRP/SENHA_DRP/CRF_* não existem no script original — são
        // colunas adicionadas por nós (ver ajustes-v5-mover-drp-crf-para-tb002.sql),
        // por isso continuam NVARCHAR (não seguem a convenção VARCHAR do resto da tabela).
        builder.Property(x => x.NumeroDrp)
            .HasColumnName("NUMERO_DRP")
            .HasMaxLength(20);

        builder.Property(x => x.DvDrp)
            .HasColumnName("DV_DRP")
            .HasMaxLength(5);

        builder.Property(x => x.SenhaDrp)
            .HasColumnName("SENHA_DRP")
            .HasMaxLength(20);

        builder.Property(x => x.DtDrp)
            .HasColumnName("DT_DRP")
            .HasColumnType("date");

        builder.Property(x => x.CrfAf)
            .HasColumnName("CRF_AF")
            .HasColumnType("date");

        builder.Property(x => x.CrfTomador)
            .HasColumnName("CRF_TOMADOR")
            .HasColumnType("date");

        builder.Property(x => x.CrfAp)
            .HasColumnName("CRF_AP")
            .HasColumnType("date");

        builder.Property(x => x.CrfAt)
            .HasColumnName("CRF_AT")
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
