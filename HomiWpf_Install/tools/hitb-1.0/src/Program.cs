using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace hitb
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {

                    case "--check-installed-svc":
                    case "-cis":
                        // vérifie si Homidom Server est installé en tant que service windows
                        Environment.Exit(CheckIfInstalledAsService() ? 1 : 0);
                        break;
                    case "--kill-all":
                    case "-ka":
                        // arrête tous les services et applications HoMIDom en cours d'execution (Kill)
                        Environment.Exit(killAll());
                        break;
                    case "--check-running-svc":
                    case "-crs":
                        Environment.Exit(CheckIfServiceIsRunning() ? 1 : 0);
                        break;
                    case "--stop-service":
                    case "-sps":
                        StopService("Homidom");
                        KillProcess("HomidomService");
                        Environment.Exit(0);
                        break;
                    case "--start-service":
                    case "-sts":
                        Environment.Exit(StartService("Homidom") ? 1 : 0);
                        break;
                    case "--check-running-app":
                    case "-cra":
                        Environment.Exit(IsProcessRunning("HomiAdmin") || IsProcessRunning("HomiWpf") ? 1 : 0);
                        break;

                    case "--detect-previous-install":
                    case "-dpi":
                        Environment.Exit(DetectPreviousInstall() ? 1 : 0);
                        break;

                    case "--unsinstall-previous-install":
                    case "-upi":
                        Environment.Exit(DetectPreviousInstall(true) ? 1 : 0);

                        break;
                    default:
                        break;
                }

            }
            else
            {
                ShowUsage();
            }

        }

        private static bool DetectPreviousInstall(bool uninstall = false)
        {

            // HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Products\B1CDED60CC25BC848B8A97B3F5D84E04
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\");
            foreach (var v in key.GetSubKeyNames())
            {
                RegistryKey productKey = key.OpenSubKey(v);
                if (productKey != null)
                {
                    foreach (var value in productKey.GetValueNames())
                    {
                        string productName = Convert.ToString(productKey.GetValue("ProductName"));
                        if (!productName.Equals("HoMIDoM", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string productIcon = productKey.GetValue("ProductIcon").ToString();
                        string installUID = productIcon.Substring(productIcon.LastIndexOf("\\") - 38, 38);

                        if (!uninstall) return true;

                        // msiexec /x {7FC2F25A-2D7C-48B8-88C8-4D1EE59ED19E} /passive

                        Process proc = new Process();
                        proc.StartInfo.FileName = "msiexec.exe";
                        proc.StartInfo.Arguments = String.Format("/x {0} /passive", installUID);
                        proc.StartInfo.UseShellExecute = true;
                        proc.Start();
                        proc.WaitForExit();

                        return true;
                    }
                }
            }


            return false;

        }

        private static bool StartService(string serviceName, int timeoutMilliseconds = 5000)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("HoMIDoM Installer ToolBox v1.0");
            Console.WriteLine("Usage:");
            Console.WriteLine("hitb.exe <command> [<options>]\r\n");
        }

        private static bool CheckIfServiceIsRunning()
        {
            // recherche du service windows -ou- du process
            Process[] pname = Process.GetProcessesByName("HomidomService");
            ServiceController ctl = ServiceController.GetServices().Where(s => s.ServiceName.ToLower() == "homidom").FirstOrDefault();
            if (ctl != null && ctl.Status == ServiceControllerStatus.Running)
                return true;

             return (pname.Length != 0);

        }

        private static bool CheckIfInstalledAsService()
        {
            try
            {
                ServiceController ctl = ServiceController.GetServices().Where(s => s.ServiceName.ToLower() == "homidom").FirstOrDefault();
                return (ctl != null);
            }
            catch (Exception)
            {

                return false;
            }

        }

        private static void KillProcess(string processName)
        {
            try
            {
                Process[] pname = Process.GetProcessesByName(processName);
                if (pname.Length != 0)
                    pname[0].Kill();

            }
            catch (Exception)
            {

            }

        }

        private static bool IsProcessRunning(string processName)
        {
            try
            {
                Process[] pname = Process.GetProcessesByName(processName);
                return (pname.Length != 0);
            }
            catch (Exception)
            {
                return false;
            }

        }

        static int killAll()
        {
            try
            {
                KillProcess("HoMIGuI");
                KillProcess("HomiWpf");
                KillProcess("HomiAdmin");
                KillProcess("HomidomService");

                return 0;
            }
            catch (Exception)
            {

                return 5;
            }

        }

        public static void StopService(string serviceName, int timeoutMilliseconds = 5000)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                // ...
            }
        }
    }
}
