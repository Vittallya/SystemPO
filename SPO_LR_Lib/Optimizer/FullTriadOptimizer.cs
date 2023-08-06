using SPO_LR_Lib.Triads;

namespace SPO_LR_Lib.Optimizer
{
    internal class FullTriadOptimizer : ITriadOptimizer
    {
        private readonly List<ITriadOptimizer> optimizers;

        public FullTriadOptimizer(params ITriadOptimizer[] optimizers)
        {
            this.optimizers = optimizers.ToList();
        }

        public IEnumerable<Triad?> Optimize(IEnumerable<Triad?> triads)
        {
            var d1 = new Dictionary<Triad, MutableTriad>();
            var d2 = new Dictionary<MutableTriad, Triad>();

            var result = triads.Select(x => x.GetMutableTriad(d1)).ToList();
            result = ((ITriadOptimizer)this).Optimize(result).ToList();
            return result.Select(y => y.GetImmutableTriad(d2)).ToList();
        }


        IEnumerable<MutableTriad> ITriadOptimizer.Optimize(IEnumerable<MutableTriad> triads)
        {
            optimizers.ForEach(x => triads = x.Optimize(triads));
            return triads;
        }
    }
}
