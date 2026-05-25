using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalR.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.DataAccessLayer.concrete
{
    public class SignalRContext: IdentityDbContext<AppUser,AppRole,int>
    {
        public SignalRContext() { }
        public SignalRContext(DbContextOptions<SignalRContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Veritabanı bağlantı dizesi yapılandırılmamış. " +
                    "'ConnectionStrings__DefaultConnection' ortam değişkenini ayarlayın.");

            optionsBuilder.UseSqlServer(connectionString);
        }
        public DbSet<About>Abouts { get; set; }
        public DbSet<Booking>Bookings { get; set; }
        public DbSet<Category>Categories { get; set; }
        public DbSet<Contact>Contacts { get; set; }
        public DbSet<Discount>Discounts { get; set; }
        public DbSet<Feature>Features { get; set; }
        public DbSet<Product>Products { get; set; }
        public DbSet<SocialMedia>SocialMedias { get; set; }
        public DbSet<Testimonial>Testimonials { get; set; }
        public DbSet<Order>Orders { get; set; }
        public DbSet<OrderDetail>OrderDetails { get; set; }
        public DbSet<MoneyCase>moneyCases { get; set; }
        public DbSet<MenuTable>MenuTables { get; set; }
        public DbSet<Slider>Sliders { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }


        }
}
