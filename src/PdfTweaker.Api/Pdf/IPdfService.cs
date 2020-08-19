using System.Collections.Generic;
using System.IO;

namespace PdfTweaker.Api.Pdf
{
    public interface IPdfService
    {
        MemoryStream ExtractPages(MemoryStream doc, int[] pageNumbers = null);

        IList<PdfPage> ExtractImages(MemoryStream doc, int[] pageNumbers = null);
    }
}
