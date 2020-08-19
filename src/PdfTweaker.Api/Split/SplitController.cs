using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PdfTweaker.Api.Pdf;

namespace PdfTweaker.Api.Split
{
    [ApiController]
    [Route("api/split")]
    public class SplitController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        private readonly ILogger<SplitController> _logger;

        public SplitController(IPdfService pdfService, ILogger<SplitController> logger)
        {
            _pdfService = pdfService;
            _logger = logger;
        }

        [HttpPost]
        [Route("preview")]
        public async Task<IActionResult> PreviewAsync([FromForm] SplitPreviewRequest request, CancellationToken token)
        {
            if (request.Pdf == null) return BadRequest("No file uploaded");

            using var ms = new MemoryStream();
            await request.Pdf.CopyToAsync(ms, token);            
            
            var images = _pdfService.ExtractImages(ms);

            var results = new List<object>();
            foreach (var image in images) 
                results.Add( new { pageNumber = image.PageNumber, thumbnail = Convert.ToBase64String(image.Content) });            

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> SplitAsync([FromForm] SplitRequest request, CancellationToken token)
        {
            if (request.Pdf == null) return BadRequest("No file uploaded");

            var pages = JsonConvert.DeserializeObject<SplitInstructions[]>(request.Instructions);

            using var doc = new MemoryStream();

            await request.Pdf.CopyToAsync(doc, token);

            var types = pages.Select(p => p.Type).Distinct();

            var results = new List<object>();
            foreach (var type in types)
            {
                var pageNumbers = pages.Where(p => p.Type == type)
                    .Select(p => p.PageNumber)
                    .ToArray();

                var pagesToAdd = _pdfService.ExtractPages(doc, pageNumbers);

                var base64 = Convert.ToBase64String(pagesToAdd.ToArray());
                results.Add(new { type, base64 });                
            }

            return Ok(results);
        }     
    }
}
