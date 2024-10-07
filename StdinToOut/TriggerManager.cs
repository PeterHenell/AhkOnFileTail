using Newtonsoft.Json;
using StdinToOut.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;

namespace StdinToOut
{
    public class TriggerManager
    {
        private List<CompiledAction> triggers;
        private readonly IAHKManager ahkManager;
        private readonly string triggerFile;

        private TriggerManager(IAHKManager ahkmanager, string triggerFile)
        {
            this.ahkManager = ahkmanager;
            this.triggerFile = triggerFile;
        }

        private static List<CompiledAction> LoadTriggers(string triggerFile)
        {
            var jsonString = System.IO.File.ReadAllText(triggerFile);
            var newTriggers = new List<CompiledAction>();
            var ahkActions = JsonConvert.DeserializeObject<List<RawAction>>(jsonString);
            foreach (var raw in ahkActions)
            {
                newTriggers.Add(CompiledAction.TryCompile(raw));
            }
            Console.WriteLine("Loaded:" + Environment.NewLine + string.Join(
                Environment.NewLine,
                ahkActions.Select(a => a.Pretty())
                ));
            return newTriggers;
        }

        public static TriggerManager FromFile(string file, IAHKManager aHKManager)
        {
            var tm = new TriggerManager(aHKManager, file);
            tm.triggers = LoadTriggers(tm.triggerFile);
            tm.StartConfigWatcher();
            return tm;
        }

        private void StartConfigWatcher()
        {
            var logDirectory = Directory.GetCurrentDirectory().ToString();
           
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = logDirectory;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = this.triggerFile;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                WaitForFile(triggerFile);
                var loadedTriggers = LoadTriggers(triggerFile);

                this.triggers = loadedTriggers;
            }
            catch (Exception xe)
            {
                Console.WriteLine("error while loading triggers: " + xe.ToString());
            }
          
        }
        public static void WaitForFile(string filename)
        {
            //This will lock the execution until the file is ready
            while (!IsFileReady(filename)) { }
        }
        public static bool IsFileReady(string filename)
        {
            // https://stackoverflow.com/questions/1406808/wait-for-file-to-be-freed-by-process/1406853#1406853
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
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
