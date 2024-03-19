using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using FluentValidation;

namespace Core.Aspects.Autofac.Validation
{
	public class ValidationAspect:MethodInterception
	{
		private Type _validatorType;
		public ValidationAspect(Type validatorType)
		{
			if(!typeof(IValidator).IsAssignableFrom(validatorType)) //gönderilen validation tipinde değilse
			{
				throw new System.Exception("Hatalı tip");
			}
			_validatorType = validatorType;
		}
		protected override void OnBefore(IInvocation invocation)
		{
			var validator = (IValidator)Activator.CreateInstance(_validatorType);
			var entityType = _validatorType.GetGenericArguments()[0];   
			var entities=invocation.Arguments.Where(t=>t.GetType() == entityType);	
			foreach(var entity in entities)
			{
				ValidationTool.Validate(validator, entity);
			}
		}
	}
}
