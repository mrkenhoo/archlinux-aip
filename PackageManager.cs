using System;
using System.IO;

namespace archlinux_aip
{
    internal class PackageManager
    {
        public static void InstallPackage(string packageName)
        {
            if (File.Exists("/usr/bin/pacman"))
            {
                ProcessManager.StartProcess("pacman", $"-Syu {packageName} --needed --noconfirm");
            }
        }
    }
}

