using System;

using Aliyun.Acs.Core.Endpoints;

using Xunit;

namespace Aliyun.Acs.Core.Tests.Units.Endpoints
{
    public class EndpointsDataProviderTest
    {
        [Fact]
        public void GetEndpoint()
        {
            EndpointsDataProvider endpointsProvider = new EndpointsDataProvider();
            Endpoint endpoint = endpointsProvider.GetEndpoint("Vod", "cn-hangzhou", "");
            Console.WriteLine(endpoint.GetDomain());
        }
    }
}
