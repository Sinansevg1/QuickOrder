using FluentValidation;
using SignalR.DtoLayer.BookingDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.BusinessLayer.ValidationRules.BookingValidations
{
    public class CreateBookingValidation:AbstractValidator<CreateBookingDto>
    {
        public CreateBookingValidation()
        {
            RuleFor(x=> x.Name).NotEmpty().WithMessage("İsim Alanı Boş Geçilemez!");
            RuleFor(x=> x.Phone).NotEmpty().WithMessage("Telefon Alanı Boş Geçilemez!");
            RuleFor(x=> x.Mail).NotEmpty().WithMessage("Mail Alanı Boş Geçilemez!");
            RuleFor(x=> x.PersonCount).NotEmpty().WithMessage("Kişi Sayısı Alanı Boş Geçilemez!");
            RuleFor(x=> x.Date).NotEmpty().WithMessage("Tarih Alanı Boş Geçilemez Lütfen Tarih Seçiniz!");

            RuleFor(x=> x.Name).MinimumLength(5).WithMessage("İsim Alanı En Az 5 Karakter Olmalıdır!").MaximumLength(50).WithMessage("İsim Alanı En Fazla 50 Karakter Olmalıdır!");
            RuleFor(x=> x.Description).MaximumLength(50).WithMessage("Açıklama Alanı En Fazla 50 Karakter Olmalıdır!");

            RuleFor(x=> x.Mail).EmailAddress().WithMessage("Lütfen Geçerli Bir Mail Adresi Giriniz!");
        }
    }
}
