using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Entities.Concrete;
using DataAccess.Abstract;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Business.Constans;
using Core.Entities.Concrete;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Entities.Dtos;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Caching;
using Business.BusinessAspects;
using Core.Aspects.Performance;
namespace Business.Concrete

{
	public class CompanyManager : ICompanyService
	{
		//class'a Kullanıcı yetkili mi değil mi,Transcaption,Log,Validation ve benzer kontrolleri yapılır.
		//DataAccess'de genel yapıyı oluştururken business'ta işlemin kalan kısımlarını oluşturuyoruz ,ardından WebApi'de sadece business'taki metodu( GetList) çağırıyoruz.

		//Dependency Injection
		private readonly ICompanyDal _companyDal;
        public CompanyManager(ICompanyDal companyDal)
        {
            _companyDal = companyDal;
        }

		//ilk şirket kaydını yaparken yetki soramayacağı için securedoperation sorgusu yapmadık
		[CacheRemoveAspect("ICompanyService.Get")]//veri varsa silip ekle.aspect'in amaçı işlem başlamadan önce onu lkontrol etmekir
		[ValidationAspect(typeof(CompanyValidator))]//bu işlem ile add işleminde(username not null,company name not null vs) belli kuralları tanımladık
		public IResult Add(Company company)
		{
			_companyDal.Add(company);
			return new SuccessResult(Messages.AddedCompany); //mesajı constan'tan çektik

			/*
						Bu yapıyı validation ile daha kullanışlı bir hale getirebiliriz
						if (company.Name.Length > 10)
						{
							_companyDal.Add(company);
							return new SuccessResult(Messages.AddedCompany); //mesajı constan'tan çektik
						}
						return new ErrorResult("Şirket adı en az 10 karakter olmalıdır");
			 */

		}

		
		[CacheRemoveAspect("ICompanyService.Get")]
		[ValidationAspect(typeof(CompanyValidator))]
		[TransactionScopeAspect]//işlemlerden biri hatalıysa olumlu olanı da iptal et ve hata mesajı gönder
		public IResult AddCompanyAndUserCompany(CompanyDto companyDto)
		{
			_companyDal.Add(companyDto.Company);
			_companyDal.UserCompanyAdd(companyDto.UserId,companyDto.Company.Id);
			return new SuccessResult(Messages.AddedCompany);
		}

		public IResult CompanyExists(Company company)
		{
			//name,taxdepartment,taxıdnumber vs kontrol ettirdik bunlar var mı diye
			var result=_companyDal.Get(c=>c.Name==company.Name &&  c.TaxDepartment==company.TaxDepartment && c.TaxIdNumber==company.TaxIdNumber && c.IdentityNumber==company.IdentityNumber);
			if(result!=null)// yani daha önce bu kayıt varsa
			{
				return new ErrorResult(Messages.CompanyAlreadyExists);
			}
			return new SuccessResult();
		}

		[CacheAspect(60)]
		public IDataResult<Company> GetById(int id)
		{
			return new SuccesDataResult<Company>(_companyDal.Get(p=>p.Id==id));	//company id'si getirildi
		}

		[CacheAspect(60)]
		public IDataResult<UserCompany> GetCompany(int userId)
		{
		   return new SuccesDataResult<UserCompany>(_companyDal.GetCompany(userId));
		}

		[CacheAspect(60)]
		public IDataResult<List<Company>> GetList()
		{
			return new SuccesDataResult<List<Company>>(_companyDal.GetList()); //(_companyDal.GetList(),"Sorgulama işlemi başarılı"); şeklinde de mesaj verilebilirdi
		}

		[PerformanceAspect(3)]
		[SecuredOperation("Company.Update,Admin")]
		[CacheRemoveAspect("ICompanyService.Get")]
		public IResult Update(Company company)
		{
			_companyDal.Update(company);
			return new SuccessResult(Messages.UpdatedCompany);
		}

		[CacheRemoveAspect("ICompanyService.Get")]
		public IResult UserCompanyAdd(int userId, int companyId)
		{
			_companyDal.UserCompanyAdd(userId, companyId);
			return new SuccessResult();
		}
	}
}
