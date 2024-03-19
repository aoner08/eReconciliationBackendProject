using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Business.Abstract;
using Business.Constans;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.CrossCuttingConcerns.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using FluentValidation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Business.Concrete
{
	public class AuthManager : IAuthService
	{
		private readonly IUserService _userService;
		private readonly ITokenHelper _tokenHelper;
		private readonly ICompanyService _companyService; //yukardaki _userserbice ile aynı amaç için oluşturuldu 
		private readonly IMailParameterService _mailParameterService;
		private readonly IMailService _mailService;	
		private readonly IMailTemplateService _mailTemplateService;
		private readonly IUserOperationClaimService _userOperationClaimService;
		private readonly IOperationClaimService _OperationClaimService;


		public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService, IMailParameterService mailParameterService, IMailService mailService, IMailTemplateService mailTemplateService, IUserOperationClaimService userOperationClaimService, IOperationClaimService OperationClaimService) 
		{
			_userService = userService;
			_tokenHelper = tokenHelper;
			_companyService = companyService;
			_mailParameterService = mailParameterService;
			_mailService = mailService;
			_mailTemplateService = mailTemplateService;
			_userOperationClaimService = userOperationClaimService;
			_OperationClaimService = OperationClaimService;
		}

		public IResult CompanyExists(Company company)
		{
			var result = _companyService.CompanyExists(company);
			if (result.Success == false)// yani daha önce bu kayıt varsa
			{
				return new ErrorResult(Messages.CompanyAlreadyExists);
			}
			return new SuccessResult();
		}

		public IDataResult<AccessToken> CreateAccessToken(User user,int companyId)
		{
			var claims=_userService.GetClaims(user,companyId);
			var accessToken = _tokenHelper.CreateToken(user, claims, companyId);
			return new SuccesDataResult<AccessToken>(accessToken);	
		}

		public IDataResult<User> GetById(int id)
		{
			return new SuccesDataResult<User>(_userService.GetById(id));
		}

		public IDataResult<User> GetByMailConfirmValue(string value)
		{
			return new SuccesDataResult<User>(_userService.GetByMailConfirmValue(value));
		}

		public IDataResult<User> Login(UserForLogin userForLogin)
		{
			var userToCheck=_userService.GetByMail(userForLogin.Email);	
			if(userToCheck == null)
			{
				return new ErrorDataResult<User>(Messages.UserNotFound);
			}
			if(!HashingHelper.VerifyPasswordHash(userForLogin.Password,userToCheck.PasswordHash,userToCheck.PasswordSalt))
			{
				return new ErrorDataResult<User>(Messages.PasswordError);
			}
			return new SuccesDataResult<User>(userToCheck, Messages.SuccessfulLogin);
		}

		[TransactionScopeAspect] //kayıt işlemlerinden herhangi birinde hata çıkması durumunda o anda yapılan tüm işlemleri iptal eder.user başarılı company kayıt başarısız ise user da iptal olur!
		public IDataResult<UserCompanyDto> Register(UserForRegister userForRegister, string password,Company company)
		{
			

			byte[] passwordHash, passwordSalt;
			HashingHelper.CreatePasswordHash(password,out passwordHash,out passwordSalt);
			var user = new User()
			{
				Email=userForRegister.Email,
				AddedAt=DateTime.Now,
				IsActive=true,
				MailConfirm=false,
				MailConfirmDate=DateTime.Now,
				MailConfirmValue=Guid.NewGuid().ToString(),
				PasswordHash=passwordHash,
				PasswordSalt=passwordSalt,
				Name=userForRegister.Name,	
			};
			//Validation 1
		//	ValidationTool.Validate(new UserValidator(), user); bunların yerine UserManager ve CompanyManager'daki add işleminin başına validation'u kullanacağız. 
			//Validation 2
		//	ValidationTool.Validate(new CompanyValidator(), company);

			_userService.Add(user);
			_companyService.Add(company);
			_companyService.UserCompanyAdd(user.Id, company.Id); //company ile user arasında bağlantı oluşturuyoruz

			UserCompanyDto userCompanyDto = new UserCompanyDto()
			{
				Id=user.Id,
				Name=user.Name,
				Email=user.Email,	
				AddedAt	= user.AddedAt,
				CompanyId=company.Id,
				IsActive = true,
				MailConfirm = user.MailConfirm,
				MailConfirmDate = user.MailConfirmDate,
				MailConfirmValue=user.MailConfirmValue,
				PasswordHash=user.PasswordHash,
				PasswordSalt = user.PasswordSalt,
			};

			/*
						for (int i = 10; i < 49; i++)
						{
							try
							{
								UserOperationClaim userOperationClaim = new UserOperationClaim()
								{
									UserId = user.Id,
									CompanyId = company.Id,
									OperationClaimId = i,
									IsActive = true,
									AddedAt = DateTime.Now
								};
								_userOperationClaimService.Add(userOperationClaim);
							}
							catch (Exception)
							{

							}
						}
			 */

			SendConfirmEmail(user);

			return new SuccesDataResult<UserCompanyDto>(userCompanyDto, Messages.UserRegistered);
		}

		void SendConfirmEmail(User user)
		{
			string subject = "Kullanıcınız kayıt onay maili.";
			string body = "Kullanıcınız sisteme kayıt oldu. Kaydınızı tamamlamak için aşağıdaki linke tıklamanız gerekmektedir.";
			string link = "https://localhost:7220/api/Auth/confirmuser?value= " + user.MailConfirmValue;
			string linkDescription = "Kaydı Onaylamak için Tıklayın";

			var mailTemplate = _mailTemplateService.GetByTemplateName("Kayıt", 4);
			string templateBody = mailTemplate.Data.Value;
			templateBody = templateBody.Replace("{{title}}", subject);
			templateBody = templateBody.Replace("{{message}}", body);
			templateBody = templateBody.Replace("{{link}}", link);
			templateBody = templateBody.Replace("{{linkDescription}}", linkDescription);


			var mailParameter = _mailParameterService.Get(4);
			SendMailDto sendMailDto = new SendMailDto()
			{
				mailParameter = mailParameter.Data,
				email = user.Email,
				subject = "Kullanıcı Kayıt Onay Maili.",
				body = templateBody,
			};
			_mailService.SendMail(sendMailDto); ;

			user.MailConfirmDate = DateTime.Now;
			_userService.Update(user);
		}

		public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password,int companyId)
		{
			byte[] passwordHash, passwordSalt;
			HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
			var user = new User()
			{
				Email = userForRegister.Email,
				AddedAt = DateTime.Now,
				IsActive = true,
				MailConfirm = false,
				MailConfirmDate = DateTime.Now,
				MailConfirmValue = Guid.NewGuid().ToString(),
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				Name = userForRegister.Name,
			};
			_userService.Add(user);

			_companyService.UserCompanyAdd(user.Id, companyId); //company ile user arasında bağlantı oluşturuyoruz
			SendConfirmEmail(user);

			return new SuccesDataResult<User>(user, Messages.UserRegistered);
		}

		public IResult Update(User user)
		{
			_userService.Update(user);
			return new SuccessResult(Messages.UserConfirmSuccessful);
		}

		public IResult UserExists(string email)
		{
			if (_userService.GetByMail(email) != null)
			{
				return new ErrorResult(Messages.UserAlreadyExists);
			}
			return new SuccessResult();
		}

	    IResult IAuthService.SendConfirmEmail(User user)
		{
			if (user.MailConfirm==true) //ayarlama 
			{
				return new ErrorResult(Messages.MailAlreadyConfirm);
			}

			DateTime confirmMailDate = user.MailConfirmDate;
			DateTime now=DateTime.Now;
			if (confirmMailDate.ToShortDateString() == now.ToShortDateString())//tarih aynıysa aşağıda saat ve dakikasını kontrol etsin
			{
				if(confirmMailDate.Hour==now.Hour&&confirmMailDate.AddMinutes(5).Minute<=now.Minute) //üzerinden 5 dk geçmiş mi, geçmişse bana mail gönder!!.
				{
					SendConfirmEmail(user);
					return new SuccessResult(Messages.MailConfirmSendSuccessful);
				}
				else//5 dk geçmemişse
				{
					return new ErrorResult(Messages.MailConfirmTimeHasNotExpired);
				}
			}
			SendConfirmEmail(user); //mail gönderdiğimiz tarih daha eskiyse direk mail göndersin
			return new SuccessResult(Messages.MailConfirmSendSuccessful);
		
		}

		public IDataResult<UserCompany> GetCompany(int userId)
		{
			return new SuccesDataResult<UserCompany>(_companyService.GetCompany(userId).Data);
		}
	}
}
