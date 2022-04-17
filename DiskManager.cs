using System;

namespace archlinux_aip
{
    internal class DiskManager
    {
        public static void GetSystemDisks()
        {
            Console.WriteLine("\n:: Available disks on this system:");
            ProcessManager.StartProcess(fileName: "lsblk", args: "-o path,size,mountpoint");
        }

        public static void FormatDisk(string disk, string bootSize, string rootSize)
        {
            if (!string.IsNullOrEmpty(disk))
            {
                if (string.IsNullOrEmpty(bootSize)) { bootSize = "500M"; };
                if (string.IsNullOrEmpty(bootSize)) { rootSize = "100G"; };

                ProcessManager.StartProcess(fileName: "sgdisk", args: $"-Z {disk}");

                ProcessManager.StartProcess(fileName: "sgdisk", args: $"-a 2048 -o {disk}");

                ProcessManager.StartProcess(fileName: "sgdisk", args: $"-n 1::+{bootSize} --typecode=1:ef00 --change-name=1:BOOT {disk}");
                ProcessManager.StartProcess(fileName: "sgdisk", args: $"-n 2::+{rootSize} --typecode=2:8300 --change-name=2:root {disk}");
                ProcessManager.StartProcess(fileName: "sgdisk", args: $"-n 3::-0 --typecode=3:8300 --change-name=3:home {disk}");

                ProcessManager.StartProcess(fileName: "partprobe", args: $"{disk}");
            }
            else
            {
                Console.WriteLine(":: ERROR: No disk was specified.");
                Environment.Exit(1);
            }
        }

        public static void InstallFilesystem(string filesystem, string disk)
        {
            if (!string.IsNullOrEmpty(filesystem))
            {
                if (filesystem != "ext4")
                {
                    Console.WriteLine($":: ERROR: {filesystem}: filesystem not yet implemented, use EXT4 instead");
                    Environment.Exit(1);
                }

                if (!disk.Contains("nvme"))
                {
                    ProcessManager.StartProcess(fileName: "mkfs.vfat", args: $"-F 32 -n BOOT {disk}1");
                    ProcessManager.StartProcess(fileName: $"mkfs.{filesystem}", args: $"{disk}2");
                    ProcessManager.StartProcess(fileName: $"mkfs.{filesystem}", args: $"{disk}3");
                }
                else
                {
                    ProcessManager.StartProcess(fileName: "mkfs.vfat", args: $"-F 32 -n BOOT {disk}p1");
                    ProcessManager.StartProcess(fileName: $"mkfs.{filesystem}", args: $"{disk}p2");
                    ProcessManager.StartProcess(fileName: $"mkfs.{filesystem}", args: $"{disk}p3");
                }
            }
            else
            {
                Console.WriteLine(":: ERROR: No filesystem was specified.");
                Environment.Exit(1);
            }
        }
    }
}
;