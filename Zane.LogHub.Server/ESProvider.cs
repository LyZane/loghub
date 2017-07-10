using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using Zane.LogHub.Client;

namespace Zane.LogHub.Server
{
    public static class ESProvider
    {
        public static ElasticClient client;
        static ESProvider()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).BasicAuthentication("elastic", "changeme");
            client = new ElasticClient(settings);

            // 随便请求个接口，检查当前配置是否有效。
            var result = client.GetLicense(new GetLicenseRequest());
            if (!result.ApiCall.Success)
            {
                throw new Exception("访问 Elasticsearch 服务失败，请检查当前配置。");
            }
        }

        internal static bool Populate(this LogEntity log)
        {
            var index = client.Index(log, i => i.Index("loghub_"+log.ApplicationId.ToLower()).Type("log").Id(log.Id));
            if (index.Result == Result.Created || index.Result == Result.Updated)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
