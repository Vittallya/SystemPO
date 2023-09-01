namespace SPO_LR_Lib
{
    /// <summary>
    /// Класс, который представляет собой операнд триады
    /// </summary>
    public class TriadOperand: ICloneable
    {
        /// <summary>
        /// из списка лекскем (identifier, constant)
        /// </summary>
        public string? LexType { get; }
        public string? OperandNameOrValue { get; }



        public TriadOperand(string? lexType = null, string? operandNameOrValue = null)
        {
            LexType = lexType;
            OperandNameOrValue = operandNameOrValue;
        }

        public override bool Equals(object? obj)
        {
            if(obj is TriadOperand op && obj is not Triad)
            {
                return op.LexType == LexType && op.OperandNameOrValue == OperandNameOrValue;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return LexType?.GetHashCode() ?? 0 + OperandNameOrValue?.GetHashCode() ?? 0;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
