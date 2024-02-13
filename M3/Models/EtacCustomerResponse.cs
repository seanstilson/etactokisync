using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.M3.Models
{
    public class CustomerRecord
    {
        public string CONO { get; set; }
        public string DIVI { get; set; }
        public string LNCD { get; set; }
        public string CUNO { get; set; }
        public string CUNM { get; set; }
        public string CUA1 { get; set; }
        public string CUA2 { get; set; }
        public string CUA3 { get; set; }
        public string CUA4 { get; set; }
        public string PONO { get; set; }
        public string PHNO { get; set; }
        public string PHN2 { get; set; }
        public string TFNO { get; set; }
        public string CUTP { get; set; }
        public string ALCU { get; set; }
        public string YREF { get; set; }
        public string YRE2 { get; set; }
        public string MAIL { get; set; }
        public string CSCD { get; set; }
        public string ECAR { get; set; }
        public string CFC1 { get; set; }
        public string CFC2 { get; set; }
        public string CFC3 { get; set; }
        public string CFC4 { get; set; }
        public string CFC5 { get; set; }
        public string CFC6 { get; set; }
        public string CFC7 { get; set; }
        public string CFC8 { get; set; }
        public string CFC9 { get; set; }
        public string CFC0 { get; set; }
        public string CESA { get; set; }
        public string STAT { get; set; }
        public string LMDT { get; set; }
        public string PYOP { get; set; }
        public string TOWN { get; set; }
        public string CUCD { get; set; }
        public string MCOS { get; set; }
        public string FRCO { get; set; }
        public string CUSU { get; set; }
        public string EALO { get; set; }
        public string RASN { get; set; }
        public string SPLE { get; set; }
        public string LSID { get; set; }
        public string LSAD { get; set; }
        public string MEAL { get; set; }
        public string HAFE { get; set; }
        public string TXID { get; set; }
        public string RGDT { get; set; }
        public string TXCO { get; set; }
        public string GEOC { get; set; }
        public string CHNO { get; set; }
        public string EDES { get; set; }
        public string ADID { get; set; }
    }

    public class CustomerResult
    {
        public string transaction { get; set; }
        public List<CustomerRecord> records { get; set; }
    }

    public class BackendCustomerRecord
    {
        public List<CustomerResult> results { get; set; }
        public bool wasTerminated { get; set; }
        public int nrOfSuccessfullTransactions { get; set; }
        public int nrOfFailedTransactions { get; set; }
    }
}
