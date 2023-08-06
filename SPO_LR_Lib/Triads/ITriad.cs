using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPO_LR_Lib.Triads
{
    public interface ITriad
    {
        public int Number { get; }

        public TriadOperand Operand1 { get; }
        public TriadOperand? Operand2 { get; }

        public string Operation { get; }

        public bool IsUnaryOperation => Operand2 == null;
    }
}
