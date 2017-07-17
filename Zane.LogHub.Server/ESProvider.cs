using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using Zane.LogHub.Client;

namespace Zane.LogHub.Server
{
    public static class ESProvider
    {
        private static ElasticClient client;
        static ESProvider()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).BasicAuthentication("elastic", "changeme");
            settings.MapDefaultTypeNames(t => t.Add(typeof(LogEntity), "log"));
            settings.MapIdPropertyFor<LogEntity>(a=>a.Id);
            client = new ElasticClient(settings);

            // 随便请求个接口，检查当前配置是否有效。
            var result = client.GetLicense(new GetLicenseRequest());
            if (!result.ApiCall.Success)
            {
                throw new Exception("访问 Elasticsearch 服务失败，请检查当前配置。");
            }
        }
        private static string BuildIndexName(string appId)
        {
            return "loghub_" + appId.ToLower();
        }
        public static bool CreateIndex(string appId)
        {
            var descriptor = new CreateIndexDescriptor(BuildIndexName(appId))
                .Mappings(ms => ms
                    .Map<LogEntity>(m => m
                        .Properties(p => p
                            .Text(t => t
                                .Name(n => n.Tag)
                                .Fields(ff => ff
                                    .Text(tt => tt
                                        .Name("tag")
                                    )
                                    .Keyword(k => k
                                        .Name("keyword")
                                    )
                                )
                            )
                        )
                    )
                );
            var response = client.CreateIndex(descriptor);
            return response.Acknowledged;
        }
        internal static bool Populate(this LogEntity log)
        {
            var index = client.Index<LogEntity>(log, i => i.Index(BuildIndexName(log.ApplicationId)));
            if (index.Result == Result.Created || index.Result == Result.Updated)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string[] ListTags(string appId)
        {
            var searcher = new SearchRequest(BuildIndexName(appId));
            
            var aggregations = new AggregationDictionary();
            var nested = new NestedAggregation("Tags")
            {
                Path = "Tag",
                Aggregations = new TermsAggregation("Tag")
                {
                    Field = "tag"
                }
            };

            aggregations["Tags"] = (AggregationContainer)nested;
            searcher.Aggregations = aggregations;


var response = client.Search<LogEntity>(s => s
    .Index(BuildIndexName(appId))
        .Aggregations(a => a
            .Terms("groupbyTag", t => t
            .Field(l => l.Tag)
        )
    )
);
var myAgg = response.Aggs.Terms("groupbyTag");
            
            var result = client.Search<LogEntity>(searcher);
            return null;
        }
    }
}
