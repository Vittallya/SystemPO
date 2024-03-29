﻿using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SPO_LR_Lib
{
    public class LeksemFinder
    {

        public static LeksemFinder Singleton => new LeksemFinder();

        private readonly Regex regex = new(@"   
                                                (?'identifier'(xor|and|or|not)[\w|_]+) |
                                                (?'logicalOperation'xor|and|or|not) | 
                                                (?'eqSign' :=) | 
                                                (?'opSign' \(|\))| (?'identifier'([a-z](\w|_)*)) | 
                                                (?'constant'[0-1]+) | (?'point';) | (\*\*.*\*\*) | 
                                                (?'error'\S)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        //простой словарь, который позволяет имена выражений перевести в читабельный вид на русском языке
        private readonly Dictionary<string, string> dictionary = new()
        {
            {"logicalOperation", "Логическая операция" },
            {"identifier", "Идентификатор" },
            {"opSign", "Знак операции" },
            {"eqSign", "Знак операции" },
            {"constant", "Константа" },
            {"point", "Точка с запятой" },
            {"error", "Ошибка" }
        };

        public LeksemFinder()
        {
            
        }

        public LeksemFinder(Regex regex, Dictionary<string, string> dictionary)
        {
            this.regex = regex;
            this.dictionary = dictionary;
        }

        public bool TryGetLeksems(string text, out IEnumerable<Group> errors, out IEnumerable<(string name, string type)> leksems)
        {
            leksems = new List<(string name, string type)>();

            //получение всех лексем, которые подпадают под шаблон регулярного выражения
            var matches = regex.Matches(text);

            //отбор из всех лексем только тех, которые успешно распознаны и их имена есть в списке ключей словаря
            IEnumerable<Group> groups = matches.
                SelectMany(x => x.Groups.Values).
                Where(x => x.Success && dictionary.ContainsKey(x.Name));

            //отбор лексем, которые были распознаны как ошибка
            errors = groups.Where(x => x.Name == "error");

            //если ошибочные лексемы есть
            if (errors.Any())
            {
                return false;
            }

            /*
             * если ошибок не найдено, то преобразовать в таблицу из двух столбцов, где в 1-й столбец - это сама лексема, 
             * второй - тип лексемы, полученный с помощью словаря
            */
            var orderedGroups = groups.
                SelectMany(x => x.Captures.Select(y => (x, y)).
                GroupBy(x => x.y.Index)).
                OrderBy(x => x.Key);

            leksems = orderedGroups.SelectMany(x => x.Select(y => (name: y.y.Value.Trim(), type: y.x.Name)));

            return true;
        }
    }
}