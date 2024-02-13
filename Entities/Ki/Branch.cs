using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.Entities.Ki
{
    [Table("Branch")]
    public class Branch
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public virtual Company company { get; set; }
        public virtual Address address { get; set; }
        public int companyId { get; set; }
        public int addressId { get; set; }
    }
}
