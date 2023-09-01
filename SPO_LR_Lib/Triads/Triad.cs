    using SPO_LR_Lib.Triads;

    namespace SPO_LR_Lib
    {
    /// <summary>
    /// Класс, который описывает триаду 
    /// Содержит такую информацию как: 
    /// Номер Триады, 
    /// Операция, 
    /// Операнд1 (либо просто TriadOperand, либо ссылка на результат другой триады). 
    /// Опернад2 - тоже самое, но его может и не быть, если операция унарная
    /// Флаг того, что операция унарная
    /// </summary>
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



            /// <summary>
            /// Методы для определения эквивалентности триад
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
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

        /// <summary>
        /// Метод, который из текущей не изменяемый триады, возвращает идентичную изменяемую  триаду .
        /// </summary>
        /// <param name="linkContainer">контейнер ссылок для правильного сохранения ссылок</param>
        /// <returns>изменяемая триада</returns>
        internal MutableTriad GetMutableTriad(Dictionary<int, MutableTriad> linkContainer)
            {
                TriadOperand op1 = Operand1 is Triad tr1 ? linkContainer[tr1.Number] : (TriadOperand)Operand1.Clone();
                TriadOperand? op2 = Operand2 is Triad tr2 ? linkContainer[tr2.Number] : Operand2?.Clone() as TriadOperand;
                var mutable = new MutableTriad(Number, Operation, op1, op2);

                linkContainer.Add(this.Number, mutable);

                return mutable;
            }
        }
    }
