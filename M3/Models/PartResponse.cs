using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.M3.Models
{
    public class PartRecord
    {
        public string ITNO { get; set; }
        public string ITDS { get; set; }
        public string FUDS { get; set; }
    }

    public class PartResult
    {
        public string transaction { get; set; }
        public List<PartRecord> records { get; set; }
    }

    public class BackendPartRecord
    {
        public List<PartResult> results { get; set; }
        public bool wasTerminated { get; set; }
        public int nrOfSuccessfullTransactions { get; set; }
        public int nrOfFailedTransactions { get; set; }
    }
}
