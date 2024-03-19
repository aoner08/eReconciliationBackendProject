﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentValidation;

namespace Business.Concrete
{
	public class UserManager : IUserService
	{
		private IUserDal _userDal;

		public UserManager(IUserDal userDal)
		{
			_userDal = userDal;
		}

		[CacheRemoveAspect("IUserService.Get")]
		[ValidationAspect(typeof(UserValidator))] //bu işlem ile add işleminde(username not null,company name not null vs) belli kuralları tanımladık
		public void Add(User user)
		{
			_userDal.Add(user);
		}

		[CacheAspect(60)]
		public User GetById(int id)
		{
			return _userDal.Get(u => u.Id == id);
		}

		[CacheAspect(60)]
		public User GetByMail(string email)
		{
			return _userDal.Get(p=>p.Email == email);
		}

		[CacheAspect(60)]
		public User GetByMailConfirmValue(string value)
		{
			return _userDal.Get(p=>p.MailConfirmValue == value);	
		}

		public List<OperationClaim> GetClaims(User user, int companyId)
		{
			return _userDal.GetClaims(user, companyId);
		}

		[PerformanceAspect(3)]
		[SecuredOperation("User.Update,Admin,Admin")] //Admin yetkisine sahip olan hepsini yapabilsin
		[CacheRemoveAspect("IUserService.Get")]
		public void Update(User user)
		{
			_userDal.Update(user);
		}
	}
}