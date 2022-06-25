using System;
using System.Collections;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace Samba.MessagingServer.WindowsService
{
    public static class ServiceHelper
    {
        private static readonly string _installAssembly = "Samba.MessagingServer.WindowsService.exe";
        private static readonly string _logFile = "WindowsServiceInstall.log";
        private static readonly string _serviceName = "SambaPOS3-MessagingServer";
        private static ServiceController _ctl;

        public static int MessagingServerPort
        {
            get
            {
                if (!File.Exists(MessagingServer.MessagingServerPortFile))
                { File.WriteAllText(MessagingServer.MessagingServerPortFile, MessagingServer.StdPort.ToString()); }

                return Convert.ToInt32(File.ReadAllText(MessagingServer.MessagingServerPortFile));
            }
        }

        public static ServiceControllerStatus? CheckServiceStatus()
        {
            InitServiceController();
            return _ctl == null ? null : (ServiceControllerStatus?)_ctl.Status;
        }

        public static bool InstallWindowsService()
        {
            bool ret = ServiceInstaller();
            StartService();
            return ret;
        }

        public static void StartService(string[] args = null)
        {
            InitServiceController();
            if (_ctl.Status == ServiceControllerStatus.Stopped)
            {
                if (args == null)
                { _ctl.Start(); }
                else
                { _ctl.Start(args); }
            }
        }

        public static void StopService()
        {
            InitServiceController();
            if (_ctl.Status == ServiceControllerStatus.Running)
            { _ctl.Stop(); }
        }

        public static bool UninstallWindowsService()
        {
            StopService();

            return ServiceInstaller(true);
        }

        private static void InitServiceController()
        {
            _ctl = ServiceController.GetServices()
                                  .Where(s => s.ServiceName == _serviceName)
                                  .FirstOrDefault();
        }

        private static bool ServiceInstaller(bool uninstall = false)
        {
            IDictionary mySavedState = new Hashtable();

            try
            {
                // Set the commandline argument array for 'logfile'.
                string[] commandLineOptions = new string[1] { string.Format("/LogFile={0}", _logFile) };

                // Create an object of the 'AssemblyInstaller' class.
                AssemblyInstaller myAssemblyInstaller = new
                AssemblyInstaller(_installAssembly, commandLineOptions)
                {
                    UseNewContext = true
                };

                if (!uninstall)
                {
                    myAssemblyInstaller.Install(mySavedState);
                    // Commit the 'MyAssembly' assembly.
                    myAssemblyInstaller.Commit(mySavedState);
                }
                else
                { myAssemblyInstaller.Uninstall(mySavedState); }
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            { return false; }

            return true;
        }
    }
}