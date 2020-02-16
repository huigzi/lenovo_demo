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
        private ActionBlock<ArrayList> _actionBlock;

        private ISaveData<byte[]> saveData;
        private IAlgorithmFlow<short[]> algorithmFlow;
        private ActionBlock<ArrayList> action;

        public DataFlowBlock(ISaveData<byte[]> saveData, IAlgorithmFlow<short[]> algorithmFlow, ActionBlock<ArrayList> action)
        {
            this.saveData = saveData;
            this.algorithmFlow = algorithmFlow;
            this.action = action;

            _transformBlock1 = new TransformBlock<byte[], byte[]>(x => saveData.WriteData(x));

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
