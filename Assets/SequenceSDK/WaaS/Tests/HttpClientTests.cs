using NUnit.Framework;
using Sequence.Config;

namespace Sequence.WaaS.Tests
{
    public class HttpClientTests
    {
        [Test]
        public void TestHttpClientIncludesAPIKey()
        {
            HttpClient client = new HttpClient("https://randomurl.com");
            var request = client.BuildRequest<object>("", null);
            string header = request.Item1.GetRequestHeader("X-Access-Token");
            Assert.IsTrue(header.Length > 0);
            Assert.AreEqual(SequenceConfig.GetConfig().BuilderAPIKey, header);
            Assert.IsTrue(request.Item2.Contains(header));
        }
    }
}