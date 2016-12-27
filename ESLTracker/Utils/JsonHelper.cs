using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ESLTracker.Utils
{
    class JsonHelper
    {
        public static T DeserialiseJson<T>(string json)
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
    }
}
