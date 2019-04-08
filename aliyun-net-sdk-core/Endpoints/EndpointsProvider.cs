using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Aliyun.Acs.Core.Auth;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core.Regions;
using Aliyun.Acs.Core.Regions.Location.Model;
using Aliyun.Acs.Core.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aliyun.Acs.Core.Endpoints
{
    public class EndpointsDataProvider
    {
        private const string BUNDLED_ENDPOINTS_RESOURCE_PATH = "endpoints.json";

        private static Dictionary<string, Endpoint> endpoints { get; set; }

        private static Dictionary<string, Dictionary<string, string>> endpointsDictionary { get; set; }

        private RemoteEndpointsParser remoteProvider = null;

        public Endpoint GetEndpoint(string product, string regionId, string serviceCode)
        {
            if (null == endpoints)
            {
                Type type = MethodBase.GetCurrentMethod().DeclaringType;
                string _namespace = type.Namespace;
                Assembly _assembly = Assembly.GetExecutingAssembly();
                string resourceName = _namespace + "." + BUNDLED_ENDPOINTS_RESOURCE_PATH;
                Console.WriteLine(resourceName);
                // _assembly.Get
                Stream stream = _assembly.GetManifestResourceStream(resourceName);
                Console.WriteLine(stream == null);
                // StreamReader r = new StreamReader(stream);
                // StreamReader r = new StreamReader("../../../endpoints.json");

                // Console.WriteLine(r.ReadToEnd());
                // this.InitEndpointsData();
            }
            string index = product + "_" + regionId;
            Endpoint endpoint = null;
            endpoints.TryGetValue(index, out endpoint);

            // request location:DescribeRegions
            if (endpoint == null && serviceCode != null)
            {
                endpoint = this.GetEndpointFromLocation(product, regionId, serviceCode);
            }

            if (endpoint == null)
            {
                throw new ClientException("SDK.UnsupportedRegionId", this.GetRegionIdList(product), "");
            }
            return endpoint;
        }

        private string GetRegionIdList(string product)
        {
            Dictionary<string, string> regionDictionary = DictionaryUtil.Get(endpointsDictionary, product);
            Dictionary<string, string>.KeyCollection keys = regionDictionary.Keys;
            return keys.ToString();
        }

        private Endpoint GetEndpointFromLocation(string product, string regionId, string serviceCode)
        {
            Credential credential = DefaultProfile.GetProfile().GetCredential();
            remoteProvider = RemoteEndpointsParser.InitRemoteEndpointsParser();
            Aliyun.Acs.Core.Regions.Endpoint locationEndpoint = remoteProvider.GetEndpoint(
                regionId, product, serviceCode, "openAPI", credential, null);

            string domain = "";

            foreach (ProductDomain productDomain in locationEndpoint.ProductDomains)
            {
                domain = productDomain.DomianName;
                break;
            }
            Endpoint endpoint = this.AddEndpoint(product, regionId, domain);
            return endpoint;
        }

        private void InitEndpointsData()
        {
            Type type = MethodBase.GetCurrentMethod().DeclaringType;
            string _namespace = type.Namespace;
            Assembly _assembly = Assembly.GetExecutingAssembly();
            string resourceName = _namespace + "." + BUNDLED_ENDPOINTS_RESOURCE_PATH;
            Console.WriteLine(resourceName);

            if (!File.Exists(resourceName))
            {
                throw new ClientException("EndpointsFile not found");
            }

            using(StreamReader r = new StreamReader(resourceName))
            {
                var json = r.ReadToEnd();
                var job = JObject.Parse(json);
                foreach (var item in job.Properties())
                {
                    foreach (var productItem in item.Value)
                    {
                        var domain = (string) ((JValue) ((JProperty) productItem).Value).Value;
                        var regionId = ((JProperty) (productItem)).Name;
                        Endpoint endpoint = new Endpoint(item.Name, regionId, domain);
                        var productName = item.Name;
                        this.AddEndpoint(productName, regionId, domain);
                    }
                }
            }
        }

        public Endpoint AddEndpoint(string product, string regionId, string domain)
        {
            Endpoint endpoint = new Endpoint(product, regionId, domain);
            DictionaryUtil.Add<Endpoint>(endpoints, product + "_" + regionId, endpoint);

            Dictionary<string, string> regionDictionary = new Dictionary<string, string> { };
            DictionaryUtil.Add<string>(regionDictionary, regionId, domain);
            DictionaryUtil.Add<Dictionary<string, string>>(endpointsDictionary, product, regionDictionary);
            return endpoint;
        }
    }
}
