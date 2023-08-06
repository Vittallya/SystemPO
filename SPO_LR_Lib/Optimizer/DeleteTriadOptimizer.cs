using SPO_LR_Lib.Triads;

namespace SPO_LR_Lib
{
    public class DeleteTriadOptimizer : ITriadOptimizer
    {
        public IEnumerable<Triad?> Optimize(IEnumerable<Triad?> triads)
        {
            var d = new Dictionary<Triad, MutableTriad>();
            var d1 = new Dictionary<MutableTriad, Triad>();
            IEnumerable<MutableTriad> mutableTriads = triads.Select(y => y.GetMutableTriad(d));
            return ((ITriadOptimizer)this).Optimize(mutableTriads).Select(y => y.GetImmutableTriad(d1));

        }

        IEnumerable<MutableTriad> ITriadOptimizer.Optimize(IEnumerable<MutableTriad?> triads)
        {
            int i = 0;

            foreach (MutableTriad triad in triads)
            {


                i++;
            }
            throw new Exception();
        }
    }
}
