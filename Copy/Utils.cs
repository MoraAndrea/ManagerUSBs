using System;
using System.Collections.Generic;
using System.Management;
using System.IO;

namespace Copy
{
    class Utils
    {
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void CopyFiles(ICollection<string> sourceFiles, string destDirName, bool overwrite)
        {
            foreach (var file in sourceFiles)
            {
                string NameFile = file.Split('\\')[file.Split('\\').Length-1];
                CopyFile(file, destDirName + "\\"+NameFile,true);
            }
        }

        public static void CopyFile(string sourceFile, string destFile, bool overwrite)
        {
            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, overwrite);
        }

        public static void FindUsbConnected()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
                Console.WriteLine("  Drive type: {0}", d.DriveType);
                if (d.IsReady == true)
                {
                    Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
                    Console.WriteLine("  File system: {0}", d.DriveFormat);
                    Console.WriteLine(
                        "  Available space to current user:{0, 15} bytes",
                        d.AvailableFreeSpace);

                    Console.WriteLine(
                        "  Total available space:          {0, 15} bytes",
                        d.TotalFreeSpace);

                    Console.WriteLine(
                        "  Total size of drive:            {0, 15} bytes ",
                        d.TotalSize);
                }
            }
        }

        public static void PrintUsbDevices()
        {
            IList<ManagementBaseObject> usbDevices = GetUsbDevices();

            foreach (ManagementBaseObject usbDevice in usbDevices)
            {
                Console.WriteLine("----- DEVICE -----");
                foreach (var property in usbDevice.Properties)
                {
                    Console.WriteLine(string.Format("{0}: {1}", property.Name, property.Value));
                }
                Console.WriteLine("------------------");
            }
        }

        private static IList<ManagementBaseObject> GetUsbDevices()
        {
            IList<string> usbDeviceAddresses = LookUpUsbDeviceAddresses();

            List<ManagementBaseObject> usbDevices = new List<ManagementBaseObject>();

            foreach (string usbDeviceAddress in usbDeviceAddresses)
            {
                // query MI for the PNP device info
                // address must be escaped to be used in the query; luckily, the form we extracted previously is already escaped
                ManagementObjectCollection curMoc = QueryMi("Select * from Win32_PnPEntity where PNPDeviceID = " + usbDeviceAddress);
                foreach (ManagementBaseObject device in curMoc)
                {
                    usbDevices.Add(device);
                }
            }

            return usbDevices;
        }

        private static IList<string> LookUpUsbDeviceAddresses()
        {
            // this query gets the addressing information for connected USB devices
            ManagementObjectCollection usbDeviceAddressInfo = QueryMi(@"Select * from Win32_USBControllerDevice");

            List<string> usbDeviceAddresses = new List<string>();

            foreach (var device in usbDeviceAddressInfo)
            {
                string curPnpAddress = (string)device.GetPropertyValue("Dependent");
                // split out the address portion of the data; note that this includes escaped backslashes and quotes
                curPnpAddress = curPnpAddress.Split(new String[] { "DeviceID=" }, 2, StringSplitOptions.None)[1];

                usbDeviceAddresses.Add(curPnpAddress);
            }

            return usbDeviceAddresses;
        }

        // run a query against Windows Management Infrastructure (MI) and return the resulting collection
        private static ManagementObjectCollection QueryMi(string query)
        {
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection result = managementObjectSearcher.Get();

            managementObjectSearcher.Dispose();
            return result;
        }
    }
}
