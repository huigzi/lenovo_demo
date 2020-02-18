using Core.Interface;
using System.Collections;
using System.Threading.Tasks.Dataflow;

namespace Core
{
    public class DataFlowBlock
    {
        private TransformBlock<byte[], byte[]> _transformBlock1;
        private TransformBlock<byte[], State> _transformBlock2;

        public DataFlowBlock(ISaveData<byte[]> saveData, IAlgorithmFlow<short[]> algorithmFlow, ActionBlock<State> action)
        {
            _transformBlock1 = new TransformBlock<byte[], byte[]>(saveData.WriteData);

            //_transformBlock2 = new TransformBlock<byte[], ArrayList>(x => algorithmFlow.DataProcess(x));

            //dummy data for test
            _transformBlock2 = new TransformBlock<byte[], State>(x => State.SomeOne);

            _transformBlock2.LinkTo(action);
        }

        public void Post(byte[] data)
        {
            _transformBlock2.Post(data);
        }
    }
}
