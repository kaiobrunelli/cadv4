using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaNotificacao.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaNotificacao.Infra.EntityConfig
{
    public class NotificacaoConfig : IEntityTypeConfiguration<Notificacao>
    {

        public void Configure(EntityTypeBuilder<Notificacao> builder)
        {

            builder.ToTable("PLA_NOT_TB001_NOTIFICACAO");

            builder.HasKey(x => x.CodigoNotificacao);

            builder.Property(x => x.CodigoNotificacao)
                .HasColumnName("CO_NOTIFICACAO");

            builder.Property(x => x.CodigoAplicativo)
                .HasColumnName("CO_APLICATIVO");

            builder.Property(x => x.CodigoUsuarioEmissor)
                .HasColumnName("CO_USUARIO_EMISSOR")
                .HasMaxLength(7);

            builder.Property(x => x.Titulo)
                .HasColumnName("TITULO")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Mensagem)
                .HasColumnName("MENSAGEM")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Tipo)
                .HasColumnName("TIPO")
                .IsRequired();

            builder.Property(x => x.DataCriacao)
                .HasColumnName("DT_CRIACAO")
                .IsRequired();

            builder.Property(x => x.DataValidade)
                .HasColumnName("DT_VALIDADE");

        }
    }

    public class NotificacaoUsuario
    {

    }
}