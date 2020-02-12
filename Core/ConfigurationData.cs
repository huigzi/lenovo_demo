using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ConfigurationData
    {
        public Dictionary<string, int> ConfigurationGroupInt { get; set; }

        public Dictionary<string, float> ConfigurationGroupFloat { get; set; }
    }
}
