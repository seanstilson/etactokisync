using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.Entities.Ki
{
    [Table("Part")]
    public class Part
    {
        [Key]
        public int id { get; set; }
        public string number { get; set; }
        public string description { get; set; }
        public string businessEntityERPCompany { get; set; }
    }
}
