using SPO_LR_Lib.Triads;

namespace SPO_LR_Lib
{
    public class Triad: TriadOperand, ITriad
    {
        public Triad(int number, string operation, TriadOperand operand1, TriadOperand? operand2 = null)
        {
            Number = number;
            Operation = operation;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public int Number { get; }
        public string Operation { get; }
        public TriadOperand Operand1 { get; }
        public TriadOperand? Operand2 { get; }
        public bool IsUnaryOperation => Operand2 == null;

        public override bool Equals(object? obj)
        {
            if(obj is Triad tr)
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

        internal MutableTriad GetMutableTriad(Dictionary<Triad, MutableTriad> linkContainer)
        {
            TriadOperand op1 = Operand1 is Triad tr1 ? linkContainer[tr1] : (TriadOperand)Operand1.Clone();
            TriadOperand? op2 = Operand2 is Triad tr2 ? linkContainer[tr2] : Operand2?.Clone() as TriadOperand;
            var mutable = new MutableTriad(Number, Operation, op1, op2);

            linkContainer.Add(this, mutable);

            return mutable;
        }
    }
}
