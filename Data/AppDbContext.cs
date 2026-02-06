using Microsoft.EntityFrameworkCore;
using NET_MVC.Models;

namespace NET_MVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Keeper> Keepers { get; set; }
        public DbSet<AnimalKeeper> AnimalKeepers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Định nghĩa khóa chính kết hợp cho bảng trung gian
            modelBuilder.Entity<AnimalKeeper>().HasKey(ak => new { ak.AnimalId, ak.KeeperId });
            // Cấu hình quan hệ để EF hiểu cách join
            modelBuilder.Entity<AnimalKeeper>() // (0) Xét bảng trung gian AnimalKeeper
            .HasOne(ak => ak.Animal) // (1) Mỗi dòng trong bảng này CHỈ liên kết với 1 con vật
            .WithMany(a => a.AnimalKeepers) // (2) Nhưng 1 con vật có thể xuất hiện trong NHIỀU dòng ở bảng này
            .HasForeignKey(ak => ak.AnimalId); // (3) Và dùng cột AnimalId để làm "chìa khóa" tìm nhau

            modelBuilder.Entity<AnimalKeeper>()
                .HasOne(ak => ak.Keeper) // (1) Mỗi dòng trong bảng này CHỈ liên kết với 1 người
                .WithMany(k => k.AnimalKeepers) // (2) Nhưng 1 người có thể xuất hiện trong NHIỀU dòng ở bảng này
                .HasForeignKey(ak => ak.KeeperId);// (3) Và dùng cột KeeperId để làm "chìa khóa" tìm nhau
        }
    }
}
