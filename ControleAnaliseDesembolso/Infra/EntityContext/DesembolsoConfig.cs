using ControleAnaliseDesembolso.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleAnaliseDesembolso.Infra.EntityContext;

public class DesembolsoConfig : IEntityTypeConfiguration<Desembolso>
{
    public void Configure(EntityTypeBuilder<Desembolso> builder)
    {
        builder.ToTable("CAD_TB001_DESEMBOLSO");

        builder.HasKey(x => x.CoDesembolso)
            .HasName("PK_CAD_TB001_DESEMBOLSO");

        builder.Property(x => x.CoDesembolso)
            .HasColumnName("CO_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.MatriculaSolicitante)
            .HasColumnName("MATRICULA_SOLICITANTE")
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(x => x.CoGigov)
            .HasColumnName("CO_GIGOV")
            .HasMaxLength(4)
            .IsFixedLength()
            .IsRequired();

        builder.Property(x => x.MatriculaGestor)
            .HasColumnName("MATRICULA_GESTOR")
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(x => x.DtSolicitado)
            .HasColumnName("DT_SOLICITADO")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.NuDesembolso)
            .HasColumnName("NU_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.CoContratoAf)
            .HasColumnName("CO_CONTRATO_AF")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CoContratoAfDv)
            .HasColumnName("CO_CONTRATO_AF_DV")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.ContratoAo)
            .HasColumnName("CO_CONTRATO_AO")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ContratoAoDv)
            .HasColumnName("CO_CONTRATO_AO_DV")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.PrimeiroDesembolso)
            .HasColumnName("PRIMEIRO_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.Recorrente)
            .HasColumnName("RECORRENTE")
            .IsRequired();

        builder.Property(x => x.AgenteFinanceiro)
            .HasColumnName("AGENTE_FINANCEIRO")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.CnpjAf)
            .HasColumnName("CNPJ_AF")
            .HasMaxLength(14)
            .IsFixedLength()
            .IsRequired();

        builder.Property(x => x.MutuarioFinal)
            .HasColumnName("MUTUARIO_FINAL")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.CnpjMutuarioFinal)
            .HasColumnName("CNPJ_MUTUARIO_FINAL")
            .HasMaxLength(14)
            .IsFixedLength()
            .IsRequired();

        builder.Property(x => x.AgenteTecnicoOperador)
            .HasColumnName("AGENTE_TECNICO_OPERADOR")
            .HasMaxLength(255);

        builder.Property(x => x.CnpjAgenteTecnicoOperador)
            .HasColumnName("CNPJ_AGENTE_TECNICO_OPERADOR")
            .HasMaxLength(14)
            .IsFixedLength();

        builder.Property(x => x.AgentePromotor)
            .HasColumnName("AGENTE_PROMOTOR")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.CnpjAgentePromotor)
            .HasColumnName("CNPJ_AGENTE_PROMOTOR")
            .HasMaxLength(14)
            .IsFixedLength()
            .IsRequired();

        builder.Property(x => x.Programa)
            .HasColumnName("CO_PROGRAMA")
            .IsRequired();

        builder.Property(x => x.UltimoDesembolso)
            .HasColumnName("ULTIMO_DESEMBOLSO");

        builder.Property(x => x.Funcionalidade)
            .HasColumnName("FUNCIONALIDADE");

        builder.Property(x => x.Concluido)
            .HasColumnName("CONCLUIDO");

        builder.Property(x => x.DtEngenharia)
            .HasColumnName("DT_ENGENHARIA")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.SituacaoObra)
            .HasColumnName("CO_SITUACAO_OBRA");

        builder.Property(x => x.DtSocioAmbiental)
            .HasColumnName("DT_SOCIO_AMBIENTAL")
            .HasColumnType("date");

        builder.Property(x => x.PercentualObra)
            .HasColumnName("PERCENTUAL_OBRA")
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(x => x.TipoDesembolso)
            .HasColumnName("CO_TIPO_DESEMBOLSO")
            .IsRequired();

        builder.Property(x => x.RetornoParcial)
            .HasColumnName("RETORNO_PARCIAL");

        builder.Property(x => x.PlacaLocal)
            .HasColumnName("PLACA_LOCAL");

        builder.Property(x => x.LicensaInstalacao)
            .HasColumnName("LICENSA_INSTALACAO");

        builder.Property(x => x.LicensaOperacao)
            .HasColumnName("LICENSA_OPERACAO");

        builder.Property(x => x.CndValido)
            .HasColumnName("CND_VALIDO");

        builder.Property(x => x.CrpValido)
            .HasColumnName("CRP_VALIDO");

        builder.Property(x => x.CrpNsa)
            .HasColumnName("CRP_NSA")
            .IsRequired();

        builder.Property(x => x.SolicitadoVi)
            .HasColumnName("SOLICITADO_VI")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.GlossadoVi)
            .HasColumnName("GLOSSADO_VI")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.AceitoVi)
            .HasColumnName("ACEITO_VI")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.ParticipacaoFgts)
            .HasColumnName("PARTICIPACAO_FGTS")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Contrapartida)
            .HasColumnName("CONTRAPARTIDA")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.ValorEmprestimo)
            .HasColumnName("VALOR_EMPRESTIMO")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Desembolsado)
            .HasColumnName("DESEMBOLSADO")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.SaldoDesembolsar)
            .HasColumnName("SALDO_DESEMBOLSAR")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Excepcionalizado)
            .HasColumnName("EXCEPCIONALIZADO");

        builder.Property(x => x.ContrapartidaAtual)
            .HasColumnName("CONTRAPARTIDA_ATUAL")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Integralizado)
            .HasColumnName("INTEGRALIZADO")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.SaldoIntegralizar)
            .HasColumnName("SALDO_INTEGRALIZAR")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.ContrapartidaAlterada)
            .HasColumnName("CONTRAPARTIDA_ALTERADA");

        builder.Property(x => x.Sanepar)
            .HasColumnName("SANEPAR");

        builder.Property(x => x.Mensagem)
            .HasColumnName("MENSAGEM")
            .HasMaxLength(3000);

        builder.Property(x => x.MensagemCefga)
            .HasColumnName("MENSAGEM_CEFGA")
            .HasMaxLength(3000);

        builder.Property(x => x.MotivoRejeicao)
            .HasColumnName("MOTIVO_REJEICAO")
            .HasMaxLength(3000);

        builder.Property(x => x.TemCarroceria)
            .HasColumnName("TEM_CARROCERIA");

        builder.Property(x => x.VeiculoPossuiAdesivos)
            .HasColumnName("VEICULO_POSSUI_ADESIVOS");

        builder.Property(x => x.DataInicioObra)
            .HasColumnName("DATA_INICIO_OBRA")
            .HasColumnType("date");

        builder.Property(x => x.DestinacaoColetaResiduosSolidos)
            .HasColumnName("DESTINACAO_COLETA_RESIDUOS_SOLIDOS");

        builder.HasOne(x => x.ControleDesembolso)
            .WithOne(x => x.Desembolso)
            .HasForeignKey<ControleDesembolso>(x => x.CoDesembolso)
            .HasConstraintName("FK_CAD_TB001_DESEMBOLSO");
    }
}
