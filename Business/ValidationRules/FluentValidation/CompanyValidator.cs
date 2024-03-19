using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
	public class CompanyValidator:AbstractValidator<Company>
	{
        public CompanyValidator()
        {
            RuleFor(p=>p.Name).NotEmpty().WithMessage("Şirket adı boş olamaz");
            RuleFor(p => p.Name).MinimumLength(4).WithMessage("Şirket adı en az 4 karakter olmalı");
            RuleFor(p => p.Address).NotEmpty().WithMessage("Şirket adı boş olamaz");

        }
    }
}
