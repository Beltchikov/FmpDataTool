using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace FmpDataTool
{
    public class MainWindowViewModel : DependencyObject
    {
        public RelayCommand CommandRequestNavigate { get; set; }

        public MainWindowViewModel()
        {
            CommandRequestNavigate = new RelayCommand(p => { Process.Start(new ProcessStartInfo(((Uri)p).AbsoluteUri) { UseShellExecute = true }); });
        }


    }
}
