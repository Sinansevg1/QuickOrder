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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=SignalRDb;User Id=sa;Password=SinanSevgi47.;TrustServerCertificate=True");

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
