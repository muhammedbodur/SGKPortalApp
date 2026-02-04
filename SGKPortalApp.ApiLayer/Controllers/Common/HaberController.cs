using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [ApiController]
    [Route("api/haberler")]
    public class HaberController : ControllerBase
    {
        private readonly IHaberService _haberService;
        private readonly ILogger<HaberController> _logger;

        public HaberController(IHaberService haberService, ILogger<HaberController> logger)
        {
            _haberService = haberService;
            _logger = logger;
        }

        /// <summary>
        /// Dashboard slider için haberleri getirir
        /// </summary>
        [HttpGet("slider")]
        public async Task<IActionResult> GetSliderHaberler([FromQuery] int count = 5)
        {
            var result = await _haberService.GetSliderHaberleriAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Haberler listesi (sayfalama + arama)
        /// GET /api/haberler?page=1&pageSize=12&search=kelime
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetHaberListe(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string? search = null)
        {
            var result = await _haberService.GetHaberListeAsync(page, pageSize, search);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tek haber detayı
        /// GET /api/haberler/{id}
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetHaber(int id)
        {
            var result = await _haberService.GetHaberByIdAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Haber Word dosyası olarak indir
        /// GET /api/haberler/{id}/word
        /// </summary>
        [HttpGet("{id:int}/word")]
        public async Task<IActionResult> DownloadHaberWord(int id)
        {
            var result = await _haberService.GetHaberByIdAsync(id);
            if (!result.Success || result.Data == null)
                return NotFound("Haber bulunamadı");

            var haber = result.Data;

            // Word (docx) oluştur - OpenXML kullanarak
            using var stream = new System.IO.MemoryStream();
            CreateWordDocument(stream, haber);
            stream.Position = 0;

            var fileName = $"Haber_{haber.HaberId}_{SanitizeFileName(haber.Baslik)}.docx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }

        #region Word Generation

        private static void CreateWordDocument(System.IO.Stream stream, SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard.HaberResponseDto haber)
        {
            // OpenXML Word generation - manual ZIP-based approach (no external dependency)
            using var zip = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create, true);

            // [Content_Types].xml
            WriteEntry(zip, "[Content_Types].xml", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types"">
  <Default Extension=""rels"" ContentType=""application/vnd.openxmlformats-package.relationships+xml""/>
  <Default Extension=""xml"" ContentType=""application/xml""/>
  <Override PartName=""/word/document.xml"" ContentType=""application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml""/>
  <Override PartName=""/word/styles.xml"" ContentType=""application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml""/>
</Types>");

            // _rels/.rels
            WriteEntry(zip, "_rels/.rels", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""word/document.xml""/>
</Relationships>");

            // word/_rels/document.xml.rels
            WriteEntry(zip, "word/_rels/document.xml.rels", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"" Target=""styles.xml""/>
</Relationships>");

            // word/styles.xml (minimal)
            WriteEntry(zip, "word/styles.xml", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<w:styles xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"">
  <w:style w:type=""paragraph"" w:default=""1"" w:styleId=""Normal"">
    <w:name w:val=""Normal""/>
    <w:rPr><w:rFonts w:ascii=""Calibri"" w:hAnsi=""Calibri""/><w:sz w:val=""22""/></w:rPr>
  </w:style>
  <w:style w:type=""paragraph"" w:styleId=""Heading1"">
    <w:name w:val=""heading 1""/>
    <w:basedOn w:val=""Normal""/>
    <w:rPr><w:b/><w:sz w:val=""32""/><w:color w:val=""2E74B5""/></w:rPr>
  </w:style>
  <w:style w:type=""paragraph"" w:styleId=""Heading2"">
    <w:name w:val=""heading 2""/>
    <w:basedOn w:val=""Normal""/>
    <w:rPr><w:b/><w:sz w:val=""26""/><w:color w:val=""404040""/></w:rPr>
  </w:style>
</w:styles>");

            // word/document.xml
            var baslik = EscapeXml(haber.Baslik);
            var tarih = haber.YayinTarihi.ToString("dd.MM.yyyy");

            var docXml = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<w:document xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"">
  <w:body>
    <w:p>
      <w:pPr><w:pStyle w:val=""Heading1""/></w:pPr>
      <w:r><w:t>{baslik}</w:t></w:r>
    </w:p>
    <w:p>
      <w:r>
        <w:rPr><w:color w:val=""808080""/><w:sz w:val=""20""/></w:rPr>
        <w:t>Yayın Tarihi: {tarih}</w:t>
      </w:r>
    </w:p>
    <w:p>
      <w:r><w:rPr><w:sz w:val=""2""/></w:rPr><w:t/></w:r>
    </w:p>
    <w:p>
      <w:pPr><w:pStyle w:val=""Heading2""/></w:pPr>
      <w:r><w:t>İçerik</w:t></w:r>
    </w:p>
    {FormatIcerikAsParagraphs(haber.Icerik)}
  </w:body>
</w:document>";

            WriteEntry(zip, "word/document.xml", docXml);
        }

        private static string FormatIcerikAsParagraphs(string icerik)
        {
            var lines = icerik.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            var sb = new System.Text.StringBuilder();
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    sb.AppendLine(@"<w:p><w:r><w:rPr><w:sz w:val=""2""/></w:rPr><w:t/></w:r></w:p>");
                }
                else
                {
                    sb.AppendLine($@"<w:p><w:r><w:t xml:space=""preserve"">{EscapeXml(trimmed)}</w:t></w:r></w:p>");
                }
            }
            return sb.ToString();
        }

        private static void WriteEntry(System.IO.Compression.ZipArchive zip, string entryName, string content)
        {
            var entry = zip.CreateEntry(entryName);
            using var writer = new System.IO.StreamWriter(entry.Open());
            writer.Write(content);
        }

        private static string EscapeXml(string s)
        {
            return s.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;")
                    .Replace("'", "&apos;");
        }

        private static string SanitizeFileName(string name)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                name = name.Replace(c.ToString(), "");
            return name.Length > 50 ? name.Substring(0, 50) : name;
        }

        #endregion
    }
}
