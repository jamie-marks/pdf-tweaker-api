namespace PdfTweaker.Api.Pdf
{
    public class PdfPage
    {
        public PdfPage(int pageNumber, byte[] content)
        {
            PageNumber = pageNumber;
            Content = content;
        }

        public int PageNumber { get; }

        public byte[] Content { get; }

        public int Index => PageNumber - 1;
    }
}
