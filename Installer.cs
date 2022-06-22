using System;
using System.IO;

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

                    if (!Directory.Exists("/mnt/boot") && !Directory.Exists("/mnt/home"))
                    {
                        ProcessManager.StartProcess(fileName: "mkdir", args: "-v /mnt/boot");
                        ProcessManager.StartProcess(fileName: "mkdir", args: "-v /mnt/home");
                    }

                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}1 /mnt/boot");
                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}3 /mnt/home");
                }
                else
                {
                    ProcessManager.StartProcess(fileName: "mount", args: $"-v {disk}p2 /mnt");

                    if (!Directory.Exists("/mnt/boot") && !Directory.Exists("/mnt/home"))
                    {
                        ProcessManager.StartProcess(fileName: "mkdir", args: "-v /mnt/boot");
                        ProcessManager.StartProcess(fileName: "mkdir", args: "-v /mnt/home");
                    }
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
            string packages = "base base-devel linux linux-firmware intel-ucode amd-ucode";
            ProcessManager.StartProcess(fileName: "pacstrap", args: $"/mnt {packages}");
            ProcessManager.StartProcess(fileName: "genfstab", args: "-t PARTUUID /mnt >> /mnt/etc/fstab");

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
            ProcessManager.StartProcess(fileName: "arch-chroot", args: "/mnt bootctl install");
            try
            {
                string[] file =
                {
                    "title   Arch Linux",
                    "linux   /vmlinuz-linux",
                    "initrd  /intel-ucode.img",
                    "initrd  /amd-ucode.img",
                    "initrd  /initramfs-linux.img",
                    "options root=PARTUUID=device_partuuid_here rw add_efi_memmap quiet splash"
                };

                File.WriteAllLines("/boot/loader/entries/archlinux.conf", file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (!disk.Contains("nvme"))
            {
                ProcessManager.StartProcess(fileName: "arch-chroot", args: $"/mnt sed 's,PARTUUID=device_partuuid_here,PARTUUID=`blkid -s PARTUUID -o value {disk}1`,g' -i /boot/loader/entries/archlinux.conf");
            }
            else
            {
                ProcessManager.StartProcess(fileName: "arch-chroot", args: $"/mnt blkid -s PARTUUID -o value {disk}p1");
            }
        }
    }
}
