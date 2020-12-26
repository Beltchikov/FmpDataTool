﻿using System;
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
using System.Windows.Threading;
using Microsoft.Win32;

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
        public static readonly DependencyProperty FileNameStockListProperty;

        public RelayCommand CommandRequestNavigate { get; set; }
        public RelayCommand CommandGetStockList { get; set; }
        public RelayCommand CommandSelectFile { get; set; }
        public RelayCommand CommandSaveInFile { get; set; }
        public RelayCommand CommandLoadFromFile { get; set; }


        private DispatcherTimer timer;

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
            FileNameStockListProperty = DependencyProperty.Register("FileNameStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
        }

        /// <summary>
        /// MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            UrlStockList = "https://financialmodelingprep.com/api/v3/stock/list?apikey=14e7a22ed6110f130afa41af05599bb6";
            FileNameStockList = @"C:\Users\Jogger\Google Drive\Wirtschaft\Trading\Aktien\stocklist.json";

            CommandRequestNavigate = new RelayCommand(p => { Process.Start(new ProcessStartInfo(((Uri)p).AbsoluteUri) { UseShellExecute = true }); });
            CommandGetStockList = new RelayCommand(async (p) => await GetStockList(p));
            CommandSelectFile = new RelayCommand((p) => SelectFile(p));
            CommandSaveInFile = new RelayCommand((p) => SaveInFile(p));
            CommandLoadFromFile = new RelayCommand((p) => throw new NotImplementedException());

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
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
        /// FileNameStockList
        /// </summary>
        public string FileNameStockList
        {
            get { return (string)GetValue(FileNameStockListProperty); }
            set { SetValue(FileNameStockListProperty, value); }
        }

        /// <summary>
        /// GetStockList
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task GetStockList(object param)
        {
            Array.Clear(StockList, 0, StockList.Length);
            ResultsStockList = string.Empty;
            Log = "Requesting stock list...";
            timer.Start();

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
            timer.Stop();
            Log += "\r\nOK! stock list recieved.";
            ProgressValue = 0;
        }

        /// <summary>
        /// Timer_Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            ProgressValue++;
        }

        /// <summary>
        /// SelectFile
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private void SelectFile(object p)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                FileNameStockList = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// SaveInFile
        /// </summary>
        /// <param name="p"></param>
        private void SaveInFile(object p)
        {
            if (File.Exists(FileNameStockList))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("The file exists allready. Do you want to overwrite it?", "Warning! File exists!", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    File.WriteAllText(FileNameStockList, ResultsStockList);
                }
            }
        }
    }
}
