using System.Text.RegularExpressions;

namespace AutomatRecognizerTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("+7843857837458", ExpectedResult = true)]
        [TestCase("+7", ExpectedResult = false)]
        [TestCase("abc", ExpectedResult = false)]
        [TestCase("", ExpectedResult = true)]
        public bool Test1(string input)
        {

            //Regex regex = new Regex("[", RegexOptions.Compiled);

            var recognizer = new AutomatRecognizer();

            recognizer.CreateState("a1", StateType.Start);
            recognizer.CreateState("a2");
            recognizer.CreateState("a3");
            recognizer.CreateState("a4", StateType.Final);

            recognizer.CreateConnection("a1", "a2", "+");
            recognizer.CreateConnection("a2", "a3", "7");
            recognizer.CreateConnection("a3", "a4", "0-9");
            recognizer.CreateConnection("a4", "a4", "0-9");
            recognizer.CreateConnection("a1", "a4", "");

            return recognizer.TryRecognize(input, out _);
        }
    }
}