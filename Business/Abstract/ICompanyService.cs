using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Abstract
{
	public interface ICompanyService
	{
		//CRUD işlemlerinden ihtiyaç duyduklarımızı çağıracağız
		IResult Add(Company company);
		IResult Update(Company company);
		IDataResult<Company> GetById(int id);
		IResult AddCompanyAndUserCompany(CompanyDto companyDto);
		IDataResult<List<Company>> GetList();
		IDataResult<UserCompany> GetCompany(int userId);

		IResult CompanyExists(Company company); //User ile company arasında ilişkiyi sağlamak için eklendi
		IResult UserCompanyAdd(int userId, int companyId);
	}
}
