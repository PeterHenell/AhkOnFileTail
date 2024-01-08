using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace StdinToOut.Model
{
    internal class CompiledAction
    {
        public Regex Pattern { get; set; }
        //public IList<Group> Groups { get; set; }
        public string Command { get; set; }

        public static CompiledAction TryCompile(RawAction raw)
        {
            try
            {
                var pattern = new Regex(raw.Pattern
              , RegexOptions.IgnoreCase | RegexOptions.Compiled
              , TimeSpan.FromMilliseconds(200));

                var command = GetCommand(raw.Commands);

                return new CompiledAction(pattern, command);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not parse pattern, check regexp.");
                Console.Error.WriteLine(e.ToString());
                throw;
            }
        }

        private static string GetCommand(List<string> commands)
        {
            return string.Join(Environment.NewLine, commands);
        }

        internal bool IsMatch(string line)
        {
            return Pattern.IsMatch(line);
        }

        internal string PullTrigger(string line)
        {
            var match = Pattern.Match(line);
            var newCommand = Command;
            if (match.Groups.Count > 1)
            {
                int c = 1;
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    var replaceMatch = $"@{c}";
                    newCommand = newCommand.Replace(replaceMatch, match.Groups[i].Captures[0].Value);
                    c++;
                }
            }
            return newCommand;
        }

        public CompiledAction(Regex pattern, string action)
        {
            this.Pattern = pattern;
            this.Command = action;
        }
    }
}
