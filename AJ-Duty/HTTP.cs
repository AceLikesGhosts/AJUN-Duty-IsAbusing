using System.Collections.Specialized;
using System.Net;

namespace AJUN
{
    class HTTP
    {
        public static byte[] Post(string uri, NameValueCollection pairs)
        {
            using (WebClient webClient = new WebClient())
                return webClient.UploadValues(uri, pairs);
        }
    }
}
