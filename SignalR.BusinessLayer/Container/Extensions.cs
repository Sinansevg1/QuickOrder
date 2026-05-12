using Microsoft.Extensions.DependencyInjection;
using SignalR.BusinessLayer.Abstract;
using SignalR.BusinessLayer.Concrete;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.EntityFreamwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.BusinessLayer.Container
{
    public static class Extensions
    {
        public static void ContainerDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAboutService, AboutManeger>();
            services.AddScoped<IAboutDal, EfAboutDal>();

            services.AddScoped<IBookingService, BookingManeger>();
            services.AddScoped<IBookingDal, EfBookingDal>();

            services.AddScoped<ICategoryService, CategoryManeger>();
            services.AddScoped<ICategoryDal, EfCategoryDal>();

            services.AddScoped<IContactService, ContactManeger>();
            services.AddScoped<IContactDal, EfContactDal>();

            services.AddScoped<IDiscountService, DiscountManeger>();
            services.AddScoped<IDiscountDal, EfDiscountDal>();

            services.AddScoped<IFeatureService, FeatureManeger>();
            services.AddScoped<IFeatureDal, EfFeatureDal>();

            services.AddScoped<IProductService, ProductManeger>();
            services.AddScoped<IProductDal, EfProductDal>();

            services.AddScoped<ISocialMediaService, SocialMediaManeger>();
            services.AddScoped<ISocialMediaDal, EfSocialMediaDal>();

            services.AddScoped<ITestimonialService, TestimonialManeger>();
            services.AddScoped<ITestimonialDal, EfTestimonialDal>();

            services.AddScoped<IOrderService, OrderManeger>();
            services.AddScoped<IOrderDal, EfOrderDal>();

            services.AddScoped<IOrderDetailService, OrderDetailManeger>();
            services.AddScoped<IOrderDetailDal, EfOrderDetailDal>();

            services.AddScoped<IMoneyCaseService, MoneyCaseManeger>();
            services.AddScoped<IMoneyCaseDal, EfMoneyCaseDal>();

            services.AddScoped<IMenuTableService, MenuTableManeger>();
            services.AddScoped<IMenuTableDal, EfMenuTableDal>();

            services.AddScoped<ISliderService, SliderManeger>();
            services.AddScoped<ISliderDal, EfSliderDal>();

            services.AddScoped<IBasketService, BasketManeger>();
            services.AddScoped<IBasketDal, EfBasketDal>();

            services.AddScoped<INotificationService, NotificationManeger>();
            services.AddScoped<INotificationDal, EfNotificationDal>();

            services.AddScoped<IMessageService, MessageMeneger>();
            services.AddScoped<IMessageDal, EfMessageDal>();
        }
    }
}
