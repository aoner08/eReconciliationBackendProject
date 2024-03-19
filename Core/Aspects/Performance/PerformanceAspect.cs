using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Core.Entities.Concrete;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Performance
{
	public class PerformanceAspect :MethodInterception
	{
		private int _interval;//ne kadar süre sonra bana haber edecek
		private Stopwatch _stopwatch;//aradaki süeryi tespit edecek

        public PerformanceAspect(int interval)
        {
            _interval = interval;
         _stopwatch=ServiceTool.ServiceProvider.GetService<Stopwatch>();
        
		}
        protected override void OnBefore(IInvocation invocation)
		{
			_stopwatch.Start();
		}
		protected override void OnAfter(IInvocation invocation)
		{
			if(_stopwatch.Elapsed.TotalSeconds>_interval)//işlem süresi verdiğim süreden fazla sürüyorsa
			{
				//mail kodları
				string body=$"Performance:{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}-->{_stopwatch.Elapsed.TotalSeconds}";
				SendConfirmEmail(body);
			}
			_stopwatch.Reset();
		}

		void SendConfirmEmail(string body)
		{
			string subject = "Kullanıcınız kayıt onay maili.";
		
		   
			SendMailDto sendMailDto = new SendMailDto()
			{
				Email = "sqlegitimi9@gmail.com",//mail göndereceğimiz hesaba ait maili giriyoruz
				Password = "eMutabakat0900",
				Port=587,
				SMTP="smtp.gmail.com",
				SSL=true,
				email= "sqlegitimi9@gmail.com",
				subject=subject,
				body=body,
			};

			using (MailMessage mail = new MailMessage())
			{
				mail.From = new MailAddress(sendMailDto.Email);
				mail.To.Add(sendMailDto.email);
				mail.Subject = sendMailDto.subject;
				mail.Body = sendMailDto.body;
				mail.IsBodyHtml = true;
				//mail.Attachments.Add();

				using (SmtpClient smtp = new SmtpClient(sendMailDto.SMTP))
				{
					smtp.UseDefaultCredentials = false;
					smtp.Credentials = new NetworkCredential(sendMailDto.Email, sendMailDto.Password);
					smtp.EnableSsl = sendMailDto.SSL;
					smtp.Send(mail);
				}

			}

			
		}


	}
}
