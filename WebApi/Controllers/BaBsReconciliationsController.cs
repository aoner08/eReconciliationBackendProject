using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BaBsReconciliationsController : ControllerBase
	{
		private readonly IBaBsReconciliationService _babsReconciliationService;

		public BaBsReconciliationsController(IBaBsReconciliationService babsReconciliationService)
		{
			_babsReconciliationService = babsReconciliationService;
		}

		[HttpPost("addFromExcel")]
		public IActionResult AddFromExcel(IFormFile file, int companyId) //dosya seçip ekleme
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
				var result = _babsReconciliationService.AddToExcel(filePath, companyId);
				if (result.Success)
				{
					return Ok(result);
				}
				return BadRequest(result.Message);
			}
			return BadRequest("Dosya seçimi yapmadınız");
		}

		[HttpPost("add")]
		public IActionResult Add(BaBsReconciliation babsReconciliationService)
		{
			var result = _babsReconciliationService.Add(babsReconciliationService);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpPost("update")]
		public IActionResult Update(BaBsReconciliation babsReconciliationService)
		{
			var result = _babsReconciliationService.Update(babsReconciliationService);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpPost("delete")]
		public IActionResult Delete(BaBsReconciliation babsReconciliationService)
		{
			var result = _babsReconciliationService.Delete(babsReconciliationService);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpGet("getById")]
		public IActionResult GetById(int id)
		{
			var result = _babsReconciliationService.GetById(id);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpGet("getList")]
		public IActionResult GetList(int companyId)
		{
			var result = _babsReconciliationService.GetListDto(companyId);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}
		[HttpPost("sendReconciliationMail")]
		public IActionResult SendReconciliationMail(BaBsReconciliationDto babsReconciliationDto)
		{
			var result = _babsReconciliationService.SendReconciliationMail(babsReconciliationDto);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}

		[HttpGet("getByCode")] //"Mutabakat mail cevaplamak için tıklayın" kısmının çalışması için tasarlandı
		public IActionResult GetByCode(string code)
		{
			var result = _babsReconciliationService.GetByCode(code);
			if (result.Success)
			{
				return Ok(result);
			}
			return BadRequest(result.Message);
		}
	}
}
