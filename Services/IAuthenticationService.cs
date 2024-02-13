using EtacToKiSync.M3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.Services
{
    public interface IAuthenticationService
    {
        public Task<TokenResponse> Authenticate(string azureSecret);
    }
}
