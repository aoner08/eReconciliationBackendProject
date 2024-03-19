using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects;
using Business.Constans;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using ExcelDataReader;

namespace Business.Concrete
{
	public class BaBsReconciliationDetailManager : IBaBsReconciliationDetailService
	{
		private readonly IBaBsReconciliationDetailDal _baBsReconciliationDetailDal;

		public BaBsReconciliationDetailManager(IBaBsReconciliationDetailDal baBsReconciliationDetailDal)
		{
			_baBsReconciliationDetailDal = baBsReconciliationDetailDal;
		}

		[PerformanceAspect(3)]
		[SecuredOperation("BaBsReconciliationDetail.Add,Admin")]
		[CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
		public IResult Add(BaBsReconciliationDetail baBsReconciliationDetailDal)
		{
			_baBsReconciliationDetailDal.Add(baBsReconciliationDetailDal);
			return new SuccessResult(Messages.AddedBaBsReconciliationDetail);
		}

		[PerformanceAspect(3)]
		[SecuredOperation("BaBsReconciliationDetail.Add,Admin")]
		[CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
		[TransactionScopeAspect]
		public IResult AddToExcel(string filePath, int babsReconciliationId)
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
			{
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					while (reader.Read())
					{
						string description = reader.GetString(1);

						if (description != "Açıklama" && description != null)
						{
							DateTime date = reader.GetDateTime(0);
							double amount= reader.GetDouble(2);

							BaBsReconciliationDetail baBsReconciliationDetail=new BaBsReconciliationDetail()
							{
								BaBsReconciliationId = babsReconciliationId,
								Date=date,
								Description=description,	
								Amount=Convert.ToDecimal(amount),

							};
							_baBsReconciliationDetailDal.Add(baBsReconciliationDetail);
						}
					}
				}
			}

			File.Delete(filePath);//excel dosyası yüklendikten sonra content içine atıyordu, ordan silme komutu!!

			return new SuccessResult(Messages.AddedAccountReconciliation);
		}

		[PerformanceAspect(3)]
		[SecuredOperation("BaBsReconciliationDetail.Delete,Admin")]
		[CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
		public IResult Delete(BaBsReconciliationDetail baBsReconciliationDetailDal)
		{
			_baBsReconciliationDetailDal.Delete(baBsReconciliationDetailDal);
			return new SuccessResult(Messages.DeletedBaBsReconciliationDetail);
		}

		[PerformanceAspect(3)]
		[SecuredOperation("BaBsReconciliationDetail.Get,Admin")]
		[CacheAspect(60)]
		public IDataResult<BaBsReconciliationDetail> GetById(int id)
		{
			return new SuccesDataResult<BaBsReconciliationDetail>(_baBsReconciliationDetailDal.Get(p => p.Id == id));
		}

		[PerformanceAspect(3)]
		[SecuredOperation("BaBsReconciliationDetail.GetList,Admin")]
		[CacheAspect(60)]
		public IDataResult<List<BaBsReconciliationDetail>> GetList(int babsReconciliationId)
		{
			return new SuccesDataResult<List<BaBsReconciliationDetail>>(_baBsReconciliationDetailDal.GetList(p => p.BaBsReconciliationId == babsReconciliationId));
		}

		[PerformanceAspect(3)]
		[SecuredOperation("BaBsReconciliationDetail.Update,Admin")]
		[CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
		public IResult Update(BaBsReconciliationDetail baBsReconciliationDetailDal)
		{
			_baBsReconciliationDetailDal.Update(baBsReconciliationDetailDal);
			return new SuccessResult(Messages.UpdatedBaBsReconciliationDetail);
		}
	}
}
