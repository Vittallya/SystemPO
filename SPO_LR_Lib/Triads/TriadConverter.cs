using System.Text;

namespace SPO_LR_Lib
{
    /// <summary>
    /// Класс принимает на вход знаки унарных и бинарных операций. 
    /// Первый метод GetTriads - получает на вход список корней дерева и возвращает на основе них список триад.  
    /// Второй метод GetNextTriad внутренний, рекурсивный.
    /// Третий метод простой печает список триад в виде текста
    /// </summary>
    public class TriadConverter
    {
        private readonly IEnumerable<string> unaryOpSigns;
        private readonly IEnumerable<string> binaryOpSigns;

        public TriadConverter(IEnumerable<string> unaryOpSigns, IEnumerable<string> binaryOpSigns)
        {
            this.unaryOpSigns = unaryOpSigns;
            this.binaryOpSigns = binaryOpSigns;
        }

        public IEnumerable<Triad> GetTriads(IEnumerable<TreeNode> rootNodes)
        {
            List<Triad> triads = new();
            rootNodes.ToList().ForEach(n => GetNextTriad(n, ref triads));

            return triads.Where(x => x.Operation != null);
        }

        /// <summary>
        /// Вывод триад в виде текста
        /// </summary>
        /// <param name="triads">список триад</param>
        /// <returns>поток текста</returns>
        public Stream PrintTriads(IEnumerable<Triad> triads)
        {
            StringBuilder stringBuilder = new StringBuilder();

            Dictionary<int, int> triadNumNum = new(triads.Count());

            int i = 1;

            foreach(Triad tr in triads)
            {
                string operation = tr.Operation;
                string op1 = tr.Operand1 is Triad op1Tr ? "^" + triadNumNum[op1Tr.Number] : tr.Operand1.OperandNameOrValue;

                if (tr.Operand2 != null)
                {
                    string op2 = tr.Operand2 is Triad op2Tr ? "^" + triadNumNum[op2Tr.Number] : tr.Operand2.OperandNameOrValue;
                    stringBuilder.AppendLine($"{i}) {operation}({op1},{op2})");

                }
                else
                {
                    stringBuilder.AppendLine($"{i}) {operation}({op1})");
                }
                triadNumNum.TryAdd(tr.Number, i++);
            }

            MemoryStream st = new(UTF8Encoding.UTF8.GetBytes(stringBuilder.ToString()));
            return st;
        }


        /// <summary>
        /// Метод принимает на вход какой-то узел дерева, а также ссылку на список триад. 
        /// Если этот узел является идентификатором, то мы возвращаем объект класса TriadOperand. 
        /// Если это не идентификатор, то есть операция, то мы смотрим на его потомки: 
        /// если операнд это терминальный символ, то мы напрямую создаем объект класса TriadOperand, 
        /// если же операнд является нетерминальным символом (E), то мы рекурсивно вызываем этот же метод, передав туда этот узел. 
        /// То же самое со 2 операндом, если операция является бинарной. 
        /// Далее полученную триаду добавляем в общий список триад.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="triads"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private TriadOperand GetNextTriad(TreeNode root, ref List<Triad> triads)
        {
            IEnumerable<TreeNode> childs = root.Childs;


            if (root.NodeType != TreeNodeType.Identifier)
            {
                IEnumerable<TreeNode> operands = childs.Where(x => x.SymbolType == SymbolType.NonTerminal || x.Symbol == "a");



                if (operands.Count() != 2 && root.NodeType == TreeNodeType.BinaryOp || operands.Count() != 1 && root.NodeType == TreeNodeType.UnaryOp)
                    throw new ArgumentException("Incorrect number of operands");

                TreeNode operandNode1 = operands.First();
                TriadOperand operandOne = operandNode1.SymbolType == SymbolType.Terminal ?
                    new TriadOperand(operandNode1.LexType, operandNode1.OriginLexem) : GetNextTriad(operandNode1, ref triads);


                Triad triad = default;

                if (root.NodeType == TreeNodeType.UnaryOp)
                {
                    string? operation = childs.FirstOrDefault(x => x.SymbolType == SymbolType.Terminal && unaryOpSigns.Contains(x.Symbol))?.Symbol;                    
                    triad = new Triad(triads.Count + 1, operation, operandOne);
                }
                else if (root.NodeType == TreeNodeType.BinaryOp)
                {
                    string operation = childs.First(x => x.SymbolType == SymbolType.Terminal && binaryOpSigns.Contains(x.Symbol)).Symbol;
                    TreeNode operandNode2 = operands.Last();
                    TriadOperand operandTwo = operandNode2.SymbolType == SymbolType.Terminal ?
                        new TriadOperand(operandNode2.LexType, operandNode2.OriginLexem) : GetNextTriad(operandNode2, ref triads);

                    triad = new Triad(triads.Count + 1, operation, operandOne, operandTwo);
                }
                triads.Add(triad);

                return triad;


            }

            var targetNode = root.SymbolType == SymbolType.NonTerminal ? root.Childs.First() : root;

            string lex = targetNode.OriginLexem;
            string lexType = targetNode.LexType;

            return new TriadOperand(lexType, lex);
        }
    }
}
