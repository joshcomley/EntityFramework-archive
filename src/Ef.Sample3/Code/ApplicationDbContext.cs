using System;
using System.Linq;
using System.Threading.Tasks;
using EfSample9.Hazception.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebApplication2.Entities;

namespace WebApplication2.Hazception
{
    /// <summary>
    /// Unsecured implementation of ApplicationDbContext
    /// </summary>
	public class ApplicationDbContext : DbContext,
        IHazceptionService
    {
        private readonly IServiceProvider _serviceProvider;
        //public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Hazard> Hazards { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<ExamCandidate> ExamCandidates { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<ExamCandidateResult> ExamCandidateResults { get; set; }
        public DbSet<ClientType> ClientTypes { get; set; }
        //public DbSet<OpenIddictApplication> OpenIddictApplications { get; set; }

        IQueryable<ClientType> IHazceptionService.ClientTypes => ClientTypes;
        IQueryable<ApplicationUser> IHazceptionService.Users => Users;
        //IQueryable<ApplicationRole> IHazceptionService.Roles => Roles;
        IQueryable<ExamCandidate> IHazceptionService.ExamCandidates => ExamCandidates;
        IQueryable<Exam> IHazceptionService.Exams => Exams;
        IQueryable<ExamResult> IHazceptionService.ExamResults => ExamResults;
        IQueryable<ExamCandidateResult> IHazceptionService.ExamCandidateResults => ExamCandidateResults;
        IQueryable<Client> IHazceptionService.Clients => Clients;
        IQueryable<Video> IHazceptionService.Videos => Videos;
        IQueryable<Hazard> IHazceptionService.Hazards => Hazards;

        // Uncomment this when running migrations
        //public ApplicationDbContext()
        //{

        //}

        public ApplicationDbContext(IServiceProvider serviceProvider, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _serviceProvider = serviceProvider;
            // TODO: #639
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(
                //Services.App.Instance?.DbConnectionString ??
                // When running migrations the connection string details don't actually matter
                Startup.ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            //builder.Entity<Client>().HasOne(u => u.CreatedByUser);
            builder.Entity<Video>().Ignore(t => t.ScreenshotUrl);
            builder.Entity<Video>().Ignore(t => t.ScreenshotMiniUrl);
            builder.Entity<ApplicationUser>().ForSqlServerToTable("AspNetUsers");
            builder.Entity<ApplicationRole>()
                .HasKey(r => r.Id);

            builder.Entity<Exam>()
                .HasFilter<ExamsQueryFilter>();

            builder.Entity<Video>()
                .HasFilter<VideosFilter>()
                ;

            builder
                .Entity<ExamCandidate>()
                .HasKey(c => c.Id)
                ;
            builder
                .Entity<ExamCandidate>()
                .HasAlternateKey(c => new { c.ExamId, c.CandidateId });
            builder
                .Entity<ExamCandidate>()
                .HasMany(c => c.Results)
                .WithOne(r => r.ExamCandidate)
                .OnDelete(DeleteBehavior.Restrict)
                ;
            builder
                .Entity<ExamCandidateResult>()
                .HasOne(c => c.ExamCandidate)
                .WithMany(r => r.CandidateResults)
                .OnDelete(DeleteBehavior.Restrict)
                ;
            builder
                .Entity<ExamCandidateResult>()
                .HasMany(c => c.Results)
                .WithOne(r => r.CandidateResult)
                .OnDelete(DeleteBehavior.Restrict)
                ;
            //builder.UseOpenIddict();
            // LinkUserAndExamCandidateResult

            builder
                .Entity<ApplicationUser>()
                .HasMany(c => c.Results)
                .WithOne(r => r.Candidate)
                .OnDelete(DeleteBehavior.Restrict)
                ;
            builder
                .Entity<ApplicationUser>()
                //.Ignore(u => u.Logins)
                //.Ignore(u => u.Roles)
                //.Ignore(u => u.Claims)
                ;
            builder
                .Entity<ExamCandidateResult>()
                .HasKey(c => c.Id)
                ;
            builder
                .Entity<Exam>()
                .HasMany(c => c.CandidateResults)
                .WithOne(r => r.Exam)
                .OnDelete(DeleteBehavior.Restrict)
                ;
            builder
                .Entity<Video>()
                .HasMany(c => c.CandidateResults)
                .WithOne(r => r.Video)
                .OnDelete(DeleteBehavior.Restrict)
                ;
            builder
                .Entity<Video>()
                .HasMany(c => c.Results)
                .WithOne(r => r.Video)
                .OnDelete(DeleteBehavior.Restrict)
                ;

            //builder
            //	.Entity<ExamCandidate>()
            //	.HasKey(c => c.Id);

            builder
                .UseServiceProviderForFilters(_serviceProvider);

            builder
                .Entity<ApplicationUser>()
                .HasOne(u => u.Client)
                .WithMany(c => c.Users);

            builder
                .Entity<Exam>()
                .HasMany(c => c.Candidates)
                .WithOne(c => c.Exam)
                .HasForeignKey(c => c.ExamId)
                .IsRequired();

            builder
                .Entity<Client>()
                .Property(c => c.TypeId)
                .HasDefaultValue(1)
                ;

            builder
                .Entity<ExamResult>()
                .HasOne(h => h.Candidate)
                .WithMany(u => u.Results)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<ExamCandidate>()
                .HasOne(c => c.Video)
                .WithMany(c => c.Candidates)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<ExamCandidate>()
            //	.HasOne(ec => ec.Exam)
            //	.WithMany(ex => ex.Candidacies)
            //	.HasForeignKey(c => c.CandidateId)
            //	.IsRequired();

            builder
                .Entity<ApplicationUser>()
                .HasMany(c => c.Exams)
                .WithOne(c => c.Candidate)
                .HasForeignKey(c => c.CandidateId)
                .IsRequired();

            builder
                .Entity<Hazard>()
                .HasMany(h => h.Results)
                .WithOne(r => r.Hazard);

            builder.Entity<Client>().HasAlternateKey(p => p.Guid);
            builder.Entity<Video>().HasAlternateKey(p => p.Guid);
            builder.Entity<Hazard>().HasAlternateKey(p => p.Guid);
            builder.Entity<Exam>().HasAlternateKey(p => p.Guid);
            //builder.Entity<Video>()
            //    .HasMany(p => p.Hazards)
            //    .WithOne(c => c.Video)
            //    .HasForeignKey(v => v.VideoGuid);
            builder.Entity<Client>().Property(p => p.Guid).HasDefaultValueSql("newid()");
            builder.Entity<Video>().Property(p => p.Guid).HasDefaultValueSql("newid()");
            builder.Entity<Hazard>().Property(p => p.Guid).HasDefaultValueSql("newid()");
            builder.Entity<Exam>().Property(p => p.Guid).HasDefaultValueSql("newid()");
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        private static bool _databaseChecked;
        // The following code creates the database and schema if they don't exist.
        // This is a temporary workaround since deploying database through EF migrations is
        // not yet supported in this release.
        // Please see this http://go.microsoft.com/fwlink/?LinkID=615859 for more information on how to do deploy the database
        // when publishing your application.
        public void EnsureDatabaseCreated()
        {
            if (!_databaseChecked)
            {
                _databaseChecked = true;
                Database.EnsureCreated();
            }
        }

        public async Task UpdateVideoDependencyCounts(int videoId)
        {
            var video = Videos.Single(v => v.Id == videoId);
            video.HazardCount = Hazards.Count(h => h.VideoId == video.Id);
            video.ExamCount = Exams.Count(h => h.VideoId == video.Id);
            video.ResultsCount = ExamResults.Count(h => h.VideoId == video.Id);
            video.CandidatesCount = ExamCandidates.Count(h => h.VideoId == video.Id);
            video.CandidateResultsCount = ExamCandidateResults.Count(h => h.VideoId == video.Id);
            await SaveChangesAsync();
        }
    }
}