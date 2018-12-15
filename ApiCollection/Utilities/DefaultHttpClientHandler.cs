using System.Net;
using System.Net.Http;

namespace ApiCollection.Utilities
{
    /// <summary>
    /// 建立 HTTP Client 的處理器
    /// </summary>
    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }
    }
}
