using System;

using Aliyun.Acs.Core.Endpoints;
namespace aa_develop
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            EndpointsDataProvider provider = new EndpointsDataProvider();
            // provider.GetEndpoint("Ecs", "cn-hangzhou", "ecs");
            provider.readEndpointsDataFile();
        }
    }
}
