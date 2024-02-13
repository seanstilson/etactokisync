using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EtacToKiSync.M3.Models;
using EtacToKiSync.Entities.Ki;
using M3Part = EtacToKiSync.M3.Models.Part;
using Part = EtacToKiSync.Entities.Ki.Part;

namespace EtacToKiSync.Mappers
{
    public class EtacMapper : Profile
    {
        public EtacMapper() 
        {
            CreateMap<Customer, Company>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.AccountNumber, opts => opts.MapFrom(src => src.CustomerNumber));

            CreateMap<Customer, Branch>()
                .ForMember(dest => dest.name, opts => opts.MapFrom<string>(src => src.CustomerName))
                .ForMember(dest => dest.companyId, opts => opts.Ignore())
                .ForMember(dest => dest.id, opts => opts.Ignore())
                .ForMember(dest => dest.addressId, opts => opts.Ignore());

            CreateMap<Customer, Address>()
                .ForMember(dest => dest.lineOne, opts => opts.MapFrom(src => src.CustomerAddress1))
                .ForMember(dest => dest.lineTwo, opts => opts.MapFrom(src => src.CustomerAddress2))
                .ForMember(dest => dest.city, opts => opts.MapFrom(src => src.City))
                .ForMember(dest => dest.state, opts => opts.MapFrom(src => src.County))
                .ForMember(dest => dest.postalCode, opts => opts.MapFrom(src => src.PostalCode))
                .ForMember(dest => dest.country, opts => opts.MapFrom(src => src.CountryCode));

            CreateMap<M3Part, Part>()
                .ForMember(dest => dest.businessEntityERPCompany, opts=>opts.MapFrom(src=>src.company));

            CreateMap<CustomerRecord, Company>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.CUNM))
                .ForMember(dest => dest.AccountNumber, opts => opts.MapFrom(src => src.CUNO));

            CreateMap<AddressRecord, Address>()
                .ForMember(dest => dest.lineOne, opts => opts.MapFrom(src => src.CUA1))
                .ForMember(dest => dest.lineTwo, opts => opts.MapFrom(src => src.CUA2))
                .ForMember(dest => dest.city, opts => opts.MapFrom(src => src.TOWN))
                .ForMember(dest => dest.state, opts => opts.MapFrom(src => src.ECAR))
                .ForMember(dest => dest.postalCode, opts => opts.MapFrom(src => src.PONO))
                .ForMember(dest => dest.country, opts => opts.MapFrom(src => src.CSCD));

            CreateMap<Company, Branch>()
                .ForMember(dest => dest.companyId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.company, opts => opts.MapFrom(src => src));

            CreateMap<PartRecord, Part>()
                .ForMember(dest => dest.number, opts => opts.MapFrom(src => src.ITNO))
                .ForMember(dest => dest.description, opts => opts.MapFrom(src => MapDescription(src)));
        }

        private string MapDescription(PartRecord src)
        {
            if (string.IsNullOrEmpty(src.FUDS) && !string.IsNullOrEmpty(src.ITDS))
                return src.ITDS;
            else if (string.IsNullOrEmpty(src.ITDS) && !string.IsNullOrEmpty(src.FUDS))
                return src.FUDS;
            else if (!string.IsNullOrEmpty(src.FUDS) && !string.IsNullOrEmpty(src.ITDS))
                return $"{src.FUDS},{src.ITDS}";
            else
                return string.Empty;
        }
        
    }
}
