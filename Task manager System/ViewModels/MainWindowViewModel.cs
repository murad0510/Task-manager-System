using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Threading;
using Task_manager_System.Commands;
using Task_manager_System.Models;

namespace Task_manager_System.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

        private ObservableCollection<ProgramInfo> runningPrograms = new ObservableCollection<ProgramInfo>();

        public ObservableCollection<ProgramInfo> RunningPrograms
        {
            get { return runningPrograms; }
            set { runningPrograms = value; OnPropertyChanged(); }
        }

        private ProgramInfo programInfo;

        public ProgramInfo ProgramInfo
        {
            get { return programInfo; }
            set { programInfo = value; OnPropertyChanged(); }
        }

        private string createOrEndProgramName;

        public string CreateOrEndProgramName
        {
            get { return createOrEndProgramName; }
            set { createOrEndProgramName = value; OnPropertyChanged(); }
        }


        public RelayCommand SelectionChanged { get; set; }
        public RelayCommand End { get; set; }
        public RelayCommand Create { get; set; }

        public MainWindowViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            ProgramInfo process = null;

            SelectionChanged = new RelayCommand((obj) =>
            {
                if (RunningPrograms.Count > 0)
                {
                    process = ProgramInfo;
                }
            });

            End = new RelayCommand((obj) =>
            {
                if (process != null)
                {
                    var allProcess = Process.GetProcesses();
                    for (int i = 0; i < allProcess.Length; i++)
                    {
                        if (allProcess[i].ProcessName == process.Name)
                        {
                            allProcess[i].Kill();

                        }
                    }
                }
            });

            Create = new RelayCommand((obj) =>
            {
                Process.Start($"{CreateOrEndProgramName}");
            });
        }

        private PerformanceCounter cpuCounter;
        private float GetCpuUsage(Process process)
        {
            cpuCounter.NextValue();
            //System.Threading.Thread.Sleep(10);
            return cpuCounter.NextValue() / Environment.ProcessorCount;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            RunningPrograms.Clear();
            var allProcess = Process.GetProcesses();
            for (int i = 0; i < allProcess.Length; i++)
            {
                var pragram = new ProgramInfo();
                pragram.Name = allProcess[i].ProcessName;
                var cpu = GetCpuUsage(allProcess[i]);
                //MessageBox.Show($"{allProcess[i].Threads.Count} - {allProcess[i].ProcessName}");
                pragram.CPU = cpu;
                RunningPrograms.Add(pragram);
            }
        }
    }

}
