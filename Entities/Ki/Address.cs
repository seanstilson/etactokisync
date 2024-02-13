using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.Entities.Ki
{
    [Table("Address")]
    public class Address
    {
        [Key]
        public int id { get; set; }
        public string lineOne { get; set; }
        public string lineTwo { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        //@Convert( converter=CountryConverter.class ) someday it would be nice to implement an AttributeConverter
        public string country { get; set; }
        public virtual Branch Branch {get; set;}
    }
}
