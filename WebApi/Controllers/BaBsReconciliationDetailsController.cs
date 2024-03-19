using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BaBsReconciliationDetailsController : ControllerBase
	{
		private readonly IBaBsReconciliationDetailService _baBsReconciliationDetailService;

		public BaBsReconciliationDetailsController(IBaBsReconciliationDetailService baBsReconciliationDetailService)
		{
			_baBsReconciliationDetailService = baBsReconciliationDetailService;
		}

		[HttpPost("addFromExcel")]
		public IActionResult AddFromExcel(IFormFile file, int babsReconciliationDetailService) //dosya seçip ekleme
		{
			if (file.Length > 0)
			{//dosya varsa içeriye gir 
				var fileName = Guid.NewGuid().ToString() + ".xlsx";
				var filePath = $"{Directory.GetCurrentDirectory()}/Content/{fileName}";
				using (FileStream stream = System.IO.File.Create(filePath))//ve kaydet
				{
					file.CopyTo(stream);
					stream.Flush();
				}
				var result = _baBsReconciliationDetailService.AddToExcel(filePath, babsReconciliationDetailService);
				if (result.Success)
				{
					return Ok(result);
				}
				return BadRequest(result.Message);
			}
			return BadRequest("Dosya seçimi yapmadınız");
		}

		[HttpPost("add")]
		public IActionResult Add(BaBsReconciliationDetail babsReconciliationDetailService)
		{
			var result = _baBsReconciliationDetailService.Add(babsReconciliationDetailService);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpPost("update")]
		public IActionResult Update(BaBsReconciliationDetail babsReconciliationDetailService)
		{
			var result = _baBsReconciliationDetailService.Update(babsReconciliationDetailService);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpPost("delete")]
		public IActionResult Delete(BaBsReconciliationDetail babsReconciliationDetailService)
		{
			var result = _baBsReconciliationDetailService.Delete(babsReconciliationDetailService);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpGet("getById")]
		public IActionResult GetById(int id)
		{
			var result = _baBsReconciliationDetailService.GetById(id);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpGet("getList")]
		public IActionResult GetList(int babsReconciliationDetailService)
		{
			var result = _baBsReconciliationDetailService.GetList(babsReconciliationDetailService);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}
	}
}
