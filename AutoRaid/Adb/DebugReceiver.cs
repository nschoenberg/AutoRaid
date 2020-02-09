using SharpAdbClient;
using System;
using System.Diagnostics;
using System.Text;

namespace AutoRaid.Adb
{
    public class DebugReceiver : IShellOutputReceiver
    {
        private StringBuilder _stringBuilder = new StringBuilder();
        public bool ParsesErrors => true;

        public void AddOutput(string line)
        {
            _stringBuilder.AppendLine(line);
        }

        public void Flush()
        {
            Debug.WriteLine(_stringBuilder.ToString());
            _stringBuilder.Clear();
        }
    }
}
