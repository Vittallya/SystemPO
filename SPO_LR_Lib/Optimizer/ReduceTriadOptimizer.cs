using SPO_LR_Lib.Triads;

namespace SPO_LR_Lib
{
    public record ReduceTriadOptimizer : ITriadOptimizer
    {

        // Метод для оптимизации списка объектов Triad
        public IEnumerable<Triad?> Optimize(IEnumerable<Triad?> triads)
        {
            // Словарь для хранения объектов MutableTriad с номерами в качестве ключей
            Dictionary<int, MutableTriad> d1 = new();
            // Словарь для хранения объектов Triad с номерами в качестве ключей
            Dictionary<int, Triad> d2 = new();

            // Преобразуем список объектов Triad в список объектов MutableTriad
            IEnumerable<MutableTriad> mutableTriads = triads.Select(y => y.GetMutableTriad(d1)).ToList();

            // Вызываем второй метод для оптимизации списка MutableTriad, затем преобразуем результат обратно в объекты Triad
            return ((ITriadOptimizer)this).Optimize(mutableTriads).Select(y => y.GetImmutableTriad(d2));
        }


        // Второй метод для оптимизации списка объектов MutableTriad
        IEnumerable<MutableTriad> ITriadOptimizer.Optimize(IEnumerable<MutableTriad?> mutableTriads)
        {
            // Словарь для хранения пар "название переменной - значение"
            Dictionary<string, string> tTable = new();

            // Словарь для хранения объектов MutableTriad с номерами в качестве ключей
            Dictionary<int, MutableTriad> cTriads = new();

            // Объект для выполнения операций над триадами
            var executer = new TriadExecuter();

            int i = 1;

            // Просматриваем каждый объект MutableTriad в списке
            foreach (MutableTriad triad in mutableTriads)
            {
                TriadOperand op1 = triad.Operand1;
                TriadOperand op2 = triad.Operand2;

                // Шаг 1: Если Operand1 содержится в таблице T, заменяем его на соответствующее значение константы
                if (triad.Operation != ":=" && op1 is not MutableTriad && op1.LexType == "identifier" && tTable.TryGetValue(op1.OperandNameOrValue, out string val1))
                {
                    triad.Operand1 = new TriadOperand("constant", val1);
                }

                // Если Operand2 содержится в таблице T, заменяем его на соответствующее значение константы
                if (op2 != null && op2 is not MutableTriad && op2.LexType == "identifier" && tTable.TryGetValue(op2.OperandNameOrValue, out string val2))
                {
                    triad.Operand2 = new TriadOperand("constant", val2);
                }

                // Шаг 2: Если Operand1 - ссылка на особую C-триаду, заменяем его на значение константы из первого операнда С-триады
                if (triad.Operand1 is MutableTriad trOp1 && trOp1.Operation == "C")
                {
                    triad.Operand1 = trOp1.Operand1;
                }

                // Если Operand2 - ссылка на особую C-триаду, заменяем его на значение константы из первого операнда С-триады
                if (triad.Operand2 is MutableTriad trOp2 && trOp2.Operation == "C")
                {
                    triad.Operand2 = trOp2.Operand1;
                }

                // Шаг 3: Если все операнды триады - константы, заменяем триаду на особую С-триаду
                if (IsConstant(triad))
                {
                    // Выполняем триаду и заменяем её на особую С-триаду с результатом выполнения
                    string result = executer.Execute(triad);
                    triad.Operand1 = new TriadOperand("constant", result);
                    triad.Operand2 = new TriadOperand("constant", "0");
                    triad.Operation = "C";
                }

                // Шаг 4: Если триада - присваивание типа A:=B
                if (triad.Operation == ":=")
                {
                    if (triad.Operand2 == null)
                        throw new ArgumentException();

                    if (triad.Operand2 is not MutableTriad)
                    {
                        // Если B - константа, то заносим значение B в таблицу T для переменной A (если там уже было старое значение, оно заменяется)
                        if (triad.Operand2.LexType == "constant")
                        {
                            if (!tTable.TryAdd(triad.Operand1.OperandNameOrValue, triad.Operand2.OperandNameOrValue))
                                tTable[triad.Operand1.OperandNameOrValue] = triad.Operand2.OperandNameOrValue;
                        }
                        // Если B - не константа, то исключаем переменную A из таблицы T, если она там присутствует
                        else if (triad.Operand1.OperandNameOrValue != null && tTable.ContainsKey(triad.Operand1.OperandNameOrValue))
                        {
                            tTable.Remove(triad.Operand1.OperandNameOrValue);
                        }
                    }
                }

                i++;
            }

            // Создаём список для хранения оптимизированных объектов MutableTriad
            var resultList = mutableTriads.Where(x => x.Operation != "C").ToList();

            // Сбрасываем номера MutableTriad, чтобы они были последовательными, начиная с 1
            for (int j = 1; j <= resultList.Count; j++)
                resultList[j - 1].Number = j;

            return resultList;
        }

        // Метод для проверки, являются ли все операнды триады константами
        private static bool IsConstant(ITriad triad)
            => triad.Operand1.LexType == "constant"
            && (triad.IsUnaryOperation || triad.Operand2.LexType == "constant");
    }
}
