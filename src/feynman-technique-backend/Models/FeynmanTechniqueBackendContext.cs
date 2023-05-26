using Microsoft.EntityFrameworkCore;

namespace FeynmanTechniqueBackend.Models
{
    public class FeynmanTechniqueBackendContext : DbContext
    {
        private readonly IConfiguration Configuration;

        public virtual DbSet<Word> Words { get; set; }
        public virtual DbSet<PartOfSpeech> PartOfSpeeches { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        public FeynmanTechniqueBackendContext(DbContextOptions options, IConfiguration configuration) 
            : base(options) 
        { 
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Configuration.GetConnectionString("FtBackendDatabase");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}
