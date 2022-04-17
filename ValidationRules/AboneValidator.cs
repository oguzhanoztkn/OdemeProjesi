using FluentValidation;
using OdemeProjesi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OdemeProjesi.ValidationRules
{
    public class AboneValidator: AbstractValidator<tblAbone>
    {
        public AboneValidator()
        {
            RuleFor(x => x.AboneAdi).NotEmpty().WithMessage("Abone Adını Boş Geçemezsiniz");
            RuleFor(x => x.AboneSoyadi).NotEmpty().WithMessage("Abone Soyadını Boş Geçemezsiniz");
            RuleFor(x => x.TCNo).NotEmpty().WithMessage("Tc No Boş Geçemezsiniz");
            RuleFor(x => x.TCNo).Length(11).WithMessage("Tc No 11 Karakter olmalıdır");
            //RuleFor(x => x.CategoryName).MinimumLength(3).WithMessage("Lütfen en az 3 karakter giriniz");
            //RuleFor(x => x.CategoryName).MaximumLength(20).WithMessage("Lütfen 20 karakteri aşmayın");

        }
    }
}