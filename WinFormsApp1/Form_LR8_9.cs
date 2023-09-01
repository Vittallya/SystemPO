using SPO_LR_Lib;
using SPO_LR_Lib.Analyzers;
using SPO_LR_Lib.Calculator;
using System.Text.Json;
using System.Text.RegularExpressions;
using WinFormsApp1.Models;

namespace WinFormsApp1
{
    public partial class Form_LR8_9 : Form
    {
        //прочитать матрицу и правила из json-файла
        MatrixJsonLr8 data;
        LeksemFinder leksemFinder;

        //регулярное выражение для лексического анализатора
        Regex regex = new Regex(@"

(
    (?'keyWord'var)\s+ (\s* ((?'a'\w+)(\s*(?'comma',)\s*(?'a'\w+))*) \s* (?'twoP':) \s* ((?'c'byte|real) |

        (((?'keyWord'union)\s+((\s*(?'a'\w+)((?'comma',)\s*(?'a'\w+))*\s*
        (?'twoP':)\s*((?'c'byte|real)|(?'t'\w+)))(?'separator';))*)\s+(?'keyWord'end)) | 
        (?'t'\w+))
        (?'separator';))*
) | 

(
    (?'keyWord'type)\s+ (\s* (?'t'\w+)(\s*(?'comma',)\s*(?'t'\w+))* \s* 
        (?'eqSign'=) \s* ((?'c'byte|real) |
        (((?'keyWord'union)\s+((\s*(?'a'\w+)((?'comma',)\s*(?'a'\w+))*\s*
        (?'twoP':)\s*((?'c'byte|real)|(?'t'\w+)))(?'separator';))*)\s+(?'keyWord'end)))
        (?'separator';))*
) |
\*\*.*\*\*",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);


        private readonly Dictionary<string, string> lexems = new()
        {
            {"keyWord", "" },
            {"c", "" },
            {"a", "" },
            {"separator", "" },
            {"comma", "" },
            {"twoP", "" },
            {"d", "" },
            {"eqSign", "" },
            {"t", "" }
        };

        public Form_LR8_9()
        {
            InitializeComponent();

            //загрузка матрицы операторного предш. и списка правил
            data = JsonSerializer.Deserialize<MatrixJsonLr8?>(Properties.Resources.dataLR8New);
            leksemFinder = new LeksemFinder(regex, lexems);
            //запись в текстовое поле содержимого файла
            richTextBox1.Text = File.ReadAllText("textLR8_9.txt");
        }

        /// <summary>
        /// При нажатии на кнопку - "выполнить анализ"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentException"></exception>
        private void button1_Click(object sender, EventArgs e)
        {

            //поиск лексем на основе регулярного выражения
            var res = leksemFinder.TryGetLeksems(richTextBox1.Text,
                out IEnumerable<Group> errors,
                out IEnumerable<(string lex, string type)> lexems);

            //если лексемы найдены удачно
            if (res)
            {

                //создание синтаксического анализатора
                var syntaxAnalyzer = new SyntaxAnalyzer(data, 
                    //аргумент - лямбда-функция для разрешения противоречий, если будет множество одинаковых правил
                    //например : ( E -> K : c и V -> K:c - выбрать E или выбрать V)
                    (stack, rule, letters) =>
                {
                    
                    if(letters.Count() == 2 && letters.All(x => x.Item1 == "E" || x.Item1 == "V"))
                    {
                        int unionCount = stack.Count(x => x.value == "union");
                        int endCount = stack.Count(x => x.value == "end");
                        
                        //если лексем "union" больше, чем "end", то выбирается правило E
                        if(unionCount > endCount)
                            return letters.First(x => x.Item1 == "E").Item2;

                        //в ином случае - правило V
                        return letters.First(x => x.Item1 == "V").Item2;
                    }
                    throw new ArgumentException();
                });

                //выполнение синтаксического анализа на основе лексем и получением дерева выражений
                var result = syntaxAnalyzer.TryAnalyze(lexems, 
                    //аргумент - лямбда-функция для форматирования лексем
                    ((string lex, string type) lexx) =>
                    {
                        return lexx.type switch
                        {
                            "a" or "c" or "t" => lexx.type,
                            _ => lexx.lex,
                        };
                    },
                    //начальный нетерминальный символ
                    "S",
                    out _, 
                    //выходной параметр - дерево выражений
                    out IEnumerable<SPO_LR_Lib.TreeNode> treeNodes);

                //если синтаксич. анализ - успешный
                if (result)
                {
                    //вывод графического дерева на экран
                    new FormTree().SetupTreeView(treeNodes.First()).Show(this);

                    //выполнение семантического анализа
                    if (new SemantickAnalyzer().TryAnalyze(treeNodes.First(), out IEnumerable<string> errorMessages))
                    {
                        //если семантический анализ успешен - подсчет памяти
                        var calc = new MemoryCalc(4, treeNodes.First(), ("byte", 1), ("real", 6));

                        //получение общего требуемого объема памяти
                        int mem1 = calc.GetTotalMemory();
                        //получение общего требуемого объема памяти с выравниванием
                        int mem2 = calc.GetTotalMemoryWithMultiplicity();

                        //вывод этих значений

                        MessageBox.Show("Требуемый объем памяти: " + mem1 + 
                            "\nТребуемый объем памяти с выравниванием: " + mem2,
                            "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        //если неуспешный семант. анализ - вывод семантических ошибок
                        MessageBox.Show("-" + string.Join("\n-", errorMessages),
                            "Ошибки семантического анализа",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }
    }
}
