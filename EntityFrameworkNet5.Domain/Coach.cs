using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkNet5.Domain
{
    public class Coach : BaseDomain
    {
        public string Name { get; set; }
        public int? TeamId { get; set; }
        public Team Team { get; set; }
    }
}
