using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChefMeet.Models;

namespace ChefMeet.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Chef> Chefs { get; set; }
        public DbSet<Creazione> Creazioni { get; set; }
        public DbSet<Evento> Eventi { get; set; }
        public DbSet<Prenotazione> Prenotazioni { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<DisponibilitaChef> DisponibilitaChef { get; set; }
        public DbSet<PrenotazioneDisponibilita> PrenotazioniDisponibilita { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1:1 ApplicationUser → Chef
            builder.Entity<Chef>()
                .HasOne(c => c.Utente)
                .WithOne(u => u.Chef)
                .HasForeignKey<Chef>(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Like → Creazione
            builder.Entity<Like>()
                .HasOne(l => l.Creazione)
                .WithMany(c => c.Likes)
                .HasForeignKey(l => l.CreazioneId)
                .OnDelete(DeleteBehavior.NoAction); 

            // Like → Utente
            builder.Entity<Like>()
                .HasOne(l => l.Utente)
                .WithMany()
                .HasForeignKey(l => l.UtenteId)
                .OnDelete(DeleteBehavior.NoAction); 

            // Prenotazione → Evento
            builder.Entity<Prenotazione>()
                .HasOne(p => p.Evento)
                .WithMany(e => e.Prenotazioni)
                .HasForeignKey(p => p.EventoId)
                .OnDelete(DeleteBehavior.NoAction); 

            // Prenotazione → Utente
            builder.Entity<Prenotazione>()
                .HasOne(p => p.Utente)
                .WithMany()
                .HasForeignKey(p => p.UtenteId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.Entity<Creazione>()
                .HasOne(c => c.Creatore)
                .WithMany()
                .HasForeignKey(c => c.CreatoreId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
