using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using System.Data.Entity.Infrastructure;

namespace DIBZ.Data
{
    public class DIBZDbContext : DbContext
    {
        public DbSet<LoginSession> LoginSessions { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<GameCatalog> GameCatalogs { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Format> Formats { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<CounterOffer> CounterOffers { get; set; }
        public DbSet<Swap> Swaps { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }
        public DbSet<NewsFeed> NewsFeeds { get; set; }
        public DbSet<NonWorkingDay> NonWorkingDays { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MyQueries> MyQueries { get; set; }
        public DbSet<Banners> Banners { get; set; }
        public DbSet<Competition> Competition { get; set; }
        public DbSet<DibzCharges> DibzCharges { get; set; }
        public DbSet<MyQueryDetails> queryLogs { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<DIBZLocation> DIBZLocations { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }
        public DbSet<NotifierEmail> NotifierEmails { get; set; }
        public DIBZDbContext() : base("DIBZDbContext")
        {
            //Database.SetInitializer<DIBZDbContext>(null);
            //// var ensureDllIsCopied = Npgsql.NpgsqlServices.Instance;
            //var ensure2 = Npgsql.SslMode.Prefer;
            //Debug.WriteLine(ensure2);
        }
        //public DIBZDbContext(): base()       
        //{            
        //    Database.SetInitializer<DIBZDbContext>(null);
        //    //  var ensureDllIsCopied = Npgsql.NpgsqlServices.Instance;
        //    var ensure2 = Npgsql.SslMode.Prefer;
        //    Debug.WriteLine(ensure2);
        //}

        // If you want to avoid EF running this check you can add this to your DbContext class
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region OneToMany

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(c => c.LoginSessions)
                .WithOptional(l => l.ApplicationUser)
                .HasForeignKey(l => l.ApplicationUserId);

            modelBuilder.Entity<GameCatalog>()
            .HasRequired(b => b.GameImage)
            .WithMany().HasForeignKey(b => b.GameImageId);

            modelBuilder.Entity<ApplicationUser>()
             .HasMany(c => c.Offers)
              .WithRequired(c => c.ApplicationUser)
              .HasForeignKey(c => c.ApplicationUserId);

            modelBuilder.Entity<GameCatalog>()
             .HasMany(c => c.Offers)
              .WithRequired(c => c.GameCatalog)
              .HasForeignKey(c => c.GameCatalogId);

            modelBuilder.Entity<ApplicationUser>()
             .HasOptional(p => p.Scorecard);

            modelBuilder.Entity<Offer>()
             .HasMany(c => c.Swaps)
              .WithRequired(c => c.Offer)
              .HasForeignKey(c => c.OfferId);

            modelBuilder.Entity<Swap>()
            .HasRequired(p => p.GameSwapPserson);

            modelBuilder.Entity<Swap>()
           .HasRequired(p => p.GameSwapWith);

            modelBuilder.Entity<Offer>()
            .HasMany(c => c.CounterOffers)
             .WithRequired(c => c.Offer)
             .HasForeignKey(c => c.OfferId);

            modelBuilder.Entity<ApplicationUser>()
            .HasOptional(pi => pi.ProfileImage);

            modelBuilder.Entity<Offer>()
             .HasMany(c => c.Transactions)
              .WithRequired(c => c.Offer)
              .HasForeignKey(c => c.OfferId);

            modelBuilder.Entity<MyQueries>()
            .HasMany(c => c.querylog)
             .WithRequired(c => c.myquery)
             .HasForeignKey(c => c.MyQueryId);

            #endregion

            #region Many to Many


            modelBuilder.Entity<GameCatalog>()
               .HasMany(c => c.ApplicationUsers)
               .WithMany(c => c.GameCatalogs)
               .Map((cfg) =>
               {
                   cfg.ToTable("ApplicationUser_Game");
               });

            #endregion

        }



    }
}
