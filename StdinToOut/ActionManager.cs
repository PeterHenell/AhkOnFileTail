using AutoHotkey.Interop;
using Newtonsoft.Json;
using StdinToOut.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StdinToOut
{
    internal class ActionManager
    {
        private readonly List<AhkAction> ahkActions;
        private readonly AutoHotkeyEngine ahk = AutoHotkeyEngine.Instance;

        public ActionManager(List<AhkAction> ahkActions)
        {
            this.ahkActions = ahkActions;
        }

        public static ActionManager FromFile(string file)
        {
            var content = System.IO.File.ReadAllText(file);
            var ahkActions = JsonConvert.DeserializeObject<List<AhkAction>>(content);
            Console.WriteLine("Loaded:" + string.Join(
                Environment.NewLine,
                ahkActions.Select(a => a.Pretty())
                ));
            return new ActionManager(ahkActions);
        }

        internal void CheckAndAct(string line)
        {
            foreach (var action in ahkActions)
            {
                if (line.Contains(action.Pattern))
                {
                    string command = GetCommand(action);

                    ahk.ExecRaw(command);
                }
            }
        }

        private string GetCommand(AhkAction action)
        {
            return string.Join(Environment.NewLine, action.Commands);
        }
    }
}
