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

        private string blockProgramName;

        public string BlockProgramName
        {
            get { return blockProgramName; }
            set { blockProgramName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProgramInfo> blockedPrograms = new ObservableCollection<ProgramInfo>();

        public ObservableCollection<ProgramInfo> BlockedPrograms
        {
            get { return blockedPrograms; }
            set { blockedPrograms = value; OnPropertyChanged(); }
        }


        public RelayCommand SelectionChanged { get; set; }
        public RelayCommand End { get; set; }
        public RelayCommand Create { get; set; }
        public RelayCommand AddBlockBoxButton { get; set; }

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
                if (BlockedPrograms.Count > 0)
                {
                    for (int i = 0; i < BlockedPrograms.Count; i++)
                    {
                        if (!CreateOrEndProgramName.Contains(BlockedPrograms[i].Name.ToLower()))
                        {
                            Process.Start($"{CreateOrEndProgramName}");
                        }
                        else
                        {
                            MessageBox.Show("This program is blocked");
                        }
                    }
                }
                else
                {
                    Process.Start($"{CreateOrEndProgramName}");
                }

                CreateOrEndProgramName = string.Empty;
            });

            AddBlockBoxButton = new RelayCommand((obj) =>
            {
                bool result = false;
                for (int i = 0; i < BlockedPrograms.Count; i++)
                {
                    if (BlockedPrograms[i].Name.ToLower() == BlockProgramName.ToLower())
                    {
                        result = true;
                        break;
                    }
                }

                if (!result)
                {
                    var programInfo = new ProgramInfo();
                    programInfo.Name = BlockProgramName;
                    BlockedPrograms.Add(programInfo);

                    var allProcess = Process.GetProcesses();

                    for (int i = 0; i < allProcess.Length; i++)
                    {
                        for (int k = 0; k < BlockedPrograms.Count; k++)
                        {
                            if (allProcess[i].ProcessName.ToLower() == BlockedPrograms[k].Name.ToLower())
                            {
                                allProcess[i].Kill();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You can't add the program you added again !!!");
                }

                BlockProgramName = string.Empty;
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
                if (BlockedPrograms.Count > 0)
                {
                    for (int k = 0; k < BlockedPrograms.Count; k++)
                    {
                        if (allProcess[i].ProcessName.ToLower() == BlockedPrograms[k].Name.ToLower())
                        {
                            allProcess[i].Kill();
                        }
                        else
                        {
                            var pragram = new ProgramInfo();
                            pragram.Name = allProcess[i].ProcessName;
                            var cpu = GetCpuUsage(allProcess[i]);
                            pragram.CPU = cpu;
                            RunningPrograms.Add(pragram);
                        }
                    }
                }
                else
                {
                    var pragram = new ProgramInfo();
                    pragram.Name = allProcess[i].ProcessName;
                    var cpu = GetCpuUsage(allProcess[i]);
                    pragram.CPU = cpu;
                    RunningPrograms.Add(pragram);
                }
            }

        }
    }
}


