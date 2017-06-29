using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zane.LogHub.Client
{
    public class ContentEntity
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ContentType
        {
            Value,
            Exception,
            HttpRequest,
            Html,
            Json,
            Other
        }
        public string Type { get; set; }
        public object Content { get; set; }

        public ContentEntity() { }
        public ContentEntity(ContentType type, object content)
        {
            this.Type = type.ToString();
            this.Content = content;
        }
        public ContentEntity(string type, object content)
        {
            this.Type = type;
            this.Content = content;
        }
        public ContentEntity(object obj)
        {
            if (obj == null)
            {
                this.Type = ContentType.Value.ToString();
                this.Content = null;
            }
            else if (obj is ContentEntity)
            {
                var item = obj as ContentEntity;
                this.Type = item.Type;
                this.Content = item.Content;
            }
            else if (obj is System.ValueType)//值类型数据无需格式化
            {
                this.Type = ContentType.Value.ToString();
                this.Content = obj;
            }
            else if (obj is Exception)
            {
                this.Type = ContentType.Exception.ToString();
                this.Content = obj;
            }
            else if (obj is Microsoft.AspNetCore.Http.HttpRequest)
            {
                this.Type = ContentType.HttpRequest.ToString();
                this.Content = Object2Json.HttpRequest2Json(obj as Microsoft.AspNetCore.Http.HttpRequest);
            }
            else if (obj is string)
            {
                string str = obj.ToString().Trim().ToLower();
                if (str.StartsWith("<!doctype") || str.StartsWith("<html"))
                {
                    this.Type = ContentType.Html.ToString();
                    this.Content = obj;
                }
                else
                {
                    this.Type = ContentType.Value.ToString();
                    this.Content = obj;
                }
            }
            else
            {
                
                if (obj.GetType().GetTypeInfo().IsClass)
                {
                    this.Type = obj.GetType().Name;
                }
                else
                {
                    this.Type = ContentType.Other.ToString();
                }
                this.Content = obj;
            }
        }
    }
}
