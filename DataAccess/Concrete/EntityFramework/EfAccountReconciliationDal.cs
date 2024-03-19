using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Core.DataAccess.EntityFramework;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Concrete;
using Entities.Dtos;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccess.Concrete.EntityFramework
{
	public class EfAccountReconciliationDal : EfEntityRepositoryBase<AccountReconciliation, ContextDb>, IAccountReconciliationDal
	{
		public List<AccountReconciliationDto> GetAllDto(int companyId)
		{
			using (var context = new ContextDb())
			{
				var result = from reconciliation in context.AccountReconciliations.Where(p=>p.CompanyId==companyId)
							 join company in context.Companies on reconciliation.CompanyId equals company.Id
							 join account in context.CurrencyAccounts on reconciliation.CurrencyAccountId equals account.Id
							 join currency in context.Currencies on reconciliation.CurrencyId equals currency.Id 

							 select new AccountReconciliationDto
							 {
								 CompanyId = companyId,
								 CurrencyAccountId = account.Id,
								 AccountIdentityNumber = account.IdentityNumber,
								 AccountName = account.Name,
								 AccountTaxDepartment = account.TaxDepartment,
								 AccountTaxIdNumber = account.TaxIdNumber,
								 CompanyIdentityNumber = company.IdentityNumber,
								 CompanyName = company.Name,
								 CompanyTaxDepartment = company.TaxDepartment,
								 CompanyTaxIdNumber = company.TaxIdNumber,
								 CurrencyCredit = reconciliation.CurrencyCredit,
								 CurrencyDebit = reconciliation.CurrencyDebit,
								 CurrencyId = reconciliation.CurrencyId,
								 EmailReadDate = reconciliation.EmailReadDate,
								 EndingDate = reconciliation.EndingDate,
								 Guid = reconciliation.Guid,
								 Id = reconciliation.Id,
								 IsEmailRead = reconciliation.IsEmailRead,
								 IsResultSucceed = reconciliation.IsResultSucceed,
								 IsSendEmail = reconciliation.IsSendEmail,
								 ResultDate = reconciliation.ResultDate,
								 ResultNote = reconciliation.ResultNote,
								 SenEmailDate = reconciliation.SenEmailDate,
								 StartingDate = reconciliation.StartingDate ,
								 CurrencyCode=currency.Code ,
								 AccountEmail=account.Email
							 };
				return result.ToList();
			}
		}
	}
}
