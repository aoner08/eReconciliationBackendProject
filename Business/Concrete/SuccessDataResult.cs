using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;

namespace Business.Concrete
{
	public class SuccessDataResult<T> : IDataResult<User>
	{
		private UserOperationClaim userOperationClaim;

		public SuccessDataResult(UserOperationClaim userOperationClaim)
		{
			this.userOperationClaim = userOperationClaim;
		}

		public User Data { get; set; }

		public bool Success { get; set; }

		public string Message { get; set; }
	}
}