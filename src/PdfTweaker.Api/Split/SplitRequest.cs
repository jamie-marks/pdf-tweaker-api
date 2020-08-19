using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace PdfTweaker.Api.Split
{
    [DataContract]
    public class SplitRequest
    {
        [DataMember]
        public IFormFile Pdf { get; set; }

        [DataMember]
        public string Instructions { get; set; }
    }

    [DataContract]
    public class SplitInstructions
    {
        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public string Type { get; set; }        
    }
}
