using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using FmpDataTool.Model;
using System.Linq;

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
        public static readonly DependencyProperty StockListProperty;
        public static readonly DependencyProperty ProgressValueProperty;

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
            StockListProperty = DependencyProperty.Register("StockList", typeof(Stock[]), typeof(MainWindowViewModel), new PropertyMetadata(new Stock[0]));
            ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
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

        /// <summary>
        /// UrlStockList
        /// </summary>
        public string UrlStockList
        {
            get { return (string)GetValue(UrlStockListProperty); }
            set { SetValue(UrlStockListProperty, value); }
        }

        /// <summary>
        /// ResultsStockList
        /// </summary>
        public string ResultsStockList
        {
            get { return (string)GetValue(ResultsStockListProperty); }
            set { SetValue(ResultsStockListProperty, value); }
        }

        /// <summary>
        /// Log
        /// </summary>
        public string Log
        {
            get { return (string)GetValue(LogProperty); }
            set { SetValue(LogProperty, value); }
        }

        /// <summary>
        /// StockList
        /// </summary>
        public Stock[] StockList
        {
            get { return (Stock[])GetValue(StockListProperty); }
            set { SetValue(StockListProperty, value); }
        }

        /// <summary>
        /// ProgressValue
        /// </summary>
        public int ProgressValue
        {
            get { return (int)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        /// <summary>
        /// GetStockList
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task GetStockList(object param)
        {
            Log += "\r\nRequesting stock list...";

            using var httpClient = new HttpClient();
            await httpClient.GetAsync(UrlStockList).ContinueWith((r) => OnRequestStockListCompleteAsync(r));
        }

        /// <summary>
        /// OnRequestStockListCompleteAsync
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        private async Task OnRequestStockListCompleteAsync(Task<HttpResponseMessage> requestTask)
        {
            var contentStream = await requestTask.Result.Content.ReadAsStreamAsync();
            Stock[] stockList = await JsonSerializer.DeserializeAsync<Stock[]>(contentStream);
            Dispatcher.Invoke(() => SetDataStockList(stockList));
        }

        /// <summary>
        /// SetDataStockList
        /// </summary>
        /// <param name="stockList"></param>
        private void SetDataStockList(Stock[] stockList)
        {
            StockList = stockList;
            ResultsStockList = JsonSerializer.Serialize(StockList);
            Log += "\r\nOK! stock list recieved.";
        }
    }
}
