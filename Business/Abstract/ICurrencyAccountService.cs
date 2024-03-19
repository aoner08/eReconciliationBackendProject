using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
	public interface ICurrencyAccountService
	{
		IResult Add(CurrencyAccount currencyAccount);
		IResult AddToExcel(string filePath,int companyId);
		IResult Update(CurrencyAccount currencyAccount);
		IResult Delete(CurrencyAccount currencyAccount);	
		IDataResult<CurrencyAccount> Get(int id);
		IDataResult<CurrencyAccount> GetByCode(string code,int companyId);
		IDataResult<List<CurrencyAccount>> GetList(int companyId);	
	}
}
