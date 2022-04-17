using System;

namespace archlinux_aip
{
    internal class Installer
    {
        public static void PrepareInstallation(string disk)
        {
            if (!string.IsNullOrEmpty(disk))
            {
                if (!disk.Contains("nvme"))
                {
                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}2 /mnt");
                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}1 /mnt/boot");
                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}3 /mnt/home");
                }
                else
                {
                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}p2 /mnt");
                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}p1 /mnt/boot");
                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}p3 /mnt/home");
                }
            }
            else
            {
                Console.WriteLine("\n:: ERROR: No partition was specified");
                Environment.Exit(1);
            }
        }

        public static void InstallBaseSystem()
        {
            string packages = "base base-devel linux-lts linux-zen linux-firmware";
            ProcessManager.StartProcess(fileName: "pacstrap", args: $"/mnt {packages}");
        }
    }
}
