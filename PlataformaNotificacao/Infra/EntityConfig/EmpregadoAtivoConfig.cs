using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaNotificacao.Domain;

namespace PlataformaNotificacao.Infra.EntityConfig
{
    public class EmpregadoAtivoConfig : IEntityTypeConfiguration<EmpregadoAtivo>
    {
        public void Configure(EntityTypeBuilder<EmpregadoAtivo> builder)
        {
            builder.ToTable("RH_TB003_EMPREGADOS_ATIVOS");

            builder.HasNoKey();

            builder.Property(x => x.CodigoEmpregado)
                .HasColumnName("CO_EMPREGADO")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Matricula)
                .HasColumnName("MATRICULA")
                .HasMaxLength(7);

            builder.Property(x => x.MatriculaDv)
                .HasColumnName("MATRICULA_DV");

            builder.Property(x => x.Nome)
                .HasColumnName("NOME")
                .HasMaxLength(150);

            builder.Property(x => x.DataAdmissao)
                .HasColumnName("DATA_ADMISSAO")
                .HasColumnType("date");

            builder.Property(x => x.DataNascimento)
                .HasColumnName("DATA_NASCIMENTO")
                .HasColumnType("date");

            builder.Property(x => x.Cgc)
                .HasColumnName("CGC");

            builder.Property(x => x.CodigoFuncao)
                .HasColumnName("CO_FUNCAO");

            builder.Property(x => x.TermoLgpd)
                .HasColumnName("TERMO_LGPD");

            builder.Property(x => x.CodigoEventual)
                .HasColumnName("CO_EVENTUAL");

            builder.Property(x => x.Coordenacao)
                .HasColumnName("COORDENACAO")
                .HasMaxLength(7);

            builder.Property(x => x.DataEntrada)
                .HasColumnName("DATA_ENTRADA")
                .HasColumnType("date");

            builder.Property(x => x.CodigoSituacao)
                .HasColumnName("CO_SITUACAO");
        }
    }
}
