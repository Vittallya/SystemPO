namespace SPO_LR_Lib.Analyzers
{
    public class SemantickAnalyzer
    {
        public SemantickAnalyzer()
        {
            
        }

        /// <summary>
        /// Метод принимает на вход корень дерева выржений S
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool TryAnalyze(TreeNode treeNode, out IEnumerable<string> messages)
        {
            //создание списка ошибок
            List<string> messagesList = new List<string>();
            messages = messagesList;

            //получение всех выражений с нетерминалом T в левой части из дерева ( T -> t = c | t = D)
            var allTDeclares = treeNode.
                GetAllNonTerminals("T");

            //из предыдущих выражений получаем лексемы t и помещаем их в список - список типов, объявленных в блоке type
            var allTDeclaresList = allTDeclares.
                Select(x => x.Childs.First().OriginLexem);

            //получение всех выражений с нетерминалом V и E в левой части из дерева ( V -> K:t | K:c | K:D), (E -> K:c | K : t)
            var allUsesTypes = treeNode.GetAllNonTerminals("V").
                Union(treeNode.GetAllNonTerminals("E"));

            //получение всех лексем t из указанных выражений
            var b = allUsesTypes.Where(x => x.Childs.Last().SymbolType == SymbolType.Terminal && x.Childs.Last().Symbol == "t").
                Select(x => x.Childs.Last().OriginLexem);

            //вычитание из множества всех используемых лексем t множества объявленных лексем в блоке type
            var usedUndeclaredTypes = b.Except(allTDeclaresList);

            //если вычитание выдает непустое множество - ошибка
            if(usedUndeclaredTypes.Any())
            {
                //перечисление необъявленных в блоке type типов данных
                messagesList.Add("Использованы необъявленные типы данных: " + string.Join(", ", usedUndeclaredTypes));                
            }


            //отбор повторяющихся лекскем с типами
            var nonUniqTypes = allTDeclaresList.GroupBy(x => x).Where(x => x.Count() > 1);

            //если есть повторяющиеся лекскемы с типами данных - ошибка
            if(nonUniqTypes.Any())
            {
                messagesList.Add("В блоке \"type\" следующие типы данных объявлены несколько раз: " + 
                    string.Join(", ", nonUniqTypes.Select(x => x.Key)));
            }

            //получение всех выражений  T -> t = D
            var tUnions = allTDeclares.Where(x => x.Childs.Last().Symbol == "D");

            //попытка найти такие выражения, которые имеют закольцованность, т.е. 
            // tt = union abc : tt; ... end;  - здесь тип tt и слева и внутри выражения
            var loopDeclares = tUnions.
                Where(x => x.GetAllNonTerminals("E").
                            Where(y => y.Childs.Last().Symbol == "t").
                            Any(y => y.Childs.Last().OriginLexem == x.Childs.First().OriginLexem));

            //если такие выражения находятся - ошибка
            if(loopDeclares.Any())
            {
                messagesList.Add("В блоке \"type\" используются циклические объявления: " + 
                    string.Join(", ", loopDeclares.Select(x => x.Childs.First().OriginLexem)));
            }

            //возврат реузльтата анализа
            return messagesList.Count == 0;
        }

    }
}
