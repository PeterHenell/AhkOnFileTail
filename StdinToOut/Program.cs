using AutoHotkey.Interop;
using System;
using System.Threading;

namespace StdinToOut
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException($"not enough args supplied, requires [dir, file configfilePath]: found {args.Length} parameters, requires 3");
            }
            
            var dir = args[0];
            var file = args[1];
            var config = args[2];

            var manager = TriggerManager.FromFile(config, new AHKManager());

            var reader = new TailReader(dir, file);

            foreach (var line in reader.Read(false))
            {
                manager.CheckAndAct(line);             
            }
        }
    }
}
