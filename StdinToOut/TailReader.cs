using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace StdinToOut
{

    internal class TailReader
    {
        private readonly string logFileName;
        private readonly string logDirectory;

        public TailReader(string logDirectory, string logFileName)
        {
            this.logFileName = logFileName;
            this.logDirectory = logDirectory;
        }

        public IEnumerable<string> Read(bool ignore = false)
        {
            using (var wh = new AutoResetEvent(false))
            {
                var fsw = new FileSystemWatcher(logDirectory, logFileName);
                // loop will pause until watcher triggers "that the file have been modified"
                fsw.Changed += (s, e) => wh.Set();
                fsw.EnableRaisingEvents = true;

                using (var fs = new FileStream(Path.Combine(logDirectory, logFileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var bfs = new BufferedStream(fs))
                using (var sr = new StreamReader(bfs))
                {
                    //while (!sr.EndOfStream)
                    //    sr.ReadLine();

                    fs.Seek(0, SeekOrigin.End);
                    string s = null;
                    while (true)
                    {
                        s = sr.ReadLine();
                        // logic för att läsa mer, så vi inte går in i wait efter varje rad
                        if (s != null)
                            yield return s;
                        else
                            wh.WaitOne(100);
                    }
                }
            }
        }
    }
}


