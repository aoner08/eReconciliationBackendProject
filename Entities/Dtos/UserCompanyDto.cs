using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Concrete;

namespace Entities.Dtos
{
	public class UserCompanyDto:User ,IDto
	{
        public int CompanyId { get; set; }
    }
}
