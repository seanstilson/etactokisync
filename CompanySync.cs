// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using EtacToKiSync.Data;
using EtacToKiSync.Entities.Ki;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using EtacToKiSync.M3.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Part = EtacToKiSync.Entities.Ki.Part;
using M3Part = EtacToKiSync.M3.Models.Part;
using EtacToKiSync.Services;
using EtacToKiSync.EventGrid;
using System.Collections.Generic;

namespace EtacToKiSync
{
    public class CompanySync
    {
        private ComplaintsDBContext _context;
        private ILogger _logger;
        private IAuthenticationService _authenticationService;
        private IM3API _m3API;

        public CompanySync(ComplaintsDBContext context, IAuthenticationService service, IM3API m3API)
        {
            _context = context;
            _authenticationService = service;
            _m3API = m3API;
        }
        /// <summary>
        /// Based on whether the address is in the first line and postal code match an existing entry or not,
        /// either the address is added or updated.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private async Task<Address> CheckAddressExistsAddOrUpdate(Address address)
        {
            try
            {
                var dbAddr = _context.Address.FirstOrDefault(a => a.lineOne == address.lineOne && a.postalCode == address.postalCode);
                if (dbAddr == null)
                {
                    int maxId = _context.Address.Max(a => a.id);
                    address.id = maxId + 1;
                    _context.Address.Add(address);
                    await _context.SaveChangesAsync();
                    return address;
                }
                else
                {
                    dbAddr.postalCode = address.postalCode;
                    dbAddr.city = address.city;
                    dbAddr.state = address.state;
                    dbAddr.country = address.country;
                    dbAddr.lineTwo = address.lineTwo;
                    _context.Address.Update(dbAddr);
                    await _context.SaveChangesAsync();
                    return dbAddr;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<Branch> AddNewBranch(Branch branch)
        {
            try
            {
                int id = _context.Branch.Max(b => b.id);
                branch.id = id + 1;
                _context.Branch.Add(branch);
                await _context.SaveChangesAsync();
                return branch;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private async Task<Branch> UpdateBranch(Branch branch)
        {
            try
            {
                var existing = await _context.Branch.SingleOrDefaultAsync(b => b.companyId == branch.companyId);
                existing.name = branch.name;
                existing.addressId = branch.addressId;
                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<BackendCustomerRecord> GetCustomerRecordFromM3API(string accountNumber)
        {
            _logger.LogInformation("Made it into the async Task Run version of the Run handler.");
            TokenResponse token = await _authenticationService.Authenticate(SessionInfo.Session.EtacAuthToken);
            SessionInfo.Session.Token = token;

            try 
            {
                if (SessionInfo.Session.Token == null)
                {
                    return null;
                }
                BackendCustomerRecord record = await _m3API.GetM3BasicCustomerData(accountNumber);
                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving address record for account number {accountNumber}");
                return null;
            }

        }
        private async Task<BackendPartRecord> GetPartRecordFromM3API(string partNumber)
        {
            _logger.LogInformation("Made it into the async Task Run version of the Run handler.");
            TokenResponse token = await _authenticationService.Authenticate(SessionInfo.Session.EtacAuthToken);
            SessionInfo.Session.Token = token;

            try
            {
                if (SessionInfo.Session.Token == null)
                {
                    return null;
                }
                BackendPartRecord record = await _m3API.GetM3BasicPartData(partNumber);
                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving part record for part number {partNumber}");
                return null;
            }

        }

        private async Task<BackendOrderInfoRecord> GetOrderInfoRecordFromM3API(string accountNumber)
        {
            _logger.LogInformation("Made it into the async Task Run version of the Run handler.");
            TokenResponse token = await _authenticationService.Authenticate(SessionInfo.Session.EtacAuthToken);
            SessionInfo.Session.Token = token;

            try
            {
                if (SessionInfo.Session.Token == null)
                {
                    return null;
                }
                BackendOrderInfoRecord record = await _m3API.GetM3OrderInfoData(accountNumber);
                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving address record for account number {accountNumber}");
                return null;
            }

        }
        /// <summary>
        /// Checks existence of Company and either adds it if it does not exist or updates it if it does.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        private async Task<bool> CheckExistsAddUpdateCompany(Company company)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request in UpdateExistingCompany.");

            try
            {
                bool succeeded = true;

                var theOne = await _context.Company.SingleOrDefaultAsync(c => c.AccountNumber == company.AccountNumber);
                if (theOne == null)
                    theOne = await AddNewCompany(company);
                else
                    succeeded = await UpdateExistingCompany(company);

                return theOne != null && succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CheckExistAndAddUpdateCompany: {ex.StackTrace}");
                return false;
            }
        }

        private async Task<bool> UpdateExistingCompany(Company company)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request in UpdateExistingCompany.");
            try
            {
                var existing = await _context.Company.SingleOrDefaultAsync(c => c.AccountNumber == company.AccountNumber);
                existing.Name = company.Name;
                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating existing company: {ex.StackTrace}");
                return false;
            }
        }

        private async Task<Company> AddNewCompany(Company company)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request in AddNewCompany.");

            try
            {
                int id = _context.Company.Max(c => c.Id);
                company.Id = id + 1;
                _context.Company.Add(company);
                await _context.SaveChangesAsync();
                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddNewCompany: {ex.StackTrace}");
                return null;
            }
        }

        private async Task<Branch> CheckExistsAndAddUpdateBranch(Branch branch)
        {
            var existing = await _context.Branch.FirstOrDefaultAsync(b => b.companyId == branch.companyId);
            Branch dbBranch = null;
            if (existing == null)
                dbBranch = await AddNewBranch(branch);
            else
                dbBranch = await UpdateBranch(branch);

            return dbBranch;
        }

        private async Task<bool> CustomerExists(Company company, ComplaintsDBContext dbContext)
        {
            return await dbContext.Company.AnyAsync(c => c.Name == company.Name);
        }

        [FunctionName("PartSync")]
        public async Task RunPart([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            _logger = log;
            if (_logger != null) //If the logger is null, that means DI failed to work and basically nothing is going to run correctly.
            {
                _logger.LogInformation("Made it into the Task RunPart.");
                _logger.LogInformation($"Value of the data coming from the event: {eventGridEvent.Data.ToString()}");

                if (_context == null)
                {
                    _logger.LogInformation("ComplaintsDBContext is null");
                    return;
                }
                _logger.LogInformation("ComplaintsDBContext is not null");

                var mapper = SessionInfo.Session.Mapper;
                string sdata = eventGridEvent.Data.ToString();
                _logger.LogInformation($"Value of the part number coming in: {sdata}");                

                BackendPartRecord M3PartMaster = null;
                Part newPart = null;

                using (_context /*new ComplaintsDBContext(optionsBuilder.Options) */)
                {
                    string spart = eventGridEvent.Data.ToString();
                    _logger.LogInformation($"Value of the part coming in: {spart}");

                    EventGridPartData data = JsonConvert.DeserializeObject<EventGridPartData>(spart);
                    if (data == null)
                    {
                        _logger.LogError("Error deserializing event grid part number from Etac.");
                        return;
                    }
                    if (string.IsNullOrEmpty(data.PartNumber))
                    {
                        _logger.LogError("Missing part number.");
                        return;
                    }

                    M3PartMaster = await GetPartRecordFromM3API(data.PartNumber);
                    if (M3PartMaster == null)
                    {
                        _logger.LogError($"Unable to retrieve part with item number {data.PartNumber} from M3");
                        return;
                    }
                    
                    foreach(var rec in M3PartMaster.results)
                    {
                        foreach(var part in rec.records)
                        {
                            newPart = mapper.Map<Part>(part);
                            var parts = await _context.Part.Where(p => p.number == newPart.number).ToListAsync();
                            if (parts != null && parts.Any())
                            {
                                var existing = parts.First();
                                existing.description = newPart.description;
                                _context.Entry(existing).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Successfully updated existing Part.");
                                return;
                            }
                            int max = _context.Part.Max(p => p.id) + 1;
                            newPart.id = max;
                            newPart.businessEntityERPCompany = "CLT";
                            await _context.Part.AddAsync(newPart);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Successfully added new part to database, ID={max}");
                        }
                    }
                }
            }
        }

        [FunctionName("AddressSync")]
        public async Task RunAddress([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            _logger = log;
            _logger.LogInformation("Made it into the async Task Run version of the Run handler.");
            TokenResponse token = await _authenticationService.Authenticate(SessionInfo.Session.EtacAuthToken);
            SessionInfo.Session.Token = token;

        }

        private async Task<int> AddOrUpdateCompany(Company company)
        {
            var existing = _context.Company.SingleOrDefault(c => c.AccountNumber == company.AccountNumber);
            var branches = company.Branches;
            company.Branches = null; // Have to null these out so the database does not error.

            int totalUpdated = 0;
            if ( existing == null ) 
            {
                int maxCOId = _context.Company.Max(c => c.Id) + 1;
                company.Id = maxCOId;
                _context.Company.Add(company);
                await _context.SaveChangesAsync();
                totalUpdated++;
            }
            else
            {
                existing.Name = company.Name;
                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                totalUpdated++;
            }
            foreach ( Branch branch in branches ) 
            {
                if ( branch.address != null ) 
                {
                    // Really the only way I can think of comparing the addresses.
                    var existingA = await _context.Address.Where(a => a.lineOne == branch.address.lineOne &&
                                    a.city == branch.address.city && a.state == branch.address.state && branch.address.postalCode == a.postalCode).FirstOrDefaultAsync();
                    if ( existing == null )
                    {
                        int maxId = _context.Address.Max(a => a.id) + 1;
                        branch.address.id = maxId;
                        branch.address.Branch = null;
                        _context.Address.Add(branch.address);
                        await _context.SaveChangesAsync();
                        totalUpdated++;
                        branch.addressId = maxId;
                        branch.companyId = company.Id;
                        _context.Branch.Add(branch);
                        await _context.SaveChangesAsync();
                        totalUpdated++;
                    }
                }
            }
            return totalUpdated;
        }

        /// <summary>
        /// This is the main entry point for the Customer/Company sync "backend".  This will eventually probably be ported to the ERP piece.
        /// </summary>
        /// <param name="eventGridEvent"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CompanySync")]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            _logger = log;
            _logger.LogInformation("Made it into the async Task Run version of the Run handler.");
            const string defaultAddressID = "C001";

        //    TokenResponse token = await _authenticationService.Authenticate(SessionInfo.Session.EtacAuthToken);
        //    SessionInfo.Session.Token = token;

            if (_logger != null)
            {
                _logger.LogInformation($"Value of the data coming from the event: {eventGridEvent.Data.ToString()}");
                if (_context == null)
                    _logger.LogInformation("ComplaintsDBContext is null");
                else
                    _logger.LogInformation("ComplaintsDBContext is not null");

                //Customer customer = null;
                Company newComp = null;
                var mapper = SessionInfo.Session.Mapper;
                string sdata = eventGridEvent.Data.ToString();
                _logger.LogInformation($"Value of the account number coming in: {sdata}");

                try
                {
                    EventGridCustomerData data = JsonConvert.DeserializeObject<EventGridCustomerData>(sdata);
                    var customerResult = await GetCustomerRecordFromM3API(data.CustomerNumber);
                    if (customerResult != null && customerResult.results != null)
                    {
                        List<Company> companyList = new List<Company>();
                        foreach (var custRecord in customerResult.results)
                        {
                            foreach (CustomerRecord customer in custRecord.records)
                            {
                                BackendOrderInfoRecord orderInforec = await _m3API.GetM3OrderInfoData(customerResult.results.FirstOrDefault()?.records?.FirstOrDefault()?.CUNO);
                                string addrId = orderInforec?.results?.FirstOrDefault()?.records?.FirstOrDefault()?.ADID ?? defaultAddressID;
                                Company company = mapper.Map<Company>(customer);
                                var addressContainer = await _m3API.GetM3AddressData(data.CustomerNumber, addrId);
                                List<Address> addresses = new List<Address>();
                                company.Branches = new List<Branch>();
                                foreach (var address in addressContainer.results)
                                {
                                    address.records.ForEach(ar =>
                                    {
                                        Address address = mapper.Map<Address>(ar);
                                        addresses.Add(address);
                                        Branch b = mapper.Map<Branch>(company);
                                        b.address = address;
                                        company.Branches.Add(b);
                                    });
                                }
                                companyList.Add(company);
                            }
                        }
                        int totalUpdated = 0;
                        foreach (var company in companyList)
                        {
                            totalUpdated += await AddOrUpdateCompany(company);
                        }
                    }                                      
                    //customer = JsonConvert.DeserializeObject<Customer>(sCust);
                    //newComp = mapper.Map<Customer, Company>(customer);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error mapping customer to company. Details: {ex.StackTrace}");
                    return;
                }

               /** using (_context )
                {
                    bool success = await CheckExistsAddUpdateCompany(newComp);
                    if (success)
                    {
                        addedCompany = await _context.Company.SingleOrDefaultAsync(c => c.AccountNumber == newComp.AccountNumber);
                        Address address = mapper.Map<Customer, Address>(customer);
                        var newAddr = await CheckAddressExistsAddOrUpdate(address);
                        Branch branch = mapper.Map<Customer, Branch>(customer);
                        branch.companyId = addedCompany.Id;
                        if (newAddr != null)
                            branch.addressId = newAddr.id;
                        Branch updatedBranch = await CheckExistsAndAddUpdateBranch(branch);
                        if (updatedBranch != null && newAddr != null)
                        {
                            updatedBranch.address = newAddr;
                            updatedBranch.company = addedCompany;
                            addedCompany.Branches = new System.Collections.Generic.List<Branch> { updatedBranch };
                        }
                    }

                    string sbag = string.Empty;
                    if (addedCompany != null)
                        sbag = JsonConvert.SerializeObject(addedCompany, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    // Company theOne = null;
                    string responseMessage = sbag;

                    _logger.LogInformation($"Successfully add/updated customer {responseMessage}");

                    return;
                } **/
            } 
            return;
        }
    }
}
