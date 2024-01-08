using System;
using System.Collections.Generic;
using System.Text;

namespace StdinToOut.Model
{
    public class RawAction
    {
        public string Pattern { get; set; }
        public List<string> Commands { get; set; } = new List<string>();

        internal string Pretty()
        {
            return $"{Pattern}: Commands: {string.Join(Environment.NewLine, Commands.ToArray())}";
        }
    }
}
