using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows;

namespace Copy
{
    public partial class MainWindow : Window
    {
        // public ObservableCollection<string> DevicesUsb_list { get; set; }
        public ObservableCollection<Device> DevicesUsb_list { get; set; }
        public ObservableCollection<string> Files_list { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //DevicesUsb_list = new ObservableCollection<string>();
            DevicesUsb_list = new ObservableCollection<Device>();
            Files_list = new ObservableCollection<string>();
            Utils.FindUsbConnected();
            List<string> list_usb_connected = DriveInfo.GetDrives().Where(d => d.DriveType.ToString() == "Removable").Select(d => d.Name).ToList();
            foreach (var i in list_usb_connected)
            {
                Console.WriteLine(i.Replace("\\", ""));
                DevicesUsb_list.Add(new Device() { Name = i.Replace("\\", ""), IsChecked = true });
            }
            DevicesListBox.ItemsSource = DevicesUsb_list;
            FilesListBox.ItemsSource = Files_list;

            var th = new Thread(SetWatchersUsbBackground)
            {
                IsBackground = true
            };
            th.Start();
        }

        private void SetWatchersUsbBackground()
        {
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertWatcher.Start();
            insertWatcher.WaitForNextEvent();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 3");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeWatcher.Start();
            removeWatcher.WaitForNextEvent();
        }

        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
            Console.Write(driveName + " inserted ");
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LoggerBox.Text += driveName + " inserted \n";
                DevicesUsb_list.Add(new Device() { Name = driveName, IsChecked = true });
            }));
            /*ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
             foreach (var property in instance.Properties)
             {
                 Console.WriteLine(property.Name + " = " + property.Value);
             }*/
        }

        private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
            Console.Write(driveName + " removed ");
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LoggerBox.Text += driveName + " removed \n";
                try
                {
                    DevicesUsb_list.Remove(DevicesUsb_list.Where(i => i.Name == driveName).Single());
                }
                catch
                {
                    LoggerBox.Text += "ERROR remove...\n";
                }
            }));
            /*ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            foreach (var property in instance.Properties)
            {
                Console.WriteLine(property.Name + " = " + property.Value);
            }*/
        }

        private void ButtonCopy(object sender, RoutedEventArgs e)
        {
            List<Thread> workerThreads = new List<Thread>();
            foreach (var item in DevicesUsb_list)
            {
                if (item.IsChecked)
                {
                    Thread thread = new Thread(() => Utils.CopyFiles(Files_list, item.Name, false));
                    thread.Name = item.Name;
                    workerThreads.Add(thread);
                    thread.Start();
                    LoggerBox.Text += "--> Copy on: "+item.Name+"\n";
                    // Utils.DirectoryCopy("C:\\Users\\Andre\\Desktop\\dirSrc", item.Name, false);
                }
            }

            // Wait for all the threads to finish so that the results list is populated.
            // If a thread is already finished when Join is called, Join will return immediately.
            int count = 0;
            foreach (Thread thread in workerThreads)
            {
                thread.Join();
                count++;
                LoggerBox.Text += "--> Files Copied: \n";
                LoggerBox.Text += String.Join(Environment.NewLine, Files_list);
                LoggerBox.Text += "\n";
                LoggerBox.Text += "--> Thread Join: " + thread.Name + "\n";
            }
            if(count!=0)
            LoggerBox.Text += "--> Number of Thread Join: "+count+"\n";
            else
                LoggerBox.Text += "--> No Thread Join \n";

            //Console.Read();
        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            Files_list.Clear();
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Multiselect = true;
            dlg.Filter = "All files (*.*)|*.*";
            Nullable<bool> result = dlg.ShowDialog(); // Display OpenFileDialog by calling ShowDialog method 
            if (result == true)            // Get the selected files name and display in a TextBox 
            {
                LoggerBox.Text += "Files Selected: \n";
                dlg.FileNames.ToList().ForEach(Files_list.Add);
                LoggerBox.Text += String.Join(Environment.NewLine, Files_list);
                LoggerBox.Text +=  "\n";
            }

            /* FolderBrowserDialog diag = new FolderBrowserDialog();
             if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
             {
                 string folder = diag.SelectedPath;  //selected folder path
             }*/
            // Set filter for file extension and default file extension
        }

        private void ButtonEject(object sender, RoutedEventArgs e)
        {
           // USBEject u = new USBEject("H:/");
            //u.Eject();
        }

        public class Device : INotifyPropertyChanged
        {
            public string Name { get; set; }

            private bool isChecked;
            public bool IsChecked
            {
                get
                {
                    return isChecked;
                }
                set
                {
                    isChecked = value;
                    NotifyPropertyChanged("IsCheck");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

    }
}



