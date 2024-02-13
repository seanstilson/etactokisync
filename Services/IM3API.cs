using EtacToKiSync.M3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.Services
{
    public interface IM3API
    {
        public Task<BackendCustomerRecord> GetM3BasicCustomerData(string customerNumber);
        public Task<BackendAddressRecord> GetM3AddressData(string customerNumber, string addressID);
        public Task<BackendOrderInfoRecord> GetM3OrderInfoData(string customerNumber);
        public Task<BackendPartRecord> GetM3BasicPartData(string partNumber);
    }
}
