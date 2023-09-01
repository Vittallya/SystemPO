namespace SPO_LR_Lib.Calculator
{
    /// <summary>
    /// Класс для подсчета объема памяти для дерева выражений с учетом кратности
    /// </summary>
    public class MemoryCalc
    {
        private readonly int multiplicity;
        private readonly Dictionary<string, int> scalarTypes;
        private readonly TreeNode treeNode;

        private Dictionary<string, int> tMemory;
        private Dictionary<string, int> varMemory;


        /// <summary>
        /// Конструктор калькулятора памяти
        /// </summary>
        /// <param name="multiplicity">кратность выравнивания</param>
        /// <param name="treeNode">корневой узел дерева выражений</param>
        /// <param name="scalarTypesMemory">скалярные типы данных и их объем</param>
        public MemoryCalc(int multiplicity,
                          TreeNode treeNode,
                          params (string dataType, int memory)[] scalarTypesMemory)
        {
            this.multiplicity = multiplicity;
            this.scalarTypes = scalarTypesMemory.ToDictionary(x => x.dataType, y => y.memory);
            this.treeNode = treeNode;
            Calculate();

        }

        private void Calculate()
        {
            var allTDeclares = treeNode.
                GetAllNonTerminals("T");

            //заносим в таблицу те объявления типов, которые являются скалярными ( T -> t = c)
            tMemory = allTDeclares.
                Where(x => x.Childs.Last().Symbol == "c").
                ToDictionary(x => x.Childs.First().OriginLexem, y => scalarTypes[y.Childs.Last().OriginLexem]);

            //определяем нетерминалы, в которых для присвоения типа используется цепочка union ... end (T -> t = D, D -> union F end...)
            var tUnions = allTDeclares.
                Where(x => x.Childs.Last().Symbol == "D");

            //среди последних определеяем такие, у которых для дочерних нетерминалов E -> K:c | K : t, t - уже известный тип
            var tUnionsDefined = tUnions.Where(x => x.
                GetAllNonTerminals("E").
                All(y => y.Childs.Last().Symbol == "c" ||
                    y.Childs.Last().Symbol == "t" && tMemory.ContainsKey(y.Childs.Last().OriginLexem)));

            //расчитываем для последних отобранных типов объем памяти, т.е. максимальный объем типа данных для выражений в составе union
            var t1 = tUnionsDefined.
                ToDictionary(x => x.Childs.First().OriginLexem,
                    y => y.GetAllNonTerminals("E").Max(GetTotalMemoryInENonTerminal));

            //обновляем таблицу новыми значениеями для типов данных
            tMemory = tMemory.Union(t1).ToDictionary(x => x.Key, y => y.Value);


            //определяем оставшиеся типы данных, для которых еще не подсчитан объем памяти
            var tUnionsNotDefined = tUnions.Where(x => !tMemory.ContainsKey( x.Childs.First().OriginLexem));


            //расчитываем для последних отобранных типов объем памяти, т.е. максимальный объем типа данных для выражений в составе union
            var t2 = tUnionsNotDefined.
                ToDictionary(x => x.Childs.First().OriginLexem,
                    y => y.GetAllNonTerminals("E").Max(GetTotalMemoryInENonTerminal));

            //получение итоговой таблицы: тип данных - требуемый объем памяти
            tMemory = tMemory.Union(t2).ToDictionary(x => x.Key, y => y.Value);

            //на этом этапе все типы данных определены

            //подсчет переменных, которые имеют тип t или c (V -> K:t | K:c)

            var nonTerminalR = treeNode.Childs.Last();

            var nonTerminalsV = nonTerminalR.
                GetAllNonTerminals("V");

            //получение таблицы (название переменной - объем памяти для нее) для простых переменных ( a : t или a : c)
            var a = nonTerminalsV.
                Where(x => x.Childs.Last().Symbol != "D").
                SelectMany(x => x.GetAllNonTerminals("K").
                                ToDictionary(y => y.Childs.Last().OriginLexem,
                                                z => x.Childs.Last().Symbol switch
                                                {
                                                    "c" => scalarTypes[x.Childs.Last().OriginLexem],
                                                    "t" => tMemory[x.Childs.Last().OriginLexem],
                                                    _ => throw new NotImplementedException()
                                                })).
                ToDictionary(x => x.Key, y => y.Value);


            //получение таблицы (название переменной - объем памяти для нее) для составных переменных (с использованием union ... end)
            var b = nonTerminalsV.
                    Where(x => x.Childs.Last().Symbol == "D").
                    SelectMany(x => x.Childs.First().GetAllNonTerminals("K").
                                        ToDictionary(y => y.Childs.Last().OriginLexem, 
                                                        z => x.Childs.Last().GetAllNonTerminals("E").
                                                            Max(GetTotalMemoryInENonTerminal))).
                    ToDictionary(x => x.Key, y => y.Value);

            //на выходе получается итоговая таблица из колонок: название переменной - требуемый объем памяти
            varMemory = a.Union(b).ToDictionary(x => x.Key, y => y.Value);

            //внутренний метод для подсчета всего объема памяти для выражения E (E -> K:c | K:t, K -> a | K,a)
            int GetTotalMemoryInENonTerminal(TreeNode node)
            {
                TreeNode lastChildNode = node.Childs.Last();
                int count = node.GetAllNonTerminals("K").Count();

                return lastChildNode.Symbol switch
                {
                    "c" => scalarTypes[lastChildNode.OriginLexem] * count,
                    "t" => tMemory[lastChildNode.OriginLexem] * count,
                    _ => throw new ArgumentException()
                };
            }

        }

        /// <summary>
        /// Подсчет суммарного объема памяти для всех переменных
        /// </summary>
        /// <returns></returns>
        public int GetTotalMemory()
        {
            return varMemory.Sum(x => x.Value);
        }

        /// <summary>
        /// Подсчет суммарного объема памяти для всех переменных с выравниванием
        /// </summary>
        /// <returns></returns>
        public int GetTotalMemoryWithMultiplicity()
        {
            return varMemory.Sum(x => Multiplicty(x.Value));
        }

        /// <summary>
        /// Выравнивание значения под заданную кратность
        /// </summary>
        /// <param name="val">значение</param>
        /// <returns>выровненное значение</returns>
        private int Multiplicty(int val) => val % multiplicity == 0 ? val : val + multiplicity - (val % multiplicity);

    }
}
