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
using FmpDataContext;
using FmpDataContext.Model;
using FmpDataContext.StockList;
using System.Collections.ObjectModel;
using FmpDataContext.SymbolDateAndDocs;

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
        public static readonly DependencyProperty ProgressValueStocksProperty;
        public static readonly DependencyProperty FileNameStockListProperty;
        public static readonly DependencyProperty ConnectionStringProperty;
        public static readonly DependencyProperty BatchSizeProperty;
        public static readonly DependencyProperty UrlIncomeProperty;
        public static readonly DependencyProperty UrlBalanceProperty;
        public static readonly DependencyProperty UrlCashFlowProperty;
        public static readonly DependencyProperty SymbolListAsTextProperty;
        public static readonly DependencyProperty ProgressValueBatchesProperty;
        public static readonly DependencyProperty ProgressMaxBatchesProperty;
        public static readonly DependencyProperty ProgressValueSymbolsProperty;
        public static readonly DependencyProperty ProgressMaxSymbolsProperty;
        public static readonly DependencyProperty BatchProcessInfoProperty;
        public static readonly DependencyProperty SymbolProcessInfoProperty;
        public static readonly DependencyProperty CurrentDocumentProperty;
        public static readonly DependencyProperty ErrorLogFinancialsProperty;
        public static readonly DependencyProperty StocksRecievedProperty;
        public static readonly DependencyProperty SymbolCountProperty;

        public RelayCommand CommandRequestNavigate { get; set; }
        public RelayCommand CommandGetStockList { get; set; }
        public RelayCommand CommandSelectFile { get; set; }
        public RelayCommand CommandSaveInFile { get; set; }
        public RelayCommand CommandLoadFromFile { get; set; }
        public RelayCommand CommandSaveToDatabase { get; set; }
        public RelayCommand CommandGetFinancials { get; set; }

        private DataContext DataContext { get; set; }

        private DispatcherTimer timer;

        private SymbolDateAndDocsList _symbolDateAndDocsList;

        /// <summary>
        /// MainWindowViewModel - Static
        /// </summary>
        static MainWindowViewModel()
        {
            UrlStockListProperty = DependencyProperty.Register("UrlStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            StockListAsTextProperty = DependencyProperty.Register("StockListAsText", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            LogStocksProperty = DependencyProperty.Register("LogStocks", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ProgressValueStocksProperty = DependencyProperty.Register("ProgressValueStocks", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            FileNameStockListProperty = DependencyProperty.Register("FileNameStockList", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            BatchSizeProperty = DependencyProperty.Register("BatchSize", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            UrlIncomeProperty = DependencyProperty.Register("UrlIncome", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            UrlBalanceProperty = DependencyProperty.Register("UrlBalance", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            UrlCashFlowProperty = DependencyProperty.Register("UrlCashFlow", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            SymbolListAsTextProperty = DependencyProperty.Register("SymbolListAsText", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ProgressValueBatchesProperty = DependencyProperty.Register("ProgressValueBatches", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressMaxBatchesProperty = DependencyProperty.Register("ProgressMaxBatches", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressValueSymbolsProperty = DependencyProperty.Register("ProgressValueSymbols", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressMaxSymbolsProperty = DependencyProperty.Register("ProgressMaxSymbols", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            BatchProcessInfoProperty = DependencyProperty.Register("BatchProcessInfo", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            SymbolProcessInfoProperty = DependencyProperty.Register("SymbolProcessInfo", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            CurrentDocumentProperty = DependencyProperty.Register("CurrentDocument", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ErrorLogFinancialsProperty = DependencyProperty.Register("ErrorLogFinancials", typeof(ObservableCollection<string>), typeof(MainWindowViewModel), new PropertyMetadata(new ObservableCollection<string>()));
            StocksRecievedProperty = DependencyProperty.Register("StocksRecieved", typeof(StocksRecieved), typeof(MainWindowViewModel), new PropertyMetadata(default(StocksRecieved)));
            SymbolCountProperty = DependencyProperty.Register("SymbolCount", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
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

            CommandRequestNavigate = new RelayCommand(p => { Process.Start(new ProcessStartInfo(((Uri)p).AbsoluteUri) { UseShellExecute = true }); });
            CommandGetStockList = new RelayCommand(async (p) => await GetStockList(p));
            CommandSelectFile = new RelayCommand((p) => SelectFile(p));
            CommandSaveInFile = new RelayCommand((p) => SaveInFile(p));
            CommandLoadFromFile = new RelayCommand((p) => LoadFromFile(p));
            CommandSaveToDatabase = new RelayCommand((p) => SaveToDatabase(p));
            CommandGetFinancials = new RelayCommand(async (p) => await GetFinancialsAsync(p));

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 25);

            DataContext = DataContext.Instance(Configuration.Instance["ConnectionString"]);
            _symbolDateAndDocsList = new SymbolDateAndDocsList(DataContext);
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
        /// StocksRecieved
        /// </summary>
        public StocksRecieved StocksRecieved
        {
            get { return (StocksRecieved)GetValue(StocksRecievedProperty); }
            set { SetValue(StocksRecievedProperty, value); }
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
        public ObservableCollection<string> ErrorLogFinancials
        {
            get { return (ObservableCollection<string>)GetValue(ErrorLogFinancialsProperty); }
            set { SetValue(ErrorLogFinancialsProperty, value); }
        }

        /// <summary>
        /// SymbolCount
        /// </summary>
        public int SymbolCount
        {
            get { return (int)GetValue(SymbolCountProperty); }
            set { SetValue(SymbolCountProperty, value); }
        }

        /// <summary>
        /// GetStockList
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task GetStockList(object param)
        {
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
            List<string> years = Configuration.Instance["Years"].Split(",").Select(s => s.Trim()).ToList();
            List<string> dates = Configuration.Instance["Dates"].Split(",").Select(s => s.Trim()).ToList();
            StocksRecieved = new StocksRecieved(stockList.ToList(), years, dates, DataContext);
            StockListAsText = StocksRecieved.AsJson;

            var docsMissingNoImportError = StocksRecieved.Cleaned.Distinct.DocsMissingNoImportError;
            docsMissingNoImportError = ExcludeSymbolsWithDot(docsMissingNoImportError);
            SymbolListAsText = docsMissingNoImportError.SymbolsTop100AsText;

            SymbolCount = docsMissingNoImportError.ToList().Count();

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
            var stockList = JsonSerializer.Deserialize<Stock[]>(StockListAsText);

            List<string> years = Configuration.Instance["Years"].Split(",").Select(s => s.Trim()).ToList();
            List<string> dates = Configuration.Instance["Dates"].Split(",").Select(s => s.Trim()).ToList();
            StocksRecieved = new StocksRecieved(stockList.ToList(), years, dates, DataContext);
            StockListAsText = StocksRecieved.AsJson;

            var docsMissingNoImportError = StocksRecieved.Cleaned.Distinct.DocsMissingNoImportError;
            docsMissingNoImportError = ExcludeSymbolsWithDot(docsMissingNoImportError);
            SymbolListAsText = docsMissingNoImportError.SymbolsTop100AsText;
            SymbolCount = docsMissingNoImportError.ToList().Count();
        }

        /// <summary>
        /// SaveToDatabase
        /// </summary>
        /// <param name="p"></param>
        private void SaveToDatabase(object p)
        {
            if (!OverwriteDataConfirmations())
            {
                LogStocks += "\r\nCancelled!";
                return;
            }

            var stocksCleaned = StocksRecieved.Cleaned.Distinct.ToList();
            var fmpSymbolCompanyList = StocksRecieved.Cleaned.SymbolCompany;

            LogStocks += "\r\nSaving to database...";
            DataContext.Stocks.AddRange(stocksCleaned);
            DataContext.FmpSymbolCompany.AddRange(fmpSymbolCompanyList);
            DataContext.SaveChanges();
            LogStocks += "\r\nOK! Saved to database.";

        }

        /// <summary>
        /// OverwriteDataConfirmation
        /// </summary>
        /// <returns></returns>
        private bool OverwriteDataConfirmations()
        {
            if (!OverwriteDataConfirmation(DataContext.Stocks, "Stocks"))
            {
                return false;
            }

            if (!OverwriteDataConfirmation(DataContext.FmpSymbolCompany, "FmpSymbolCompany"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// OverwriteDataConfirmation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool OverwriteDataConfirmation<T>(DbSet<T> table, string tableName) where T : class
        {
            if (table.Any())
            {
                MessageBoxResult messageBoxResult = MessageBox.Show($"Database table '{tableName}' has already data. Do you want to overwrite it?", "Warning! Data exists!", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    table.RemoveRange(table);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
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
                var docsMissingNoImportError = StocksRecieved.Cleaned.Distinct.DocsMissingNoImportError;
                docsMissingNoImportError = ExcludeSymbolsWithDot(docsMissingNoImportError);
                var symbolsToProcess = docsMissingNoImportError.Symbols;

                int batchQuantity = symbolsToProcess.Count() % BatchSize == 0
                    ? symbolsToProcess.Count() / BatchSize
                    : symbolsToProcess.Count() / BatchSize + 1;
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
                    if (symbolsToProcess.Skip(BatchSize * (batchNr - 1)).Any())
                    {
                        batch = symbolsToProcess.Skip(BatchSize * (batchNr - 1)).Take(BatchSize).ToList();
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
                ErrorLogFinancials.Add(exception.ToString());
                throw;
            }
        }

        /// <summary>
        /// ExcludeSymbolsWithDot
        /// </summary>
        /// <param name="inputStockList"></param>
        /// <returns></returns>
        private StockListBase ExcludeSymbolsWithDot(StockListBase inputStockList)
        {
            var outputStockList = inputStockList.ToList().Where(sl => !sl.Symbol.Contains(".")).ToList();
            return new StockListBase(outputStockList, inputStockList.Years, inputStockList.Dates, inputStockList.DataContext);
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

                foreach (var urlAndTypeTemplate in UrlList)
                {
                    var urlAndType = urlAndTypeTemplate.Copy();
                    while (ResponsePending)
                    { }
                    CurrentDocument = urlAndType.DocumentName;
                    urlAndType.Url = urlAndType.Url.Replace("{SYMBOL}", symbol);
                    using var httpClient = new HttpClient();
                    await httpClient.GetAsync(urlAndType.Url).ContinueWith((r) => OnRequestCompleteSwitch(r, urlAndType));
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
                await OnRequestCompleteAsync<IncomeStatement>(requestTask, urlAndType);
            }
            else if (urlAndType.ReturnType == typeof(BalanceSheet))
            {
                await OnRequestCompleteAsync<BalanceSheet>(requestTask, urlAndType);
            }
            else if (urlAndType.ReturnType == typeof(CashFlowStatement))
            {
                await OnRequestCompleteAsync<CashFlowStatement>(requestTask, urlAndType);
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
        private async Task OnRequestCompleteAsync<TEntity>(Task<HttpResponseMessage> requestTask, UrlAndType urlAndType) where TEntity : class
        {
            object lockObject = new object();

            var contentStream = await requestTask.Result.Content.ReadAsStreamAsync();
            TEntity[] financialDocuments;
            try
            {
                financialDocuments = await JsonSerializer.DeserializeAsync<TEntity[]>(contentStream);
                if (financialDocuments.Any())
                {
                    foreach (var document in financialDocuments)
                    {
                        _symbolDateAndDocsList.Add(document);
                    }

                    lock (lockObject)
                    {
                        foreach (SymbolDateAndDocs completed in _symbolDateAndDocsList.CompletedButNotSaved.Where(s => !s.PersistenceFailed))
                        {
                            var saveResult = completed.SaveInDatabase();
                            if (!string.IsNullOrWhiteSpace(saveResult))
                            {
                                Dispatcher.Invoke(() => { ErrorLogFinancials.Add(saveResult); });
                            }
                        }

                        Dispatcher.Invoke(() => { AfterResponseProcessed(financialDocuments); });
                    }
                }
                else
                {
                    Dispatcher.Invoke(() => {
                        ImportErrorFmpSymbol(urlAndType.Symbol);
                        CurrentDocument = "No data..."; 
                    });
                }
            }
            catch (Exception exception)
            {
                Dispatcher.Invoke(() => {
                    ImportErrorFmpSymbol(urlAndType.Symbol);
                    ErrorLogFinancials.Add(exception.ToString()); 
                });
            }
        }

        /// <summary>
        /// ImportErrorFmpSymbol
        /// </summary>
        /// <param name="symbol"></param>
        private void ImportErrorFmpSymbol(string symbol)
        {
            if (!DataContext.ImportErrorFmpSymbol.Any(e => e.Symbol == symbol))
            {
                DataContext.ImportErrorFmpSymbol.Add(new ImportErrorFmpSymbol { Symbol = symbol, Timestamp = DateTime.Now });
                DataContext.SaveChanges();
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
        /// LogTransferStart
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private Guid LogTransferStart(DateTime startTime)
        {
            var dataTranferId = Guid.NewGuid();
            DataContext.DataTransfer.Add(new DataTransfer
            {
                Id = dataTranferId,
                Start = startTime
            }); ;
            DataContext.SaveChanges();
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
            DataContext.Batches.Add(new Batch
            {
                Id = batchId,
                DataTransferId = dataTransferId,
                Start = now,
                StartSymbol = startSymbol,
                EndSymbol = endSymbol
            });
            DataContext.SaveChanges();
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
            DataContext.Batches.First(b => b.Id == batchId).End = now;
            DataContext.SaveChanges();
        }

        /// <summary>
        /// LogTransferEnd
        /// </summary>
        /// <param name="dataTransferId"></param>
        /// <param name="now"></param>
        private void LogTransferEnd(Guid dataTransferId, DateTime now)
        {
            DataContext.DataTransfer.First(b => b.Id == dataTransferId).End = now;
            DataContext.SaveChanges();
        }
    }
}
