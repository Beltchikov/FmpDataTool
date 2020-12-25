using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FmpDataTool
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty UrlStockListProperty;
        public static readonly DependencyProperty ResultsStockListProperty;
        public static readonly DependencyProperty LogProperty;

        public RelayCommand CommandRequestNavigate { get; set; }
        public RelayCommand CommandGetStockList { get; set; }

        /// <summary>
        /// MainWindowViewModel - Static
        /// </summary>
        static MainWindowViewModel()
        {
            UrlStockListProperty = DependencyProperty.Register("UrlStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ResultsStockListProperty = DependencyProperty.Register("ResultsStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            LogProperty = DependencyProperty.Register("Log", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));

        }

        /// <summary>
        /// MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            UrlStockList = "https://financialmodelingprep.com/api/v3/stock/list?apikey=14e7a22ed6110f130afa41af05599bb6";

            CommandRequestNavigate = new RelayCommand(p => { Process.Start(new ProcessStartInfo(((Uri)p).AbsoluteUri) { UseShellExecute = true }); });
            CommandGetStockList = new RelayCommand(async (p) => await GetStockList(p));
        }

        public string UrlStockList
        {
            get { return (string)GetValue(UrlStockListProperty); }
            set { SetValue(UrlStockListProperty, value); }
        }

        public string ResultsStockList
        {
            get { return (string)GetValue(ResultsStockListProperty); }
            set { SetValue(ResultsStockListProperty, value); }
        }

        public string Log
        {
            get { return (string)GetValue(LogProperty); }
            set { SetValue(LogProperty, value); }
        }


        private async Task GetStockList(object param)
        {
            Log += "\r\nRequesting stock list...";

            using var httpClient = new HttpClient();
            await httpClient.GetAsync(UrlStockList).ContinueWith((r) => OnRequestStockListComplete(r));

        }

        private void OnRequestStockListComplete(Task<HttpResponseMessage> requestTask)
        {
            HttpResponseMessage httpResponseMessage = requestTask.Result;

            // TODO
            //var contentStream = await requestTask.Content.ReadAsStreamAsync();
            //ResultsStockList = response.ToString();
            Dispatcher.Invoke(SetData);
      
        }

        private void SetData()
        {
            Log += "\r\nOK! stock list recieved.";
        }

    }
}
