using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PdfTweaker.Api.Pdf;
using PdfTweaker.Api.Split;
using Xunit;

namespace PdfTweaker.Api.Tests.Split
{
    public class SplitControllerTests
    {        
        [Fact]
        public async Task PreviewAsyncReturnsBadRequestWhenNoFileIsUploaded()
        {
            var request = SplitPreviewRequest();
            request.Pdf = null;

            var sut = CreateTestSubject();

            var result = await sut.PreviewAsync(request, default);

            Assert.NotNull(result as BadRequestObjectResult);            
        }

        [Fact]
        public async Task PreviewAsyncReturnsOkWhenFileIsUploaded()
        {
            var pagesInDocument = 2;

            var mockPdfService = CreateMockPdfService(pagesInDocument);
            var sut = CreateTestSubject(mockPdfService);

            var request = SplitPreviewRequest();

            var result = await sut.PreviewAsync(request, default(CancellationToken));

            var payload = result as OkObjectResult;
            Assert.Equal(pagesInDocument, (payload.Value as List<object>).Count); 
        }

        [Fact]
        public async Task SplitAsyncReturnsBadRequestWhenNoFileIsUploaded()
        {
            var sut = CreateTestSubject();

            var request = SplitRequest(1);
            request.Pdf = null;

            var result = await sut.SplitAsync(request, default);

            Assert.NotNull(result as BadRequestObjectResult);
        }

        [Fact]
        public async Task SplitAsyncReturnsOkWhenRequestIsValid()
        {
            var pagesInDocument = 2;

            var mockPdfService = CreateMockPdfService(2);
            var sut = CreateTestSubject(mockPdfService);

            var request = SplitRequest(pagesInDocument);

            var result = await sut.SplitAsync(request, default);

            var payload = result as OkObjectResult;
            Assert.Equal(pagesInDocument, (payload.Value as List<object>).Count);
        }

        private SplitController CreateTestSubject(Mock<IPdfService> mockPdfService = null)
        {
            var mockLogger = new Mock<ILogger<SplitController>>();

            mockPdfService = mockPdfService ?? CreateMockPdfService(0);

            return new SplitController(mockPdfService.Object, mockLogger.Object);
        }

        private Mock<IPdfService> CreateMockPdfService(int numberOfPages)
        {
            var mockPdfService = new Mock<IPdfService>();
            var result = Enumerable.Range(1, numberOfPages).Select(i => new PdfPage(i, new byte[i])).ToList();

            mockPdfService.Setup(s => s.ExtractImages(It.IsAny<MemoryStream>(), It.IsAny<int[]>())).Returns(result);
            mockPdfService.Setup(s => s.ExtractPages(It.IsAny<MemoryStream>(), It.IsAny<int[]>())).Returns(new MemoryStream());

            return mockPdfService;
        }

        private SplitPreviewRequest SplitPreviewRequest()
        {
            return new SplitPreviewRequest()
            {
                Pdf = new Mock<IFormFile>().Object
            };
        }

        private SplitRequest SplitRequest(int numberOfPages)
        {
            var instructions = Enumerable.Range(1, numberOfPages).Select(i => new SplitInstructions { Type = $"type{i}", PageNumber = i });

            return new SplitRequest()
            {
                Pdf = new Mock<IFormFile>().Object,
                Instructions = JsonConvert.SerializeObject(instructions)
            };
        }
    }
}