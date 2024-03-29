﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
	public interface IMailParameterService
	{
		IResult Update(MailParameter mailParameter);
		IDataResult<MailParameter> Get(int companyId);
	}
}
