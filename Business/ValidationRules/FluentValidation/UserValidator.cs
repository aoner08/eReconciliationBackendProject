﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
	public class UserValidator:AbstractValidator<User>//A.Validator.. business katmanına eklediğimiz FluentValidator'un uzantısı 
	{
		public UserValidator()
		{
			RuleFor(p=>p.Name).NotEmpty().WithMessage("Kullanıcı adı boş olamaz");
			RuleFor(p => p.Name).MinimumLength(4).WithMessage("Kullanıcı adı en az 4 karakter olmalıdır");
			RuleFor(p => p.Email).NotEmpty().WithMessage("Mail boş olamaz");
			RuleFor(p => p.Email).EmailAddress().WithMessage("Geçerli bir mail adresi yazın");

		}
	}
}
