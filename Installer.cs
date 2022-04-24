using System;
using System.IO;
using System.Text;

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
            string packages = "base base-devel linux-lts linux-zen linux-firmware intel-ucode amd-ucode";
            ProcessManager.StartProcess(fileName: "pacstrap", args: $"/mnt {packages}");
            ProcessManager.StartProcess(fileName: "genfstab", args: "-U /mnt/etc/fstab");

            ProcessManager.StartProcess(fileName: "arch-chroot",
                                        args: "/mnt ln -sf /usr/share/zoneinfo/Europe/London /etc/localtime");
            ProcessManager.StartProcess(fileName: "arch-chroot", args: "/mnt timedatectl set-ntp true");
            ProcessManager.StartProcess(fileName: "arch-chroot", args: "/mnt hwclock --systohc");

            ProcessManager.StartProcess(fileName: "arch-chroot",
                                        args: "/mnt echo 'LANG=en_US.UTF-8' > /etc/locale.conf && echo 'KEYMAP=es' > /etc/vconsole.conf");
        
            ProcessManager.StartProcess(fileName: "arch-chroot", args: "/mnt echo 'archlinux' > /etc/hostname");

            Console.WriteLine("\nPlease type a password for root below");
            ProcessManager.StartProcess(fileName: "arch-chroot", args: "/mnt passwd");
        }

        public static void InstallBootloader(string disk)
        {
            string filePath = @"/root/archlinux.conf";
            ProcessManager.StartProcess(fileName: "arch-chroot", args: "/mnt bootctl install");
            try
            {
                using (FileStream fs = File.Create(filePath))
                {
                    byte[] title = new UTF8Encoding(true).GetBytes("title   Arch Linux");
                    byte[] linux = new UTF8Encoding(true).GetBytes("linux   /vmlinuz-linux");
                    byte[] initrd1 = new UTF8Encoding(true).GetBytes("initrd  /intel-ucode.img");
                    byte[] initrd2 = new UTF8Encoding(true).GetBytes("initrd  /amd-ucode.img");
                    byte[] initrd3 = new UTF8Encoding(true).GetBytes("initrd  /initramfs-linux.img");
                    byte[] options = new UTF8Encoding(true).GetBytes("options root=UUID=device_uuid_here rw add_efi_memmap delayacct quiet splash");
                    fs.Write(title, 0, title.Length);
                    fs.Write(linux, 1, linux.Length);
                    fs.Write(initrd1, 2, initrd1.Length);
                    fs.Write(initrd2, 3, initrd2.Length);
                    fs.Write(initrd3, 4, initrd3.Length);
                    fs.Write(options, 5, options.Length);
                }

                using (StreamReader sr = File.OpenText(filePath))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
