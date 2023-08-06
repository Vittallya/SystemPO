
using System.Text.RegularExpressions;

//AutomatRecognizer recognizer = new();

//recognizer.CreateState("a1", StateType.Start);
//recognizer.CreateState("a2");
//recognizer.CreateState("a3");
//recognizer.CreateState("a4", StateType.Final);

//recognizer.CreateConnection("a1", "a2", "+");
//recognizer.CreateConnection("a2", "a3", "7");
//recognizer.CreateConnection("a3", "a4", "0-9");
//recognizer.CreateConnection("a4", "a4", "0-9");


//var a = "+7084093824908590091";

//bool res = recognizer.TryRecognize(a, out _);
//Console.WriteLine(res);



//Regex regex = new("(?'logicalOperation'xor|and|or|not) | (?'identitfier'[a-z][a-z0-9]*) | (?'equal':=) | (?'lBrain'\\() | (?'rBrain'\\)) | (?'constant'[0-1]+) | (?'point';)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
Regex regex = new("if\\s((?<constant>[a-f0-9]*\\s)*|(?<identifier>[a-z][0-9a-z]*\\s)* | (?<eq>(:=|=|<|>)\\s)*)*then", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

string data = "if kajdfgj 394589348 jgadfjgj > < = := then";

var matches = regex.Matches(data);
var match = regex.Match(data);
string abc = regex.Replace(data, "");

IEnumerable<Group> groups = matches.SelectMany(x => x.Groups.Values).Where(x => x.Success && x.Name != "0");

foreach(Group g in groups)
{
}

Console.WriteLine(matches);
