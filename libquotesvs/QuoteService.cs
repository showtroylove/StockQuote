using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Bloomberg
{
    public class QuoteService
    {
        IList<string> fqQuotes;
        IList<string> tmpQuotes;

        public QuoteService()
        {
            fqQuotes = new List<string>(){ "BAC", "C", "CS", "GS", "JPM", "MS" };
        }

        protected string[] AddGroups(string csvsymbols)
        {
            var symbols = csvsymbols.Split(new char[] { ',' }).Select(q => q.Trim().ToUpper()).Where(x => !x.StartsWith("-DJIA") && !x.StartsWith("-NASDAQ"));

            if (csvsymbols.ToUpper().Contains("-DJIA"))
            {      
                symbols = symbols.Concat(SymbolLibrary.DJIA30);
            }

            if (csvsymbols.ToUpper().Contains("-NASDAQ"))
            {
                symbols = symbols.Concat(SymbolLibrary.NASDAQ);
            }

            return symbols.ToArray();
        }

        protected Task<IList<string>> ParseQuoteListAsync(string csvsymbols = "-DJIA", bool preserve = true)
        {
            return Task.Run(() =>
                {
                    var trimmedcapitalizedquotes = AddGroups(csvsymbols);
                    if (preserve)
                    {
                        fqQuotes = trimmedcapitalizedquotes;
                        if(null != tmpQuotes)
                            tmpQuotes.Clear();
                        return fqQuotes;
                    }
                    else
                    {
                        tmpQuotes = trimmedcapitalizedquotes;
                        return tmpQuotes;
                    }
                });
        }

        public virtual Task<List<StockQuote>> GetStockQuoteAsync(string csvsymbols = "-DJIA", bool preserve = true, int timeoutInMinutes = 5)
        {
            return Task.Run(async () =>
            {
                var quotes = new List<StockQuote>();                
                var symbols = await ParseQuoteListAsync(csvsymbols, preserve);

                foreach (var s in symbols)
                {
                    try 
                    {
                        var result = GetStockQuote(s, timeoutInMinutes);
                        if(null != result)
                            quotes.Add(result);
                    } 
                    catch (Exception ex) 
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }

                return quotes;
            });
        }

        public StockQuote GetStockQuote(string symbol, int timeoutInMinutes = 3)
        {
            StockQuote quote = null;
            var sw = new Stopwatch();     

            var quotesvs = new StockQuoteServiceClient();
            quotesvs.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(timeoutInMinutes);

            sw.Start();  
            quote = quotesvs.GetStockQuote(symbol.Trim());
            sw.Stop();

            if(null != quote)
                quote.ElapsedMilliseconds = sw.ElapsedMilliseconds.ToString();            

            return quote;
        }
    }
}

