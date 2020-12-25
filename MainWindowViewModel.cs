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
        public Stock[] StockList
        {
            get { return (Stock[])GetValue(StockListProperty); }
            set { SetValue(StockListProperty, value); }
        }

        private async Task GetStockList(object param)
        {
            Log += "\r\nRequesting stock list...";

            using var httpClient = new HttpClient();
            await httpClient.GetAsync(UrlStockList).ContinueWith((r) => OnRequestStockListCompleteAsync(r));
        }

        private async Task OnRequestStockListCompleteAsync(Task<HttpResponseMessage> requestTask)
        {
            HttpResponseMessage httpResponseMessage = requestTask.Result;
            var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            //using StreamReader streamReader = new StreamReader(contentStream);
            //Utf8JsonReader utf8Reader = new Utf8JsonReader(contentStream);
            //using var jsonReader = new JsonTextReader(streamReader);

            Stock[] json;

            try
            {
                json = await JsonSerializer.DeserializeAsync<Stock[]>(contentStream);
            }
            catch (Exception ex)
            {

                throw;
            }

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
