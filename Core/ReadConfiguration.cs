using Core.Interface;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Core
{
    public class ReadConfiguration : IReadFile
    {
        public ConfigurationData ReadFile()
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
