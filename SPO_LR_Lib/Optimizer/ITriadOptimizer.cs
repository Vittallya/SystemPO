using SPO_LR_Lib.Triads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPO_LR_Lib
{
    public interface ITriadOptimizer
    {
        public IEnumerable<Triad?> Optimize(IEnumerable<Triad?> triads);

        internal IEnumerable<MutableTriad> Optimize(IEnumerable<MutableTriad> triads);
    }
}
