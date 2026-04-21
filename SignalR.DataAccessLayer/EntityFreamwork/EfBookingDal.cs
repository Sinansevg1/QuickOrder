using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.concrete;
using SignalR.DataAccessLayer.Repositories;
using SignalR.EntityLayer.Entities;

namespace SignalR.DataAccessLayer.EntityFreamwork
{
    public class EfBookingDal : GenericRepository<Booking>, IBookingDal
    {
        public EfBookingDal(SignalRContext context) : base(context)
        {
        }

        public void BookingStatusApproved(int id)
        {
            using var context = new SignalRContext();
            var values = context.Bookings.Find(id);
            if (values is null) return;
            values.Description = "Rezervasyon Onaylandı";
            context.SaveChanges();
        }

        public void BookingStatusCancelled(int id)
        {
            using var context = new SignalRContext();
            var values = context.Bookings.Find(id);
            if (values is null) return;
            values.Description = "Rezervasyon İptal Edildi";
            context.SaveChanges();
        }
    }
}

