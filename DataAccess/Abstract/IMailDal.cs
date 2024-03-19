using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Dtos;

namespace DataAccess.Abstract
{
	public interface IMailDal
	{
		void SendMail(SendMailDto sendMailDto);
	}
}
