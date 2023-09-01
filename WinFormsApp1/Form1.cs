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

        //������� �� ������ "�������� �������"
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForLeksems();
        }

        private void CheckForLeksems()
        {
            //��������� ������ ������ (��� �������� � ���������� ������������ ������)
            var res = LeksemFinder.
                Singleton.
                TryGetLeksems(richTextBox1.Text, out IEnumerable<Group>? errors, out IEnumerable<(string name, string type)>? leksems);

            //���� ������� ��������� ���������
            if (res)
            {
                //��������� ������� � ������� �� json-�����
                MatrixJson? data = JsonSerializer.Deserialize<MatrixJson>(Properties.Resources.data2);

                if (data != null)
                {
                    //�������� ���������� �������. �����������
                    var analyzer = new SyntaxAnalyzer(data);
                    //���������� ��������������� �������
                    //��������� �������� ��������� �� ������ ������� � �������� ����. �������
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

                    //���� �������������� ������ ���
                    if (syntaxAnalyzeResult)
                    {
                        var replacer = new TriadConverter(new[] { "not" }, new[] { "and", "xor", "or", ":=" });
                        //�������� ������ �� ������ ��������
                        var triads = replacer.GetTriads(rootNodes).ToList();

                        //������� ����������� ���������� ������� � �������� ������ ��������
                        var optimizer = new TriadOptimizerBuilder().
                            AddReduceOptimizer().
                            AddDeleteOptimizer().
                            Build();

                        //�������� ���������������� ������  �����
                        var triadsOptimized = optimizer.Optimize(triads);

                        var form1 = new Form1();
                        form1.Text = "������ ����� � ������������";
                        using StreamReader reader = new(replacer.PrintTriads(triadsOptimized));
                        form1.richTextBox1.Text = reader.ReadToEnd();
                        form1.Show(this);

                        var form2 = new Form1();
                        form2.Text = "������ ����� ��� �����������";
                        using StreamReader reader2 = new(replacer.PrintTriads(triads));
                        form2.richTextBox1.Text = reader2.ReadToEnd();
                        form2.Show(this);

                    }
                    else
                    {
                        //���� ������ ���� - ������� � ����� ������
                        MessageBox.Show(
                            "���������� ������: \n-" + string.Join("\n-", syntaxErrors), 
                            "�������", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
                    }

                }
            }
            else
            {
                //���������� ����� �� �������� richTextBox
                richTextBox1.Focus();
                //�������� ���� ��������� �����
                richTextBox1.SelectAll();
                //���������� ���� ���� ��� �������
                richTextBox1.SelectionBackColor = Color.Red;

                //�������� �� ����� �� ���� ��������� ������
                foreach (var error in errors)
                {
                    //������ ������� "error" �������� ���������� � ����� ������� � ������ � �����
                    //��������������, ��������� ��� ����������, �������� � ������ ��� ���������
                    richTextBox1.Select(error.Index, error.Length);
                }
                //����� ��������� �� ������
                MessageBox.Show("� ������ ���� ������������ �������", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //���������� ������ �������
                return;
            }
        }
    }
}
/// <summary>
/// ����� ��������� ������ �� ������, ���������� � richTextBox
/// </summary>
//private void GetLecksems()
//{
//    //��������� ������, ���������� �������������
//    string data = richTextBox1.Text;

//    //��������� ���� ������, ������� ��������� ��� ������ ����������� ���������
//    var matches = regex.Matches(data);

//    //����� �� ���� ������ ������ ���, ������� ������� ���������� � �� ����� ���� � ������ ������ �������
//    IEnumerable<Group> groups = matches.SelectMany(x => x.Groups.Values).Where(x => x.Success && dictionary.Keys.Contains(x.Name));

//    //����� ������, ������� ���� ���������� ��� ������
//    IEnumerable<Group> errors = groups.Where(x => x.Name == "error");

//    //���� ��������� ������� ����
//    if (errors.Any())
//    {
//        //���������� ����� �� �������� richTextBox
//        richTextBox1.Focus();
//        //�������� ���� ��������� �����
//        richTextBox1.SelectAll();
//        //���������� ���� ���� ��� �������
//        richTextBox1.SelectionBackColor = Color.Red;

//        //�������� �� ����� �� ���� ��������� ������
//        foreach (var error in errors)
//        {
//            //������ ������� "error" �������� ���������� � ����� ������� � ������ � �����
//            //��������������, ��������� ��� ����������, �������� � ������ ��� ���������
//            richTextBox1.Select(error.Index, error.Length);
//        }
//        //����� ��������� �� ������
//        MessageBox.Show("� ������ ���� ������������ �������", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //���������� ������ �������
//        return;
//    }

//    /*
//     * ���� ������ �� �������, �� ������������� � ������� �� ���� ��������, ��� � 1-� ������� - ��� ���� �������, 
//     * ������ - ��� �������, ���������� � ������� �������
//    */
//    IEnumerable<(string, string)> list = groups.Select(x => (x.Value, dictionary[x.Name]));

//    //�������� ���������� ����� ��� ������ ������, �������� � ��� ����������� ���������� �������
//    Form2 form = new Form2(list);
//    //����� ����� ������� ������
//    form.ShowDialog();

//}
