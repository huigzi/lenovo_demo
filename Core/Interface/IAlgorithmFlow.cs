using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IAlgorithmFlow<T> where T : class
    {
        State DataProcess((T, T) bytes);

        State DataProcess(byte[] bytes);

        void DataInitial(byte[] bytes);
    }

}
