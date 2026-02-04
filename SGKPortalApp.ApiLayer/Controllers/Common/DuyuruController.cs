using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class DuyuruController : ControllerBase
    {
        private readonly IDuyuruService _duyuruService;
        private readonly ILogger<DuyuruController> _logger;

        public DuyuruController(
            IDuyuruService duyuruService,
            ILogger<DuyuruController> logger)
        {
            _duyuruService = duyuruService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _duyuruService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _duyuruService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DuyuruCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _duyuruService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.DuyuruId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DuyuruUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _duyuruService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _duyuruService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("slider/{count:int?}")]
        public async Task<IActionResult> GetSliderDuyurular(int count = 5)
        {
            var result = await _duyuruService.GetSliderDuyurularAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("liste/{count:int?}")]
        public async Task<IActionResult> GetListeDuyurular(int count = 10)
        {
            var result = await _duyuruService.GetListeDuyurularAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("aktif")]
        public async Task<IActionResult> GetAktifDuyurular()
        {
            var result = await _duyuruService.GetAktifDuyurularAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { Success = false, Message = "Dosya seçilmedi" });

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new { Success = false, Message = "Geçersiz dosya türü. Sadece JPG, PNG, GIF ve WebP dosyaları yüklenebilir." });

                if (file.Length > 5 * 1024 * 1024) // 5MB
                    return BadRequest(new { Success = false, Message = "Dosya boyutu 5MB'dan büyük olamaz" });

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "duyurular");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/duyurular/{uniqueFileName}";
                _logger.LogInformation("Duyuru görseli yüklendi: {ImageUrl}", imageUrl);

                return Ok(new { Success = true, Data = imageUrl, Message = "Görsel başarıyla yüklendi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Görsel yükleme hatası");
                return StatusCode(500, new { Success = false, Message = "Görsel yüklenirken bir hata oluştu" });
            }
        }
    }
}
