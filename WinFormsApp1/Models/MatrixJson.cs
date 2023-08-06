using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WinFormsApp1.Models
{
    public class MatrixJson
    {
        [JsonPropertyName("rules")]
        public string[][] Rules { get; set; }

        [JsonPropertyName("matrix")]
        public Dictionary<string, Dictionary<string, string>> Matrix { get; set; }
    }
}
