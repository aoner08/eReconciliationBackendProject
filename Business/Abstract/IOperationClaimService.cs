using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
	public interface IOperationClaimService
	{
		IResult Add(OperationClaim operationClaim);
		IResult Update(OperationClaim operationClaim);
		IResult Delete(OperationClaim operationClaim);
		IDataResult<OperationClaim> GetById(int id);
		IDataResult<List<OperationClaim>> GetList();
	}
}
