using SPO_LR_Lib.Triads;

namespace SPO_LR_Lib
{
    public class DeleteTriadOptimizer : ITriadOptimizer
    {
        // Метод для оптимизации списка объектов Triad
        public IEnumerable<Triad?> Optimize(IEnumerable<Triad?> triads)
        {
            // Словарь для хранения объектов MutableTriad с номерами в качестве ключей
            var d = new Dictionary<int, MutableTriad>();
            // Словарь для хранения объектов Triad с номерами в качестве ключей
            var d1 = new Dictionary<int, Triad>();

            // Преобразуем список объектов Triad в список объектов MutableTriad
            IEnumerable<MutableTriad> mutableTriads = triads.Select(y => y.GetMutableTriad(d));

            // Вызываем второй метод для оптимизации списка MutableTriad, затем преобразуем результат обратно в объекты Triad
            return ((ITriadOptimizer)this).Optimize(mutableTriads).Select(y => y.GetImmutableTriad(d1));
        }

        // Второй метод для оптимизации списка объектов MutableTriad
        IEnumerable<MutableTriad> ITriadOptimizer.Optimize(IEnumerable<MutableTriad> triads)
        {
            int i = 1;

            // Словарь для отслеживания количества зависимостей для каждого операнда
            Dictionary<TriadOperand, int> depsCountByOperands = new();
            // Словарь для хранения объектов MutableTriad с номерами в качестве ключей
            Dictionary<int, MutableTriad> triadByNum = new();

            // Словарь для отслеживания количества зависимостей для каждой триады
            Dictionary<int, int> depsCountByTriadNum = new();

            // Проходимся по каждой триаде в списке
            foreach (MutableTriad triad in triads)
            {
                // Шаг 1: Если Operand1 является MutableTriad типа "SAME", заменяем его на ссылку на соответствующий MutableTriad
                if (triad.Operand1 is MutableTriad tr1 && tr1.Operation == "SAME")
                {
                    triad.Operand1 = tr1.Operand1;
                }

                // Шаг 2: Если Operand2 является MutableTriad типа "SAME", заменяем его на ссылку на соответствующий MutableTriad
                if (triad.Operand2 is MutableTriad tr2 && tr2.Operation == "SAME")
                {
                    triad.Operand2 = tr2.Operand1;
                }

                int a = 0;
                int b = 0;

                // Шаг 3: Вычисляем количество зависимостей для Operand1
                if (triad.Operand1 is MutableTriad tr11)
                {
                    a = depsCountByTriadNum[tr11.Number];
                }
                else if (triad.Operand1.LexType == "identifier")
                {
                    if (!depsCountByOperands.TryGetValue(triad.Operand1, out a))
                    {
                        depsCountByOperands.Add(triad.Operand1, 0);
                    }
                }

                // Шаг 4: Вычисляем количество зависимостей для Operand2
                if (triad.Operand2 is MutableTriad tr22)
                {
                    b = depsCountByTriadNum[tr22.Number];
                }
                else if (triad.Operand2 != null && triad.Operand2.LexType == "identifier")
                {
                    if (!depsCountByOperands.TryGetValue(triad.Operand2, out b))
                    {
                        depsCountByOperands.Add(triad.Operand2, b);
                    }
                }

                // Вычисляем зависимость текущей триды на основе зависимостей её операндов
                int depi = Math.Max(a, b) + 1;

                // Шаг 5: Проверяем, существует ли идентичная триада с такой же зависимостью перед текущей Triad
                // Если находим, заменяем текущую триаду типа "SAME"
                MutableTriad? sameFounded = null;
                if (depsCountByTriadNum.Any(x => x.Value == depi && (sameFounded = triadByNum[x.Key]).Equals(triad)))
                {
                    triad.Operation = "SAME";
                    triad.Operand1 = sameFounded;
                    triad.Operand2 = new TriadOperand("constant", "0");
                }

                // Сохраняем количество зависимостей для текущей триады
                depsCountByTriadNum.Add(triad.Number, depi);

                // Шаг 6: Если текущая триада является присвоением, вычисляем зависимость соответствующей переменной
                if (triad.Operation == ":=" && !depsCountByOperands.TryAdd(triad.Operand1, i))
                {
                    depsCountByOperands[triad.Operand1] = i;
                }

                // Сохраняем текущую Triad в словаре, используя её номер в качестве ключа
                triadByNum[i] = triad;
                i++;
            }

            // Создаём список для хранения оптимизированных объектов MutableTriad
            var resultList = triads.Where(x => x.Operation != "SAME").ToList();

            // Сбрасываем номера Triad, чтобы они были последовательными, начиная с 1
            for (int j = 0; j < resultList.Count; j++)
                resultList[j].Number = j + 1;

            return resultList;
        }
    }
}
