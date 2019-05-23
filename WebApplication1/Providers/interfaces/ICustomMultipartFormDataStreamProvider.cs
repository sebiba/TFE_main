using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Providers
{
    public interface ICustomMultipartFormDataStreamProvider
    {
        string ChunkFilename { get; }
        int ChunkNumber { get; }
        string CorrelationId { get; }
        string Filename { get; }
        int TotalChunks { get; }
        void ExtractValues();
    }
}