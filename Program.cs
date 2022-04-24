using System;

namespace archlinux_aip
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "archlinux-aip";
            Console.WriteLine("Welcome to the Arch Linux Automated Installation Procedure tool!");
            Console.WriteLine("Created by Ken Hoo (mrkenhoo)");

            if (!PrivilegesManager.IsUserAdmin(username: Environment.UserName))
            {
                Console.WriteLine("\nPlease execute this program as root");
                Environment.Exit(1);
            }

            DiskManager.GetSystemDisks();

            Console.WriteLine("\n:: Type the disk you want to format below (e.g.: /dev/sda)");
            string disk = Console.ReadLine();

            if (!disk.Contains("/dev/"))
            {
                Console.WriteLine($":: ERROR: {disk} Invalid device, try again.");
                Environment.Exit(1);
            }

            Console.WriteLine("\n:: Specify the size for the boot partition below (default: 500M)");
            string bootSize = Console.ReadLine();

            Console.WriteLine("\n:: Specify the size for the root partition below (default: 100G)");
            string rootSize = Console.ReadLine();

            DiskManager.FormatDisk(disk: $"{disk}", bootSize: $"{bootSize}", rootSize: $"{rootSize}");

            Console.WriteLine("\n:: Specify the filesystem to use below (default: ext4)");
            string filesystem = Console.ReadLine();

            DiskManager.InstallFilesystem(filesystem: $"{filesystem}", disk: $"{disk}");

            Installer.PrepareInstallation(disk: $"{disk}");
            Installer.InstallBaseSystem();
            Installer.InstallBootloader(disk: $"{disk}");
        }
    }
}
