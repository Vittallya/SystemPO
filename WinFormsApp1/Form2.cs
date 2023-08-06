namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(IEnumerable<(string, string)> list) : this()
        {
            //протись в цикле по каждой строке таблицы, полученной из-вне
            foreach ((string, string) item in list)
            {
                //добавить в таблицу формы "dataGridView" очередную строку с именем и типом лексемы
                dataGridView1.Rows.Add(item.Item1, item.Item2);
            }
        }
    }
}
