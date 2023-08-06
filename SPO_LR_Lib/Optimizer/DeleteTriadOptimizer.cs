using SPO_LR_Lib.Triads;

namespace SPO_LR_Lib
{
    public class DeleteTriadOptimizer : ITriadOptimizer
    {
        public IEnumerable<Triad?> Optimize(IEnumerable<Triad?> triads)
        {
            var d = new Dictionary<int, MutableTriad>();
            var d1 = new Dictionary<int, Triad>();
            IEnumerable<MutableTriad> mutableTriads = triads.Select(y => y.GetMutableTriad(d));
            return ((ITriadOptimizer)this).Optimize(mutableTriads).Select(y => y.GetImmutableTriad(d1));

        }

        IEnumerable<MutableTriad> ITriadOptimizer.Optimize(IEnumerable<MutableTriad> triads)
        {
            int i = 1;

            Dictionary<TriadOperand, int> depsCountByOperands = new();
            Dictionary<int, MutableTriad> triadByNum = new();

            //число зависимостей по номеру триады
            Dictionary<int, int> depsCountByTriadNum = new();

            

            foreach (MutableTriad triad in triads)
            {
                if(triad.Operand1 is MutableTriad tr1 && tr1.Operation == "SAME")
                {
                    triad.Operand1 = tr1.Operand1;
                }

                if(triad.Operand2 is MutableTriad tr2 && tr2.Operation == "SAME")
                {
                    triad.Operand2 = tr2.Operand1;
                }

                int a = 0;
                int b = 0;

                if(triad.Operand1 is MutableTriad tr11)
                {
                    a = depsCountByTriadNum[tr11.Number];
                }
                else if(triad.Operand1.LexType == "identifier")
                {
                    if (!depsCountByOperands.TryGetValue(triad.Operand1, out a))
                    {
                        depsCountByOperands.Add(triad.Operand1, 0);
                    }
                }

                if(triad.Operand2 is MutableTriad tr22)
                {
                    b = depsCountByTriadNum[tr22.Number];
                }
                else if(triad.Operand2 != null && triad.Operand2.LexType == "identifier")
                {
                    if (!depsCountByOperands.TryGetValue(triad.Operand2, out b))
                    {
                        depsCountByOperands.Add(triad.Operand2, b);
                    }
                }

                int depi = Math.Max(a, b) + 1;

                MutableTriad? sameFounded = null;

                if(depsCountByTriadNum.Any(x => x.Value == depi && (sameFounded = triadByNum[x.Key]).Equals(triad)))
                {
                    triad.Operation = "SAME";
                    triad.Operand1 = sameFounded;
                    triad.Operand2 = new TriadOperand("constant", "0");
                }

                depsCountByTriadNum.Add(triad.Number, depi);

                if(triad.Operation == ":=" && !depsCountByOperands.TryAdd(triad.Operand1, i))
                {
                    depsCountByOperands[triad.Operand1] = i;
                }
                

                triadByNum[i] = triad;
                i++;
            }


            var resultList = triads.Where(x => x.Operation != "SAME").ToList();

            for (int j = 0; j < resultList.Count; j++)
                resultList[j].Number = j + 1;

            return resultList;
        }
    }
}
