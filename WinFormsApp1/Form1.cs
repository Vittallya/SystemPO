using System.Text.Json;
using System.Text.RegularExpressions;
using SPO_LR_Lib;
using SPO_LR_Lib.Optimizer;
using WinFormsApp1.Models;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            richTextBox1.Text = File.ReadAllText("test.txt");
        }

        //Нажатие на кнопку "Показать лексемы"
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForLeksems();
        }

        private void CheckForLeksems()
        {
            //получение списка лексем (код приведен в предыдущей лабораторной работе)
            var res = LeksemFinder.
                Singleton.
                TryGetLeksems(richTextBox1.Text, out IEnumerable<Group>? errors, out IEnumerable<(string name, string type)>? leksems);

            //если лексемы определны корректно
            if (res)
            {
                //прочитать матрицу и правила из json-файла
                MatrixJson? data = JsonSerializer.Deserialize<MatrixJson>(Properties.Resources.data2);

                if (data != null)
                {
                    //создание экземпляра синтакс. анализатора
                    var analyzer = new SyntaxAnalyzer(data);
                    //выполнение синтаксического анализа
                    //получение деревьев выражений на основе сверток в процессе синт. анализа
                    bool syntaxAnalyzeResult = analyzer.TryAnalyze(leksems,
                        ((string lex, string type) lexx) =>
                        {
                            return lexx.type switch
                            {
                                "identifier" or "constant" => "a",
                                _ => lexx.lex,
                            };
                        },
                        "E",
                        out IEnumerable<string>? syntaxErrors, out IEnumerable<SPO_LR_Lib.TreeNode>? rootNodes);

                    //если синтаксических ошибок нет
                    if (syntaxAnalyzeResult)
                    {
                        var replacer = new TriadConverter(new[] { "not" }, new[] { "and", "xor", "or", ":=" });
                        //получить триады на основе деревьев
                        var triads = replacer.GetTriads(rootNodes).ToList();

                        //создать оптимизатор включающий свертку и удаление лишних операций
                        var optimizer = new TriadOptimizerBuilder().
                            AddReduceOptimizer().
                            AddDeleteOptimizer().
                            Build();

                        //получить оптимизированный список  триад
                        var triadsOptimized = optimizer.Optimize(triads);

                        var form1 = new Form1();
                        form1.Text = "Список триад с оптимизацией";
                        using StreamReader reader = new(replacer.PrintTriads(triadsOptimized));
                        form1.richTextBox1.Text = reader.ReadToEnd();
                        form1.Show(this);

                        var form2 = new Form1();
                        form2.Text = "Список триад без оптимизации";
                        using StreamReader reader2 = new(replacer.PrintTriads(triads));
                        form2.richTextBox1.Text = reader2.ReadToEnd();
                        form2.Show(this);

                    }
                    else
                    {
                        //если ошибки есть - вывести с новой строки
                        MessageBox.Show(
                            "Обнаружены ошибки: \n-" + string.Join("\n-", syntaxErrors), 
                            "Система", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
                    }

                }
            }
            else
            {
                //произвести фокус на элементе richTextBox
                richTextBox1.Focus();
                //выделить весь введенный текст
                richTextBox1.SelectAll();
                //определить цвет фона как красный
                richTextBox1.SelectionBackColor = Color.Red;

                //пройтись по циклу по всей коллекции ошибок
                foreach (var error in errors)
                {
                    //каждая лексема "error" содержит информацию о своем индексе в тексте и длине
                    //соответственно, используя эту информацию, выделить в тексте эти выражения
                    richTextBox1.Select(error.Index, error.Length);
                }
                //вывод сообщения об ошибке
                MessageBox.Show("В тексте есть неопознанные символы", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //завершение работы функции
                return;
            }
        }
    }
}
/// <summary>
/// Метод получения лексем из текста, введенного в richTextBox
/// </summary>
//private void GetLecksems()
//{
//    //получение текста, введенного пользователем
//    string data = richTextBox1.Text;

//    //получение всех лексем, которые подпадают под шаблон регулярного выражения
//    var matches = regex.Matches(data);

//    //отбор из всех лексем только тех, которые успешно распознаны и их имена есть в списке ключей словаря
//    IEnumerable<Group> groups = matches.SelectMany(x => x.Groups.Values).Where(x => x.Success && dictionary.Keys.Contains(x.Name));

//    //отбор лексем, которые были распознаны как ошибка
//    IEnumerable<Group> errors = groups.Where(x => x.Name == "error");

//    //если ошибочные лексемы есть
//    if (errors.Any())
//    {
//        //произвести фокус на элементе richTextBox
//        richTextBox1.Focus();
//        //выделить весь введенный текст
//        richTextBox1.SelectAll();
//        //определить цвет фона как красный
//        richTextBox1.SelectionBackColor = Color.Red;

//        //пройтись по циклу по всей коллекции ошибок
//        foreach (var error in errors)
//        {
//            //каждая лексема "error" содержит информацию о своем индексе в тексте и длине
//            //соответственно, используя эту информацию, выделить в тексте эти выражения
//            richTextBox1.Select(error.Index, error.Length);
//        }
//        //вывод сообщения об ошибке
//        MessageBox.Show("В тексте есть неопознанные символы", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //завершение работы функции
//        return;
//    }

//    /*
//     * если ошибок не найдено, то преобразовать в таблицу из двух столбцов, где в 1-й столбец - это сама лексема, 
//     * второй - тип лексемы, полученный с помощью словаря
//    */
//    IEnumerable<(string, string)> list = groups.Select(x => (x.Value, dictionary[x.Name]));

//    //создание экземпляра формы для вывода лексем, отправка в его конструктор полученной таблицы
//    Form2 form = new Form2(list);
//    //вывод формы таблицы лексем
//    form.ShowDialog();

//}
