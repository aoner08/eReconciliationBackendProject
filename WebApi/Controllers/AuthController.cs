using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegister)
		{
			var userExists = _authService.UserExists(userAndCompanyRegister.UserForRegister.Email);//user'ı kontrol ediyoruz var mı yok mu diye?
			if (!userExists.Success)
			{
				return BadRequest(userExists.Message);
			}
			var companyExists = _authService.CompanyExists(userAndCompanyRegister.Company);
			if (!companyExists.Success)
			{
				return BadRequest(companyExists.Message);
			}

			var registerResult = _authService.Register(userAndCompanyRegister.UserForRegister, userAndCompanyRegister.UserForRegister.Password, userAndCompanyRegister.Company);

			var result = _authService.CreateAccessToken(registerResult.Data, registerResult.Data.CompanyId);

			if (result.Success)
			{
				return Ok(result.Data); //Ok=>200 kodudur
			}

			return BadRequest(registerResult.Message);
		}

		[HttpPost("registerSecondAccount")]
		public IActionResult RegisterSecondAccount(UserForRegisterToSecondAccountDto userForRegister)
		{
			var userExists = _authService.UserExists(userForRegister.Email);
			if (!userExists.Success)
			{
				return BadRequest(userExists.Message);
			}
			var registerResult = _authService.RegisterSecondAccount(userForRegister, userForRegister.Password, userForRegister.CompanyId);

			var result = _authService.CreateAccessToken(registerResult.Data, userForRegister.CompanyId);
			if (result.Success)
			{
				return Ok(result.Data); //Ok=>200 kodudur
			}

		
			return BadRequest(registerResult.Message);
		}

		[HttpPost("login")] //api/Auth(control_ismi)/login şeklinde api'yi çağırıyoruz
		public IActionResult Login(UserForLogin userForLogin)
		{

			var userToLogin = _authService.Login(userForLogin);
			if (!userToLogin.Success)
			{
				return BadRequest(userToLogin.Message);
			}
			if (userToLogin.Data.IsActive)
			{
				var userCompany = _authService.GetCompany(userToLogin.Data.Id).Data;
				var result = _authService.CreateAccessToken(userToLogin.Data, userCompany.CompanyId); //işlem başarılı ise token ver
				if (result.Success)
				{
					return Ok(result.Data);
				}
				return BadRequest(result.Message);
			}
			return BadRequest("Kullanıcı pasif durumda.Aktif etmek için yöneticinize danışın");
		}


		[HttpGet("confirmuser")]
		public IActionResult ConfirmUser(string value)
		{ 
			//Artık kayıt yapıldığında mail butonu gönderecek ve butona tıklandığında bize kayıt olduğuna dair mesaj atacak .
			var user=_authService.GetByMailConfirmValue(value).Data;
			user.MailConfirm = true;
			user.MailConfirmDate = DateTime.Now;
			var result = _authService.Update(user);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpGet("sendConfirmEmail")] //5
		public IActionResult SecondConfirmEmail(int id)
		{
			var user = _authService.GetById(id).Data;
			var result=_authService.SendConfirmEmail(user);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);

		}
	}
}
