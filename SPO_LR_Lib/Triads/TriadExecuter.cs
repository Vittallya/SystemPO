using SPO_LR_Lib.Triads;

namespace SPO_LR_Lib
{
    /// <summary>
    /// Класс, который исполняет операцию триады и возвращает результат
    /// в качестве операндов триады должны быть константы
    /// </summary>
    internal class TriadExecuter
    {
        public string Execute(ITriad triad)
        {
            string op1 = triad.Operand1.OperandNameOrValue;
            string operation = triad.Operation;
            string? op2 = triad.Operand2?.OperandNameOrValue;

            if (op2 == null)
            {
                return ExecuteUnary(op1, operation);
            }

            uint op1U = Convert.ToUInt32(op1, 2);
            uint op2U = Convert.ToUInt32(op2, 2);

            switch(operation)
            {
                case "or": return Convert.ToString(op1U | op2U, 2);
                case "xor": return Convert.ToString(op1U ^ op2U, 2);
                case "and": return Convert.ToString(op1U & op2U, 2);
            }

            throw new ArgumentException();
        }

        public string ExecuteUnary(string operandValue, string operation)
        {
            if(operation == "not")
            {
                uint op = Convert.ToUInt32(operandValue, 2);
                uint res = ~op;
                return Convert.ToString(res, 2);
            }
            throw new ArgumentException();
        }

    }
}
