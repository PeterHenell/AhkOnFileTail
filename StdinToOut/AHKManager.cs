using AutoHotkey.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StdinToOut
{
    public class AHKManager : IAHKManager
    {
        private readonly AutoHotkeyEngine ahk = AutoHotkeyEngine.Instance;

        public void SendCommand(string command)
        {
            ahk.ExecRaw(command);
        }
    }
}
