using System.Data.Entity;

namespace MeetingsLibrary
{
    public class MeetingContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<MeetingContext>(null);
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Negotiation> Negotiations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Venue> Venues { get; set; }
    }
}
