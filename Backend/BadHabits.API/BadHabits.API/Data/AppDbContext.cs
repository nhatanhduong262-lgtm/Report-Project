using BadHabits.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BadHabits.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Dòng này sẽ tạo ra một bảng tên là "Users" trong SQL Server
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
         public DbSet<Product> Products { get; set; }
         public DbSet<Variant> Variants { get; set; }

        // ĐÂY LÀ 2 BẢNG MỚI CHÚNG TA VỪA THÊM:
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // THÊM 2 BẢNG NÀY CHO GIỎ HÀNG:
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }
}