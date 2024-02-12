using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp8
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var uri = new Uri("net.tcp://localhost:6565/S");
            IS service = new S();
            ServiceHost host = new ServiceHost(service, uri);
            var binding = new NetTcpBinding(SecurityMode.None);
            host.AddServiceEndpoint(typeof(IS), binding, "");
            host.Open();






            Console.ReadKey();
            
        }
    }
}
