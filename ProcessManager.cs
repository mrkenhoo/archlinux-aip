using System;
using System.Diagnostics;

namespace archlinux_aip
{
    internal class ProcessManager
    {
        public static void StartProcess(string fileName, string args)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = args;
                process.Start();
                process.WaitForExit();
                process.Close();
            }
            catch (Exception)
            {
                throw new();
            }
        }
    }
}
