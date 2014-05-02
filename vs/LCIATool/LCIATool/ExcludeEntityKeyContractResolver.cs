using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Data;

namespace LCIATool
{
    public class ExcludeEntityKeyContractResolver : DefaultContractResolver
    {
        private static Type mCollectionType = typeof(System.Data.Objects.DataClasses.RelatedEnd);

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var members = GetSerializableMembers(type);

            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            IList<JsonProperty> serializeProperties = new List<JsonProperty>();

            for (int i = 0; i < properties.Count; i++)
            {
                var memberInfo = members.Find(p => p.Name == properties[i].PropertyName);
                if (!memberInfo.GetCustomAttributes(false).Any(a => a is SoapIgnoreAttribute) && properties[i].PropertyType != typeof(System.Data.EntityKey))
                {
                    serializeProperties.Add(properties[i]);
                }
            }
            return serializeProperties;
        }
    }
}