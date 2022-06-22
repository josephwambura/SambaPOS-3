using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Samba.Infrastructure.Data;

namespace Samba.Domain.Models.Settings
{
    public class PrinterTemplate : EntityClass
    {
        private string _template;
        public string Template
        {
            get { return _template; }
            set
            {
                _template = value;
                _parts = null;
            }
        }

        public bool MergeLines { get; set; }

        private IDictionary<string, StringBuilder> _parts;
        public IDictionary<string, StringBuilder> Parts
        {
            get { return _parts ?? (_parts = CreateParts(Template)); }
        }

        public string GetPart(string partName)
        {
            if (string.IsNullOrEmpty(partName)) partName = "LAYOUT";
            if (Parts.ContainsKey(partName)) return Parts[partName].ToString();
            var p2 = partName.Contains(":") ? partName.Substring(0, partName.IndexOf(':')) : "";
            return Parts.ContainsKey(p2)
                ? Parts[p2].ToString()
                : Parts.Keys.Any(x => x.StartsWith(p2 + "|"))
                ? Parts[Parts.Keys.First(x => x.StartsWith(p2 + "|"))].ToString()
                : Parts.Keys.Any(x => x.StartsWith(partName + "|")) ? Parts[Parts.Keys.First(x => x.StartsWith(partName + "|"))].ToString() : "";
        }

        private static IDictionary<string, StringBuilder> CreateParts(string template)
        {
            var result = new Dictionary<string, StringBuilder>();
            var currentSection = "LAYOUT";
            result.Add(currentSection, new StringBuilder());
            foreach (var line in template.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var m = Regex.Match(line, @"(?<=\[)(?<SectionName>[^\]]+)(?=\])");
                if (m.Success && !line.Contains("<"))
                {
                    currentSection = m.Groups["SectionName"].Value;
                    if (!result.ContainsKey(currentSection))
                        result.Add(currentSection, new StringBuilder());
                }
                else
                {
                    if (!line.StartsWith("-- ") && result.ContainsKey(currentSection))
                        result[currentSection].AppendLine(line);
                }
            }
            return result;
        }

        public string GetSwitch(string s)
        {
            var key = Parts.Keys.FirstOrDefault(x => x.StartsWith(s + "|"));
            return key?.Substring(key.IndexOf("|", StringComparison.Ordinal)).Trim('|');
        }

        public Dictionary<string, string> GetFilters(string partName)
        {
            return Parts.Where(x => x.Key.StartsWith(partName + ":")).ToDictionary(y => y.Key.Substring(y.Key.IndexOf(":", StringComparison.Ordinal) + 1), y => y.Key);
        }
    }
}
