namespace Aliyun.Acs.Core.Endpoints
{
    public class Endpoint
    {
        private string product { get; set; }
        private string regionId { get; set; }
        private string domain { get; set; }

        public Endpoint(string product, string regionId, string domain)
        {
            this.product = product;
            this.regionId = regionId;
            this.domain = domain;
        }

        public string GetIndex()
        {
            return this.product + "_" + this.regionId;
        }

        public string GetDomain()
        {
            return this.domain;
        }
    }
}
