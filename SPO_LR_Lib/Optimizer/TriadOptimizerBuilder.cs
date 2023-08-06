namespace SPO_LR_Lib.Optimizer
{
    public class TriadOptimizerBuilder
    {
        private List<ITriadOptimizer> _optimizers = new();

        public void AddTriadOptimizer<TOptimizer>()
            where TOptimizer: ITriadOptimizer, new()
        {
            _optimizers.Add(new TOptimizer());
        }
        public TriadOptimizerBuilder AddDeleteOptimizer() { AddTriadOptimizer<DeleteTriadOptimizer>(); return this; }
        public TriadOptimizerBuilder AddReduceOptimizer() { AddTriadOptimizer<ReduceTriadOptimizer>(); return this; }

        public ITriadOptimizer Build()
        {
            return new FullTriadOptimizer(_optimizers.ToArray());
        }
    }
}
