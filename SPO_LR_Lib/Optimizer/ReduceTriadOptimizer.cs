using SPO_LR_Lib.Triads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPO_LR_Lib
{
    public record ReduceTriadOptimizer : ITriadOptimizer
    {

        public IEnumerable<Triad?> Optimize(IEnumerable<Triad?> triads)
        {
            Dictionary<int, MutableTriad> d1 = new();
            Dictionary<int, Triad> d2 = new();

            IEnumerable<MutableTriad> mutableTriads = triads.Select(y => y.GetMutableTriad(d1)).ToList();

            return ((ITriadOptimizer)this).Optimize(mutableTriads).Select(y => y.GetImmutableTriad(d2));
        }


        IEnumerable<MutableTriad> ITriadOptimizer.Optimize(IEnumerable<MutableTriad?> mutableTriads)
        {
            //название переменной - значение
            Dictionary<string, string> tTable = new();

            Dictionary<int, MutableTriad> cTriads = new();

            var executer = new TriadExecuter();

            int i = 1;

            foreach (MutableTriad triad in mutableTriads)
            {
                TriadOperand op1 = triad.Operand1;
                TriadOperand op2 = triad.Operand2;

                //шаг 1
                //если операнд 1 содержится в таблице T
                if (op1 is not MutableTriad && op1.LexType == "identifier" && tTable.TryGetValue(op1.OperandNameOrValue, out string val1))
                {
                    triad.Operand1 = new TriadOperand("constant", val1);
                }

                //если операнд 2 содержится в таблице T
                if (op2 != null && op2 is not MutableTriad && op2.LexType == "identifier" && tTable.TryGetValue(op2.OperandNameOrValue, out string val2))
                {
                    triad.Operand2 = new TriadOperand("constant", val2);
                }

                //шаг 2
                //если операнд 1 - ссылка на особую C-триаду
                if (triad.Operand1 is MutableTriad trOp1 && trOp1.Operation == "C")
                {
                    //операнд 1 есть константа первого операнда С-триады
                    triad.Operand1 = trOp1.Operand1;
                }

                //если операнд 2 - ссылка на особую C-триаду
                if (triad.Operand2 is MutableTriad trOp2 && trOp2.Operation == "C")
                {
                    //операнд 2 есть константа первого операнда С-триады
                    triad.Operand2 = trOp2.Operand1;
                }

                //шаг 3
                //если все операнды - константы
                if (IsConstant(triad))
                {
                    //мутация триады в особую С-триаду
                    string result = executer.Execute(triad);
                    triad.Operand1 = new TriadOperand("constant", result);
                    triad.Operand2 = new TriadOperand("constant", "0");
                    triad.Operation = "C";
                }

                //шаг 4
                //если операция присваивания
                if (triad.Operation == ":=")
                {
                    if (triad.Operand2 == null)
                        throw new ArgumentException();

                    if (triad.Operand2 is not MutableTriad)
                    {

                        if (triad.Operand2.LexType == "constant")
                        {
                            if (!tTable.TryAdd(triad.Operand1.OperandNameOrValue, triad.Operand2.OperandNameOrValue))
                                tTable[triad.Operand1.OperandNameOrValue] = triad.Operand2.OperandNameOrValue;
                        }
                        else if (triad.Operand1.OperandNameOrValue != null && tTable.ContainsKey(triad.Operand1.OperandNameOrValue))
                        {
                            tTable.Remove(triad.Operand1.OperandNameOrValue);
                        }
                    }


                }

                i++;
            }
            var resultList = mutableTriads.Where(x => x.Operation != "C").ToList();

            for (int j = 1; j <= resultList.Count; j++)
                resultList[j - 1].Number = j;

            return resultList;
        }


        private static bool IsConstant(ITriad triad)
            => triad.Operand1.LexType == "constant"
            && (triad.IsUnaryOperation || triad.Operand2.LexType == "constant");
    }
}
