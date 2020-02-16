using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Core.Interface;

namespace Core
{
    public class DataFlowBlock
    {
        private TransformBlock<byte[], byte[]> _transformBlock1;
        private TransformBlock<byte[], ArrayList> _transformBlock2;

        public DataFlowBlock(ISaveData<byte[]> saveData, IAlgorithmFlow<short[]> algorithmFlow, ActionBlock<ArrayList> action)
        {
            _transformBlock1 = new TransformBlock<byte[], byte[]>(saveData.WriteData);

            //_transformBlock2 = new TransformBlock<byte[], ArrayList>(x => algorithmFlow.DataProcess(x));

            //dummy data for test
            _transformBlock2 = new TransformBlock<byte[], ArrayList>(x => new ArrayList());

            _transformBlock2.LinkTo(action);
        }

        public void Post(byte[] data)
        {
            _transformBlock2.Post(data);
        }
    }
}
