using Newtonsoft.Json;
using StdinToOut.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace StdinToOut
{
    public class TriggerManager
    {
        private readonly List<CompiledAction> triggers;
        private readonly IAHKManager ahkManager;
       

        private TriggerManager(List<RawAction> ahkActions, IAHKManager ahkmanager)
        {
            this.triggers = new List<CompiledAction>();
            this.ahkManager = ahkmanager;

            foreach (var raw in ahkActions)
            {
                triggers.Add(CompiledAction.TryCompile(raw));
            }
        }

        public static TriggerManager FromText(string content, IAHKManager aHKManager)
        {
            var ahkActions = JsonConvert.DeserializeObject<List<RawAction>>(content);
            Console.WriteLine("Loaded:" + string.Join(
                Environment.NewLine,
                ahkActions.Select(a => a.Pretty())
                ));
            return new TriggerManager(ahkActions, aHKManager);
        }

        public static TriggerManager FromFile(string file, IAHKManager aHKManager)
        {
            var content = System.IO.File.ReadAllText(file);
            return TriggerManager.FromText(content, aHKManager);
        }

        public void CheckAndAct(string line)
        {
            foreach (var trigger in triggers)
            {
                if (trigger.IsMatch(line))
                {
                    var command = trigger.PullTrigger(line);
                    ahkManager.SendCommand(command);
                }   
            }
        }
    }
}
