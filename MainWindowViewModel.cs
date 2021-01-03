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
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using IBApi;
using FmpDataTool.Ib;
using System.Threading;
using IBSampleApp;

namespace FmpDataTool
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty UrlStockListProperty;
        public static readonly DependencyProperty StockListAsTextProperty;
        public static readonly DependencyProperty LogStocksProperty;
        public static readonly DependencyProperty StockListProperty;
        public static readonly DependencyProperty ProgressValueStocksProperty;
        public static readonly DependencyProperty FileNameStockListProperty;
        public static readonly DependencyProperty ConnectionStringProperty;
        public static readonly DependencyProperty BatchSizeProperty;
        public static readonly DependencyProperty UrlIncomeProperty;
        public static readonly DependencyProperty UrlBalanceProperty;
        public static readonly DependencyProperty UrlCashFlowProperty;
        public static readonly DependencyProperty SymbolListProperty;
        public static readonly DependencyProperty SymbolListAsTextProperty;
        public static readonly DependencyProperty ProgressValueBatchesProperty;
        public static readonly DependencyProperty ProgressMaxBatchesProperty;
        public static readonly DependencyProperty ProgressValueSymbolsProperty;
        public static readonly DependencyProperty ProgressMaxSymbolsProperty;
        public static readonly DependencyProperty BatchProcessInfoProperty;
        public static readonly DependencyProperty SymbolProcessInfoProperty;
        public static readonly DependencyProperty CurrentDocumentProperty;
        public static readonly DependencyProperty ErrorLogFinancialsProperty;
        public static readonly DependencyProperty PortIbProperty;
        public static readonly DependencyProperty ClientIdIbProperty;
        public static readonly DependencyProperty LogIbProperty;

        public RelayCommand CommandRequestNavigate { get; set; }
        public RelayCommand CommandGetStockList { get; set; }
        public RelayCommand CommandSelectFile { get; set; }
        public RelayCommand CommandSaveInFile { get; set; }
        public RelayCommand CommandLoadFromFile { get; set; }
        public RelayCommand CommandSaveToDatabase { get; set; }
        public RelayCommand CommandGetFinancials { get; set; }
        public RelayCommand CommandConnectToIb { get; set; }

        private DispatcherTimer timer;

        /// <summary>
        /// MainWindowViewModel - Static
        /// </summary>
        static MainWindowViewModel()
        {
            UrlStockListProperty = DependencyProperty.Register("UrlStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            StockListAsTextProperty = DependencyProperty.Register("StockListAsText", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            LogStocksProperty = DependencyProperty.Register("LogStocks", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            StockListProperty = DependencyProperty.Register("StockList", typeof(Stock[]), typeof(MainWindowViewModel), new PropertyMetadata(new Stock[0]));
            ProgressValueStocksProperty = DependencyProperty.Register("ProgressValueStocks", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            FileNameStockListProperty = DependencyProperty.Register("FileNameStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            BatchSizeProperty = DependencyProperty.Register("BatchSize", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            UrlIncomeProperty = DependencyProperty.Register("UrlIncome", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            UrlBalanceProperty = DependencyProperty.Register("UrlBalance", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            UrlCashFlowProperty = DependencyProperty.Register("UrlCashFlow", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            SymbolListProperty = DependencyProperty.Register("SymbolList", typeof(string[]), typeof(MainWindowViewModel), new PropertyMetadata(new string[0]));
            SymbolListAsTextProperty = DependencyProperty.Register("SymbolListAsText", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ProgressValueBatchesProperty = DependencyProperty.Register("ProgressValueBatches", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressMaxBatchesProperty = DependencyProperty.Register("ProgressMaxBatches", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressValueSymbolsProperty = DependencyProperty.Register("ProgressValueSymbols", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressMaxSymbolsProperty = DependencyProperty.Register("ProgressMaxSymbols", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            BatchProcessInfoProperty = DependencyProperty.Register("BatchProcessInfo", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            SymbolProcessInfoProperty = DependencyProperty.Register("SymbolProcessInfo", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            CurrentDocumentProperty = DependencyProperty.Register("CurrentDocument", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ErrorLogFinancialsProperty = DependencyProperty.Register("ErrorLogFinancials", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            PortIbProperty = DependencyProperty.Register("PortIb", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ClientIdIbProperty = DependencyProperty.Register("ClientIdIb", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            LogIbProperty = DependencyProperty.Register("LogIb", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
        }

        /// <summary>
        /// MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            UrlStockList = Configuration.Instance["UrlStockList"];
            FileNameStockList = Configuration.Instance["FileNameStockList"];
            ConnectionString = Configuration.Instance["ConnectionString"];
            BatchSize = Convert.ToInt32(Configuration.Instance["BatchSize"]);
            UrlIncome = Configuration.Instance["UrlIncome"];
            UrlBalance = Configuration.Instance["UrlBalance"];
            UrlCashFlow = Configuration.Instance["UrlCashFlow"];
            UrlList = new List<UrlAndType> {
                new UrlAndType{Url= UrlIncome, ReturnType = typeof(IncomeStatement), DocumentName= "Income Statement"},
                new UrlAndType{Url= UrlBalance, ReturnType = typeof(BalanceSheet), DocumentName= "Balance Sheet"},
                new UrlAndType{Url= UrlCashFlow, ReturnType = typeof(CashFlowStatement), DocumentName= "Cash Flow Statement"},

            };
            BatchProcessInfo = "Batches:";
            SymbolProcessInfo = "Symbols:";
            PortIb = 4001;
            ClientIdIb = 1;

            CommandRequestNavigate = new RelayCommand(p => { Process.Start(new ProcessStartInfo(((Uri)p).AbsoluteUri) { UseShellExecute = true }); });
            CommandGetStockList = new RelayCommand(async (p) => await GetStockList(p));
            CommandSelectFile = new RelayCommand((p) => SelectFile(p));
            CommandSaveInFile = new RelayCommand((p) => SaveInFile(p));
            CommandLoadFromFile = new RelayCommand((p) => LoadFromFile(p));
            CommandSaveToDatabase = new RelayCommand((p) => SaveToDatabase(p));
            CommandGetFinancials = new RelayCommand(async (p) => await GetFinancialsAsync(p));
            CommandConnectToIb = new RelayCommand((p) => ConnectToIb(p));

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 25);

            IBClient.Instance.Message += IbClient_Message;
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
        /// StockListAsText
        /// </summary>
        public string StockListAsText
        {
            get { return (string)GetValue(StockListAsTextProperty); }
            set { SetValue(StockListAsTextProperty, value); }
        }

        /// <summary>
        /// LogStocks
        /// </summary>
        public string LogStocks
        {
            get { return (string)GetValue(LogStocksProperty); }
            set { SetValue(LogStocksProperty, value); }
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
        /// SymbolList
        /// </summary>
        public string[] SymbolList
        {
            get { return (string[])GetValue(SymbolListProperty); }
            set { SetValue(SymbolListProperty, value); }
        }

        /// <summary>
        /// ProgressValueStocks
        /// </summary>
        public int ProgressValueStocks
        {
            get { return (int)GetValue(ProgressValueStocksProperty); }
            set { SetValue(ProgressValueStocksProperty, value); }
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
        /// ConnectionString
        /// </summary>
        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        /// <summary>
        /// BatchSize
        /// </summary>
        public int BatchSize
        {
            get { return (int)GetValue(BatchSizeProperty); }
            set { SetValue(BatchSizeProperty, value); }
        }

        /// <summary>
        /// UrlIncome
        /// </summary>
        public string UrlIncome
        {
            get { return (string)GetValue(UrlIncomeProperty); }
            set { SetValue(UrlIncomeProperty, value); }
        }

        /// <summary>
        /// UrlBalance
        /// </summary>
        public string UrlBalance
        {
            get { return (string)GetValue(UrlBalanceProperty); }
            set { SetValue(UrlBalanceProperty, value); }
        }

        /// <summary>
        /// UrlCashFlow
        /// </summary>
        public string UrlCashFlow
        {
            get { return (string)GetValue(UrlCashFlowProperty); }
            set { SetValue(UrlCashFlowProperty, value); }
        }

        /// <summary>
        /// SymbolListAsText
        /// </summary>
        public string SymbolListAsText
        {
            get { return (string)GetValue(SymbolListAsTextProperty); }
            set { SetValue(SymbolListAsTextProperty, value); }
        }

        /// <summary>
        /// ResponsePending
        /// </summary>
        public bool ResponsePending { get; set; }

        /// <summary>
        /// ProgressValueBatches
        /// </summary>
        public int ProgressValueBatches
        {
            get { return (int)GetValue(ProgressValueBatchesProperty); }
            set { SetValue(ProgressValueBatchesProperty, value); }
        }

        /// <summary>
        /// ProgressMaxBatches
        /// </summary>
        public int ProgressMaxBatches
        {
            get { return (int)GetValue(ProgressMaxBatchesProperty); }
            set { SetValue(ProgressMaxBatchesProperty, value); }
        }

        /// <summary>
        /// ProgressValueSymbols
        /// </summary>
        public int ProgressValueSymbols
        {
            get { return (int)GetValue(ProgressValueSymbolsProperty); }
            set { SetValue(ProgressValueSymbolsProperty, value); }
        }

        /// <summary>
        /// ProgressMaxSymbols
        /// </summary>
        public int ProgressMaxSymbols
        {
            get { return (int)GetValue(ProgressMaxSymbolsProperty); }
            set { SetValue(ProgressMaxSymbolsProperty, value); }
        }

        /// <summary>
        /// UrlList
        /// </summary>
        public List<UrlAndType> UrlList { get; private set; }

        /// <summary>
        /// BatchProcessInfo
        /// </summary>
        public string BatchProcessInfo
        {
            get { return (string)GetValue(BatchProcessInfoProperty); }
            set { SetValue(BatchProcessInfoProperty, value); }
        }

        /// <summary>
        /// SymbolProcessInfo
        /// </summary>
        public string SymbolProcessInfo
        {
            get { return (string)GetValue(SymbolProcessInfoProperty); }
            set { SetValue(SymbolProcessInfoProperty, value); }
        }

        /// <summary>
        /// CurrentDocument
        /// </summary>
        public string CurrentDocument
        {
            get { return (string)GetValue(CurrentDocumentProperty); }
            set { SetValue(CurrentDocumentProperty, value); }
        }

        /// <summary>
        /// ErrorLogFinancials
        /// </summary>
        public string ErrorLogFinancials
        {
            get { return (string)GetValue(ErrorLogFinancialsProperty); }
            set { SetValue(ErrorLogFinancialsProperty, value); }
        }

        /// <summary>
        /// PortIb
        /// </summary>
        public int PortIb
        {
            get { return (int)GetValue(PortIbProperty); }
            set { SetValue(PortIbProperty, value); }
        }

        /// <summary>
        /// ClientIdIb
        /// </summary>
        public int ClientIdIb
        {
            get { return (int)GetValue(ClientIdIbProperty); }
            set { SetValue(ClientIdIbProperty, value); }
        }

        /// <summary>
        /// LogIb
        /// </summary>
        public string LogIb
        {
            get { return (string)GetValue(LogIbProperty); }
            set { SetValue(LogIbProperty, value); }
        }

        /// <summary>
        /// GetStockList
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task GetStockList(object param)
        {
            Array.Clear(StockList, 0, StockList.Length);
            Array.Clear(SymbolList, 0, SymbolList.Length);
            StockListAsText = string.Empty;
            SymbolListAsText = string.Empty;

            LogStocks = "Requesting stock list...";
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
            SymbolList = stockList.Select(s => s.Symbol).OrderBy(u => u).ToArray<string>();
            StockListAsText = JsonSerializer.Serialize(StockList);
            SymbolListAsText = SymbolList.Aggregate((r, n) => r + Environment.NewLine + n);

            timer.Stop();
            LogStocks += "\r\nOK! stock list recieved.";
            ProgressValueStocks = 0;
        }

        /// <summary>
        /// Timer_Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            ProgressValueStocks++;
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
                MessageBoxResult messageBoxResult = MessageBox.Show("The file exists already. Do you want to overwrite it?", "Warning! File exists!", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    File.WriteAllText(FileNameStockList, StockListAsText);
                }
            }
        }

        /// <summary>
        /// LoadFromFile
        /// </summary>
        /// <param name="p"></param>
        private void LoadFromFile(object p)
        {
            if (!File.Exists(FileNameStockList))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("The file doesn't exist. Selct another one.", "Warning! File doesn't exist!", MessageBoxButton.OK);
                return;
            }

            StockListAsText = File.ReadAllText(FileNameStockList);
            StockList = JsonSerializer.Deserialize<Stock[]>(StockListAsText);
            SymbolList = StockList.Select(s => s.Symbol).OrderBy(u => u).ToArray<string>();
            SymbolListAsText = SymbolList.Aggregate((r, n) => r + Environment.NewLine + n);
        }

        /// <summary>
        /// SaveToDatabase
        /// </summary>
        /// <param name="p"></param>
        private void SaveToDatabase(object p)
        {
            if (DataContext.Instance.Stocks.Any())
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Database table 'Stocks' has already data. Do you want to overwrite it?", "Warning! Data exists!", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DataContext.Instance.Stocks.RemoveRange(DataContext.Instance.Stocks);
                }
                else
                {
                    return;
                }
            }

            LogStocks += "Saving to database...";
            DataContext.Instance.Stocks.AddRange(StockList);
            DataContext.Instance.SaveChanges();
            LogStocks += "\r\nOK! Saved to database.";

        }

        /// <summary>
        /// GetFinancials
        /// </summary>
        /// <param name="p"></param>
        private async Task GetFinancialsAsync(object p)
        {
            try
            {
                // Prepare batch calculation
                SymbolList = SymbolListAsText.Split(Environment.NewLine).Where(s => !String.IsNullOrWhiteSpace(s)).ToArray();
                int batchQuantity = SymbolList.Count() % BatchSize == 0
                    ? SymbolList.Count() / BatchSize
                    : SymbolList.Count() / BatchSize + 1;
                ProgressMaxBatches = batchQuantity;
                ProgressMaxSymbols = BatchSize;

                // For every batch
                var dataTransferId = LogTransferStart(DateTime.Now);
                Guid batchId = Guid.Empty;
                for (int batchNr = 1; batchNr <= batchQuantity; batchNr++)
                {
                    ProgressValueBatches = batchNr;
                    BatchProcessInfo = $"Batch {batchNr} of {batchQuantity}";

                    List<string> batch;
                    if (SymbolList.Skip(BatchSize * (batchNr - 1)).Any())
                    {
                        batch = SymbolList.Skip(BatchSize * (batchNr - 1)).Take(BatchSize).ToList();
                        if (batchNr > 1)
                        {
                            LogBatchEnd(batchId, DateTime.Now, batchNr - 1);
                        }
                        batchId = LogBatchStart(dataTransferId, DateTime.Now, batch.First(), batch.Last(), batchNr, batchQuantity);

                        await ProcessBatchAsync(batch);
                    }
                }

                LogBatchEnd(batchId, DateTime.Now, batchQuantity);
                LogTransferEnd(dataTransferId, DateTime.Now);
            }
            catch (Exception exception)
            {
                ErrorLogFinancials += Environment.NewLine + exception.ToString();
                throw exception;
            }
        }

        /// <summary>
        /// ProcessBatchAsync
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        private async Task ProcessBatchAsync(List<string> batch)
        {
            var symbolBefore = string.Empty;
            ProgressValueSymbols = 0;
            foreach (string symbol in batch)
            {
                ProgressValueSymbols++;
                SymbolProcessInfo = $"Symbol: {symbol}";
                while (ResponsePending)
                { }

                foreach (var urlAndType in UrlList)
                {
                    while (ResponsePending)
                    { }
                    CurrentDocument = urlAndType.DocumentName;
                    var url = urlAndType.Url.Replace("{SYMBOL}", symbol);
                    using var httpClient = new HttpClient();
                    await httpClient.GetAsync(url).ContinueWith((r) => OnRequestCompleteSwitch(r, urlAndType));
                }
                symbolBefore = symbol;
            }
        }

        /// <summary>
        /// OnRequestCompleteSwitch
        /// </summary>
        /// <param name="requestTask"></param>
        /// <param name="urlAndType"></param>
        /// <returns></returns>
        private async Task OnRequestCompleteSwitch(Task<HttpResponseMessage> requestTask, UrlAndType urlAndType)
        {
            if (urlAndType.ReturnType == typeof(IncomeStatement))
            {
                await OnRequestCompleteAsync<IncomeStatement>(requestTask);
            }
            else if (urlAndType.ReturnType == typeof(BalanceSheet))
            {
                await OnRequestCompleteAsync<BalanceSheet>(requestTask);
            }
            else if (urlAndType.ReturnType == typeof(CashFlowStatement))
            {
                await OnRequestCompleteAsync<CashFlowStatement>(requestTask);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// OnRequestIncomeCompleteAsync
        /// </summary>
        /// <param name="r"></param>
        private async Task OnRequestCompleteAsync<TEntity>(Task<HttpResponseMessage> requestTask) where TEntity : class
        {
            object lockObject = new object();

            var contentStream = await requestTask.Result.Content.ReadAsStreamAsync();
            TEntity[] financialDocument = await JsonSerializer.DeserializeAsync<TEntity[]>(contentStream);
            if (financialDocument.Any())
            {
                lock (lockObject)
                {
                    DataContext.Instance.Set<TEntity>().AddRange(financialDocument);
                    DataContext.Instance.SaveChanges();
                    Dispatcher.Invoke(() => { AfterResponseProcessed(financialDocument); });
                }
            }
            else
            {
                CurrentDocument = "No data...";
            }
        }

        /// <summary>
        /// AfterRequest
        /// </summary>
        /// <param name="financialDocument"></param>
        private void AfterResponseProcessed(object financialDocument)
        {
            ResponsePending = false;
        }

        /// <summary>
        /// ConnectToIb
        /// </summary>
        /// <param name="p"></param>
        private void ConnectToIb(object p)
        {
            IBClient.Instance.Connect(Configuration.Instance["Localhost"], PortIb, ClientIdIb);
        }

        /// <summary>
        /// IbClient_Message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void IbClient_Message(object sender, string message)
        {
            Dispatcher.Invoke(() => { LogIb += "\r\n" + message; });
        }

        /// <summary>
        /// LogTransferStart
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private Guid LogTransferStart(DateTime startTime)
        {
            var dataTranferId = Guid.NewGuid();
            DataContext.Instance.DataTransfer.Add(new DataTransfer
            {
                Id = dataTranferId,
                Start = startTime
            });
            DataContext.Instance.SaveChanges();
            return dataTranferId;
        }

        /// <summary>
        /// LogBatchStart
        /// </summary>
        /// <param name="dataTransferId"></param>
        /// <param name="now"></param>
        /// <param name="startSymbol"></param>
        /// <param name="endSymbol"></param>
        /// <param name="batchNr"></param>
        /// <param name="batchQuantity"></param>
        /// <returns></returns>
        private Guid LogBatchStart(Guid dataTransferId, DateTime now, string startSymbol, string endSymbol, int batchNr, int batchQuantity)
        {
            var batchId = Guid.NewGuid();
            DataContext.Instance.Batches.Add(new Batch
            {
                Id = batchId,
                DataTransferId = dataTransferId,
                Start = now,
                StartSymbol = startSymbol,
                EndSymbol = endSymbol
            });
            DataContext.Instance.SaveChanges();
            return batchId;
        }

        /// <summary>
        /// LogBatchEnd
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="now"></param>
        /// <param name="batchNr"></param>
        private void LogBatchEnd(Guid batchId, DateTime now, int batchNr)
        {
            DataContext.Instance.Batches.First(b => b.Id == batchId).End = now;
            DataContext.Instance.SaveChanges();
        }

        /// <summary>
        /// LogTransferEnd
        /// </summary>
        /// <param name="dataTransferId"></param>
        /// <param name="now"></param>
        private void LogTransferEnd(Guid dataTransferId, DateTime now)
        {
            DataContext.Instance.DataTransfer.First(b => b.Id == dataTransferId).End = now;
            DataContext.Instance.SaveChanges();
        }
    }
}
