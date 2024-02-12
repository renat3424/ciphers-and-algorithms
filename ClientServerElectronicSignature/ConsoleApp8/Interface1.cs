using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp8
{
    [ServiceContract]
    interface IS
    {
        [OperationContract]
        BigInteger GetE();

        [OperationContract]
        BigInteger GetN();

        [OperationContract]
        bool CheckKey(string realMsg, string Key);

      
    }
}
