using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface ISaveData<T> 
    {
        DataState State { get; set; }

        void StartSave();
        void StopSave();
        T WriteData(T bytes);
        void WriteProcess(T bytes);
    }
}
