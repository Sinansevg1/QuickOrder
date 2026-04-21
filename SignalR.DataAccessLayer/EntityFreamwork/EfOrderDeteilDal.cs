using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.concrete;
using SignalR.DataAccessLayer.Repositories;
using OrderDeteil = SignalR.EntityLayer.Entities.OrderDetail;

namespace SignalR.DataAccessLayer.EntityFreamwork
{
    public class EfOrderDeteilDal : GenericRepository<OrderDeteil>, IOrderDeteilDal
    {
        public EfOrderDeteilDal(SignalRContext context) : base(context)
        {
        }
    }
}

