using FmpDataTool.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace FmpDataTool
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject
    {
        public RelayCommand CommandRequestNavigate { get; set; }
        public static readonly DependencyProperty UrlStockListProperty;

        /// <summary>
        /// MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            CommandRequestNavigate = new RelayCommand(p => { Process.Start(new ProcessStartInfo(((Uri)p).AbsoluteUri) { UseShellExecute = true }); });
            UrlStockList = "https://financialmodelingprep.com/api/v3/stock/list?apikey=14e7a22ed6110f130afa41af05599bb6";
        }

        /// <summary>
        /// MainWindowViewModel - Static
        /// </summary>
        static MainWindowViewModel()
        {
            UrlStockListProperty = DependencyProperty.Register("UrlStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
        }



        public string UrlStockList
        {
            get { return (string)GetValue(UrlStockListProperty); }
            set { SetValue(UrlStockListProperty, value); }
        }




    }
}
