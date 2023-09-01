using SPO_LR_Lib.Analyzers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace SPO_LR_Lib
{
    //тип символа (терминал или нетерминал)
    public enum SymbolType
    {
        Terminal, NonTerminal
    }

    //класс синтаксического анализатора
    public class SyntaxAnalyzer
    {
        private readonly TreeCreator treeCreator;
        private readonly IData data;
        private readonly Func<Stack<(string value, SymbolType, string, string lexType)>, IEnumerable<string>, IEnumerable<(string, int)>, int>? resolver;


        //получение матрицы и правил через конструктор
        public SyntaxAnalyzer(IData data, Func<Stack<(string value, SymbolType, string, string lexType)>, IEnumerable<string>, IEnumerable<(string, int)>, int>? resolver = null) 
        {
            this.treeCreator = new TreeCreator();
            this.data = data;
            this.resolver = resolver;
        }


        /// <summary>
        /// Метод для проведения синтиаксического анализа лексем
        /// </summary>
        /// <param name="lekems">входной параметр - список лексем и их тип</param>
        /// <param name="errors">вероятные ошибки при анализе</param>
        /// <returns>успех/не успех анализа</returns>
        public bool TryAnalyze(IEnumerable<(string name, string type)> lekems, 
            Func<(string name, string type), string> replacer,
            string startLetter,
            out IEnumerable<string> errors, out IEnumerable<TreeNode> rootNodes)
        {
            List<TreeNode> treeNodes = new();
            rootNodes = treeNodes;

            List<string> errorList = new();
            errors = errorList;

            //создание списка лексем, помещая в него конечный символ
            Stack<(string name, string type)> leksemsList = new(lekems.Append(("$", "")).Reverse());
            //создание стэка
            Stack<(string value, SymbolType type, string, string lexType)> stack = new();
            //помещение в стэк начального символа
            stack.Push(("s", SymbolType.Terminal, "", ""));


            //цепочка вывода
            List<Node> nodes = new();

            //конечное состояние стэка
            Stack<(string, SymbolType, string, string)> finalStackState = new(new []{("s", SymbolType.Terminal, "", ""), ("E", SymbolType.NonTerminal, "", "") });

            while(true)
            {
                //чтение очередной лексемы
                (string lex, string type) = leksemsList.Peek();
                string origin = lex;

                lex = replacer((lex, type));

                //если это константа или идентификатор
                //if (type == "identifier" || type == "constant")
                    //lex = "a"; //замена на символ a

                //получение первого терминального символа из стэка
                (string stackTerminal, SymbolType _, string _, string _) = stack.FirstOrDefault(x => x.type == SymbolType.Terminal);

                //если символ не найден, ошибка
                if(stackTerminal == null)
                {
                    errorList.Add("Терминальный символ не был найден в стеке");
                    return false;
                }

                //проверка наличия отношения
                if(!data.HasRelationBetween(stackTerminal, lex, out string? relation))
                {
                    //если нет отношения, ошибка
                    errorList.Add($"Нет отношения предшествования между символами '{stackTerminal}' и '{lex}'");
                    return false;
                }

                //проверка отношения между символами
                switch(relation)
                {
                    case "<" or "=":
                        //выполнение переноса
                        stack.Push((lex, SymbolType.Terminal, origin, type));
                        leksemsList.Pop();
                        continue;
                    case ">":
                        //выполнение свертки
                        Debug.Print("Стэк: {" + string.Join("} {", stack.Select(x => x.value)) + "}");
                        Debug.Print("Список лексем: {" + string.Join("} {", leksemsList.Select(x => x.name)) + "}");


                        //int ruleNum = Fold(stack, out IEnumerable<(string, SymbolType, string, string)>? replacedSymbols);
                        int ruleNum = Reduce(stack, out IEnumerable<(string, SymbolType, string, string)> replacedSymbols);
                        if (ruleNum == -1)
                        
                        {
                            //если правило для свертки не найдено - ошибка
                            errorList.Add("Правило не найдено");
                            return false;
                        }


                        //if (ruleNum != 5)
                        //{
                        //    //пропускаем скобочки
                        nodes.Add(new Node(replacedSymbols.Reverse(), ruleNum));
                        //}

                        break;

                }

                //проверка стэка с его финальным состоянием, а также проверка, что в списке лексем остался только конечный символ


                if (stack.Last().value == "s" && stack.SkipLast(1).All(x => x.value.All(char.IsUpper)))
                {
                    var treeNode = treeCreator.GetTreeView(nodes.Reverse<Node>(), startLetter);
                    treeNodes.Add(treeNode);
                    nodes.Clear();


                    Debug.Print("Стэк: {" + string.Join("} ; {", stack.Select(x => x.value)) + "}");
                    Debug.Print("Список лексем: {" + string.Join("} ; {", leksemsList.Select(x => x.name)) + "}");

                    if (leksemsList.Any() && leksemsList.Peek().name == "$")
                        return true;
                }

            }
            
        }





        //функция свертки
        private int Fold(Stack<(string value, SymbolType, string, string lexType)> stack,
                         out IEnumerable<(string, SymbolType, string, string)>? replacedSymbols)
        {
            //получение массива строчек из стэка
            var stackStr = stack.Select(x => x.value).ToArray();
            replacedSymbols = null;

            int i = 0;


            foreach (var rule in data.GetRules())
            {
                //взять очередное правило в обратном состоянии
                var currentRule = rule.rules.Reverse<string>();
                int c = currentRule.Count();

                //если это правило по длине больше, чем длина стэка, пропуск правила
                if (stackStr.Length <= c)
                    continue;

                replacedSymbols = default;

                var list = new List<(string, SymbolType, string, string)>();
                replacedSymbols = list;

                //если правило подходит, т.е. верхушка стэка ему соотвествует
                if (Enumerable.SequenceEqual(currentRule, stackStr[..c]))
                {
                    //замена верхушки стэка на нетерминал E
                    int j = 0;
                    while (j++ < c)
                        list.Add(stack.Pop());
                    stack.Push((rule.NonTerminalToReplace, SymbolType.NonTerminal, "", ""));
                    //возврат номера правила
                    return i;
                }
                i++;
            }
            //правило не найдено
            return -1;
        }

        //функция свертки
        private int Reduce(Stack<(string value, SymbolType, string, string lexType)> stack,
                         out IEnumerable<(string, SymbolType, string, string)> replacedSymbols)
        {
            //получение массива строчек из стэка
            var stackStr = stack.Select(x => x.value).ToArray();

            var list = new List<(string, SymbolType, string, string)>();
            replacedSymbols = list;

            var rules = data.GetRules();


            int maxCountToReplace = 0;

            rules = rules.Where(x => x.rules.Count <= stackStr.Length && Enumerable.SequenceEqual(stackStr[..x.rules.Count], x.rules.Reverse<string>()));
            maxCountToReplace = rules.Max(x => x.rules.Count);

            rules = rules.
                OrderByDescending(x => x.rules.Count).
                TakeWhile(x => x.rules.Count == maxCountToReplace).
                ToList();


            int count = rules.Count();

            if (count > 0)
            {
                if(count > 1 && resolver is null)
                    throw new ArgumentException("rules conflict without resolver");

                int ruleNum = count == 1 ? rules.First().ruleNum : 
                    resolver.Invoke(stack, rules.First().rules, rules.Select(x => (x.NonTerminalToReplace, x.ruleNum)));

                var rule = rules.Single(x => x.ruleNum == ruleNum);

                string nonTerminal = rule.NonTerminalToReplace;

                int c = rule.rules.Count;

                int j = 0;
                while (j++ < c)
                    list.Add(stack.Pop());
                stack.Push((nonTerminal, SymbolType.NonTerminal, "", ""));


                return ruleNum;
            }


            //правило не найдено
            return -1;
        }
    }
}
