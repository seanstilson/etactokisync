using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.M3.Models
{
    public class OrderInfo
    {
        public string CONO { get; set; }
        public string CUNO { get; set; }
        public string ADID { get; set; }
    }

    public class OrderInfoRequestResult
    {
        public string transaction { get; set; }
        public List<OrderInfo> records { get; set; }
    }

    public class BackendOrderInfoRecord
    {
        public List<OrderInfoRequestResult> results { get; set; }
        public bool wasTerminated { get; set; }
        public int nrOfSuccessfullTransactions { get; set; }
        public int nrOfFailedTransactions { get; set; }
    }
}
