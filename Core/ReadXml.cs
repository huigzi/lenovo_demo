using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Core
{
    public class ReadXml : IReadFile
    {

        public List<int> ReadJsonFile()
        {
            return new List<int>();
        }

        public List<int> ReadXmlFile()
        {
            try
            {
                List<int> result = new List<int>();

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load("config.xml");

                XmlNode root = xmlDoc.SelectSingleNode("configuration"); //指向根节点

                XmlNode xn0 = root?.SelectSingleNode("dthres");
                result.Add(int.Parse(xn0?.InnerText ?? throw new InvalidOperationException("dthres missing")));

                XmlNode xn1 = root.SelectSingleNode("thres");
                result.Add(int.Parse(xn1?.InnerText ?? throw new InvalidOperationException("thres missing")));

                XmlNode xn2 = root.SelectSingleNode("det_thres");
                result.Add(int.Parse(xn2?.InnerText ?? throw new InvalidOperationException("det_thres missing")));

                XmlNode xn3 = root.SelectSingleNode("det_miss_thre");
                result.Add(int.Parse(xn3?.InnerText ?? throw new InvalidOperationException("det_miss_thre missing")));

                XmlNode xn4 = root.SelectSingleNode("mv_thres");
                result.Add(int.Parse(xn4?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn5 = root.SelectSingleNode("fb");
                result.Add(int.Parse(xn5?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn6 = root.SelectSingleNode("ff");
                result.Add(int.Parse(xn6?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn7 = root.SelectSingleNode("lhp");
                result.Add(int.Parse(xn7?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn8 = root.SelectSingleNode("trough_thre");
                result.Add(int.Parse(xn8?.InnerText ?? throw new InvalidOperationException("trough_thre missing")));

                XmlNode xn9 = root.SelectSingleNode("tail_thres");
                result.Add(int.Parse(xn9?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn10 = root.SelectSingleNode("tail_len");
                result.Add(int.Parse(xn10?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn11 = root.SelectSingleNode("tailn_thres");
                result.Add(int.Parse(xn11?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn12 = root.SelectSingleNode("head_thres");
                result.Add(int.Parse(xn12?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn13 = root.SelectSingleNode("head_len");
                result.Add(int.Parse(xn13?.InnerText ?? throw new InvalidOperationException()));

                XmlNode xn14 = root.SelectSingleNode("headln_thres");
                result.Add(int.Parse(xn14?.InnerText ?? throw new InvalidOperationException()));

                return result;

            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
