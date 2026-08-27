using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlataformaNotificacao.Infra.Context;

#nullable disable

namespace PlataformaNotificacao.Infra.Data.Migrations
{
    [DbContext(typeof(PlataformaNotificacaoContext))]
    partial class PlataformaNotificacaoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PlataformaNotificacao.Domain.ControleVisualizacao", b =>
                {
                    b.Property<int>("CodigoVisualizacao")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CO_VISUALIZACAO");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CodigoVisualizacao"));

                    b.Property<int>("CodigoNotificacao")
                        .HasColumnType("int")
                        .HasColumnName("CO_NOTIFICACAO");

                    b.Property<string>("CodigoUsuario")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)")
                        .HasColumnName("CO_USUARIO");

                    b.Property<DateTime?>("DataVisualizacao")
                        .HasColumnType("datetime2")
                        .HasColumnName("DT_VISUALIZACAO");

                    b.Property<string>("Link")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("LINK");

                    b.HasKey("CodigoVisualizacao");

                    b.HasIndex("CodigoNotificacao");

                    b.ToTable("PLA_NOT_TB002_CONTROLE_VISUALIZACAO", (string)null);
                });

            modelBuilder.Entity("PlataformaNotificacao.Domain.EmpregadoAtivo", b =>
                {
                    b.Property<int?>("Cgc")
                        .HasColumnType("int")
                        .HasColumnName("CGC");

                    b.Property<int>("CodigoEmpregado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CO_EMPREGADO");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CodigoEmpregado"));

                    b.Property<int?>("CodigoEventual")
                        .HasColumnType("int")
                        .HasColumnName("CO_EVENTUAL");

                    b.Property<int?>("CodigoFuncao")
                        .HasColumnType("int")
                        .HasColumnName("CO_FUNCAO");

                    b.Property<int?>("CodigoSituacao")
                        .HasColumnType("int")
                        .HasColumnName("CO_SITUACAO");

                    b.Property<string>("Coordenacao")
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)")
                        .HasColumnName("COORDENACAO");

                    b.Property<DateTime?>("DataAdmissao")
                        .HasColumnType("date")
                        .HasColumnName("DATA_ADMISSAO");

                    b.Property<DateTime?>("DataEntrada")
                        .HasColumnType("date")
                        .HasColumnName("DATA_ENTRADA");

                    b.Property<DateTime?>("DataNascimento")
                        .HasColumnType("date")
                        .HasColumnName("DATA_NASCIMENTO");

                    b.Property<string>("Matricula")
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)")
                        .HasColumnName("MATRICULA");

                    b.Property<int?>("MatriculaDv")
                        .HasColumnType("int")
                        .HasColumnName("MATRICULA_DV");

                    b.Property<string>("Nome")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasColumnName("NOME");

                    b.Property<int?>("TermoLgpd")
                        .HasColumnType("int")
                        .HasColumnName("TERMO_LGPD");

                    b.ToTable("RH_TB003_EMPREGADOS_ATIVOS", (string)null);
                });

            modelBuilder.Entity("PlataformaNotificacao.Domain.EmpregadoGigov", b =>
                {
                    b.Property<int>("CodigoEmpregado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CO_EMPREGADO");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CodigoEmpregado"));

                    b.Property<bool>("Ativo")
                        .HasColumnType("bit")
                        .HasColumnName("ATIVO");

                    b.Property<string>("CodigoGigov")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("CO_GIGOV");

                    b.Property<string>("Matricula")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)")
                        .HasColumnName("MATRICULA");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasColumnName("NOME");

                    b.HasKey("CodigoEmpregado");

                    b.ToTable("PLA_NOT_TB003_EMPREGADOS_GIGOV", (string)null);
                });

            modelBuilder.Entity("PlataformaNotificacao.Domain.Notificacao", b =>
                {
                    b.Property<int>("CodigoNotificacao")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CO_NOTIFICACAO");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CodigoNotificacao"));

                    b.Property<int?>("CodigoAplicativo")
                        .HasColumnType("int")
                        .HasColumnName("CO_APLICATIVO");

                    b.Property<string>("CodigoUsuarioEmissor")
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)")
                        .HasColumnName("CO_USUARIO_EMISSOR");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2")
                        .HasColumnName("DT_CRIACAO");

                    b.Property<DateTime?>("DataValidade")
                        .HasColumnType("datetime2")
                        .HasColumnName("DT_VALIDADE");

                    b.Property<string>("Mensagem")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("MENSAGEM");

                    b.Property<int>("Tipo")
                        .HasColumnType("int")
                        .HasColumnName("TIPO");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("TITULO");

                    b.HasKey("CodigoNotificacao");

                    b.ToTable("PLA_NOT_TB001_NOTIFICACAO", (string)null);
                });

            modelBuilder.Entity("PlataformaNotificacao.Domain.ControleVisualizacao", b =>
                {
                    b.HasOne("PlataformaNotificacao.Domain.Notificacao", "Notificacao")
                        .WithMany("Destinatarios")
                        .HasForeignKey("CodigoNotificacao")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Notificacao");
                });

            modelBuilder.Entity("PlataformaNotificacao.Domain.Notificacao", b =>
                {
                    b.Navigation("Destinatarios");
                });
#pragma warning restore 612, 618
        }
    }
}
