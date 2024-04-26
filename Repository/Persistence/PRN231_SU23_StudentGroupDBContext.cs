using Microsoft.EntityFrameworkCore;
using Repository.Models.Entities;

namespace Repository.Persistence
{
    public partial class PRN231_SU23_StudentGroupDBContext : DbContext
    {
        public PRN231_SU23_StudentGroupDBContext()
        {
        }

        public PRN231_SU23_StudentGroupDBContext(DbContextOptions<PRN231_SU23_StudentGroupDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<StudentGroup> StudentGroups { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.FullName).HasMaxLength(250);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK__Student__GroupId__4E88ABD4");
            });

            modelBuilder.Entity<StudentGroup>(entity =>
            {
                entity.ToTable("StudentGroup");

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.GroupName).HasMaxLength(250);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");

                entity.HasIndex(e => e.Username, "UQ__UserRole__536C85E45B15B616")
                    .IsUnique();

                entity.Property(e => e.Passphrase)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserRole1).HasColumnName("UserRole");

                entity.Property(e => e.Username).HasMaxLength(250);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
