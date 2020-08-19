using Microsoft.AspNetCore.Http;

namespace PdfTweaker.Api.Split
{
    public class SplitPreviewRequest
    {
        public IFormFile Pdf { get; set; }        
    }
}
