using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkNet5.Domain
{
    public class Team : BaseDomain
    {
        public string Name { get; set; }
        public int LeagueId { get; set; }
        public virtual League League { get; set; }
        public virtual Coach Coach { get; set; }

        public virtual ICollection<Match> HomeMatches { get; set; }
        public virtual ICollection<Match> AwayMatches { get; set; }
    }
}
