using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Abstract
{
	public interface IAccountReconciliationService
	{
		IResult Add(AccountReconciliation accountReconciliation);
		IResult AddToExcel(string filePath,int companyId);
		IResult Update(AccountReconciliation accountReconciliation);
		IResult Delete(AccountReconciliation accountReconciliation);
		IDataResult<AccountReconciliation> GetById(int id);
		IDataResult<AccountReconciliation> GetByCode(string code);
		IDataResult<List<AccountReconciliation>>GetList(int companyId);
		IDataResult<List<AccountReconciliationDto>> GetListDto(int companyId);
		IResult SendReconciliationMail(AccountReconciliationDto accountReconciliationDto);

	}  
}
