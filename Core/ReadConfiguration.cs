using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Core
{
    public class ReadConfiguration : IReadFile
    {
        public ConfigurationData ReadJsonFile()
        {
            using (var fs = new FileStream("configuration.json", FileMode.Open))
            {
                using (var streamReader = new StreamReader(fs, Encoding.UTF8))
                {
                    var jsonData = streamReader.ReadToEnd();

                    return JsonConvert.DeserializeObject<ConfigurationData>(jsonData);
                }
            }
        }
    }
}
