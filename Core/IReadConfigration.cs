using System.Collections.Generic;

namespace Core
{
    public interface IReadFile
    {
        List<int> ReadXmlFile();
        List<int> ReadJsonFile();
    }
}
