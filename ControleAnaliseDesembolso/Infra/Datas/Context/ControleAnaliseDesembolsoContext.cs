using ControleAnaliseDesembolso.Domain.Entitys;
using ControleAnaliseDesembolso.Infra.EntityContext;
using Microsoft.EntityFrameworkCore;

namespace ControleAnaliseDesembolso.Infra.Datas.Context
{
    public class ControleAnaliseDesembolsoContext(DbContextOptions<ControleAnaliseDesembolsoContext> options) : DbContext(options)
    {
        public DbSet<Validacao> Validacao { get; set; }
        public DbSet<Desembolso> Desembolso { get; set; }
        public DbSet<ControleDesembolso> ControleDesembolso { get; set; }
        public DbSet<ValidacaoControleDesembolso> ValidacaoControleDesembolso { get; set; }
        public DbSet<Mensagem> Mensagem { get; set; }
        public DbSet<TrilhaAuditoria> TrilhaAuditoria { get; set; }
        public DbSet<CampoConferencia> CampoConferencia { get; set; }
        public DbSet<ConferenciaControleDesembolso> ConferenciaControleDesembolso { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            new ValidacaoConfig().Configure(builder.Entity<Validacao>());
            new DesembolsoConfig().Configure(builder.Entity<Desembolso>());
            new ControleDesembolsoConfig().Configure(builder.Entity<ControleDesembolso>());
            new ValidacaoControleDesembolsoConfig().Configure(builder.Entity<ValidacaoControleDesembolso>());
            new MensagemConfig().Configure(builder.Entity<Mensagem>());
            new TrilhaAuditoriaConfig().Configure(builder.Entity<TrilhaAuditoria>());
            new CampoConferenciaConfig().Configure(builder.Entity<CampoConferencia>());
            new ConferenciaControleDesembolsoConfig().Configure(builder.Entity<ConferenciaControleDesembolso>());
        }
    }
}
