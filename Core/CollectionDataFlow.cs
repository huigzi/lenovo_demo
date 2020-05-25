using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Core.Interface;

namespace Core
{
    public class CollectionDataFlow : IDataFlow
    {
        private readonly ActionBlock<byte[]> action;

        public CollectionDataFlow(ActionBlock<byte[]> action)
        {
            this.action = action;
        }

        public void Post(byte[] data)
        {
            action.Post(data);
        }
    }
}
