using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPO_LR_Lib.Triads
{
    public class MutableTriad: TriadOperand, ITriad
    {

        public MutableTriad(int number, string operation, TriadOperand operand1, TriadOperand? operand2 = null)
        {
            Number = number;
            Operation = operation;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public int Number { get; set; }
        public string Operation { get; set; }
        public TriadOperand Operand1 { get; set; }
        public TriadOperand? Operand2 { get; set; }

        public bool IsUnaryOperation => Operand2 == null;

        public override bool Equals(object? obj)
        {
            if (obj is MutableTriad tr)
            {
                return tr.Operation == Operation && tr.Operand1.Equals(Operand1)
                    && (Operand2?.Equals(tr.Operand2) ?? tr.IsUnaryOperation);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Operation.GetHashCode() + Operand1.GetHashCode() + Operand2?.GetHashCode() ?? 0;
        }

        public Triad GetImmutableTriad(Dictionary<int, Triad> linkContainer)
        {
            TriadOperand op1 = Operand1 is MutableTriad tr1 ? linkContainer[tr1.Number] : (TriadOperand)Operand1.Clone();
            TriadOperand? op2 = Operand2 is MutableTriad tr2 ? linkContainer[tr2.Number] : Operand2?.Clone() as TriadOperand;

            var triad = new Triad(Number, Operation, op1, op2);
            linkContainer.Add(this.Number, triad);
            return triad;
        }
    }
}
