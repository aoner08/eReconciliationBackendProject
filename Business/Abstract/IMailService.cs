using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using Entities.Dtos;

namespace Business.Abstract
{
	public interface IMailService
	{
		IResult SendMail(SendMailDto sendMailDto);
	}
}
