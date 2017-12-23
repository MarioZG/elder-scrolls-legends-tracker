using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ESLTracker.Utils
{
    class SerializationHelper
    {
        public static T DeserializeXML<T>(string xml)
        {
            T result;
            using (TextReader reader = new StringReader(xml))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                result = (T)xmlSerializer.Deserialize(reader);
            }
            return result;
        }

        public static string SerializeXML<T>(T instance)
        {
            StringBuilder output = new StringBuilder();
            using (TextWriter writer = new StringWriter(output))
            {
                var xml = new XmlSerializer(typeof(T));
                xml.Serialize(writer, instance);
            }
            return output.ToString();
        }

        public static T DeserializeJson<T>(string json)
        {
            T jobs;

            var js = new JsonSerializer();
            using (StringReader sr = new StringReader(json))
            {
                using (var jr = new JsonTextReader(sr))
                {
                    jobs = js.Deserialize<T>(jr);
                }
            }
            return jobs;
        }

        public static string SerializeJson<T>(T objectToSerialise)
        {
            var sb = new StringBuilder();
            var js = new JsonSerializer();
            using (StringWriter sr = new StringWriter(sb))
            {
                js.Serialize(sr, objectToSerialise);
            }
            return sb.ToString();
        }
    }
}
