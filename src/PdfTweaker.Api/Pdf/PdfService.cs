using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PdfTweaker.Api.Pdf
{
    public class PdfService : IPdfService
    {
        public MemoryStream ExtractPages(MemoryStream doc, int[] pageNumbers = null)
        {
            var pdf = PdfReader.Open(doc, PdfDocumentOpenMode.Import);

            pageNumbers = pageNumbers ?? Enumerable.Range(1, pdf.PageCount).ToArray();

            var newDoc = new PdfDocument();

            using var ms = new MemoryStream();

            foreach (var pageNumber in pageNumbers)
                newDoc.AddPage(pdf.Pages[pageNumber-1]);

            newDoc.Save(ms);

            return ms;
        }

        public IList<PdfPage> ExtractImages(MemoryStream doc, int[] pageNumbers = null)
        {
            var results = new List<PdfPage>();
            var rast = new GhostscriptRasterizer();

            try
            { 
                var version = GhostscriptVersionInfo.GetLastInstalledVersion(
                    GhostscriptLicense.GPL | GhostscriptLicense.AFPL,
                    GhostscriptLicense.GPL);

                rast.Open(doc, version, true);

                pageNumbers = pageNumbers ?? Enumerable.Range(1, rast.PageCount).ToArray();

                foreach (var pageNumber in pageNumbers)
                {
                    using var ms = new MemoryStream();

                    var img = rast.GetPage(600, 600, pageNumber);
                    img.Save(ms, ImageFormat.Png);

                    results.Add(new PdfPage(pageNumber, ms.ToArray()));
                }

                return results;
            }

            catch(Exception ex) { throw ex; }
                
            finally { rast.Close(); }
        }
    }
}
