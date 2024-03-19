using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Entities.Concrete;

namespace Entities.Dtos
{
	public class UserAndCompanyRegisterDto:IDto
	{
	public UserForRegister UserForRegister { get; set; }
	public Company Company { get; set; }
	}
}
