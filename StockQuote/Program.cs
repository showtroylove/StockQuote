using System.Collections.Generic;
using System.Linq;
using System;
using Bloomberg;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace StockQuoteApp
{    
    class Program
    {        

        static void Main(string[] args)
        {
			try
            {
                //SQLServerConnect();
                StartQuoteBox();                
            }
			catch(Exception ex) 
			{
				Console.WriteLine (ex.Message);
			}
        }

        static void DisplayQuotes(List<StockQuote> quotes)
        {
            foreach (var result in quotes)
            {
                var last = 0.0;
                var qresult = double.TryParse(result.Last, out last);
                var qlast = qresult ? last.ToString("C") : "N/A";
                Console.WriteLine("{0, -40} | Symbol [{1, -6}] | Last [{2, -10}] | Elapsed time: [{3}ms]",
                    string.IsNullOrEmpty(result.Name) ? "Market Quote " : result.Name, result.Symbol, qlast, result.ElapsedMilliseconds);            
            }
        }

        static void StartQuoteBox()
        {
            var qs = new QuoteService();
            var someValue = "-q";
            do
            {
				Console.WriteLine("Enter one or more comma seperated stock symbol(s) for quote(s)\nor -nasdaq | -djia to use a default symbol set\nor type -q to exit.");
				someValue = Console.ReadLine();
                
                if (someValue != "-q" && !string.IsNullOrEmpty(someValue))
                {
                    var quotes = qs.GetStockQuoteAsync(someValue, false);
                    DisplayQuotes(quotes.Result);      
                }

            } while (someValue != "-q");		

			Console.WriteLine("Exiting QuoteBox...");
        }		

        static void SQLServerConnect()
        {
            const string BloombergCS = "Data Source=192.168.1.73,1433;Initial Catalog=QuoteApp.ReutersDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;";
            using (var conn = new SqlConnection(BloombergCS))
            {
                var ds = new DataSet();
                var cmd = new SqlCommand("Select * From Companies", conn);
                var da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Companies");
                var c = ds.Tables.Count;
            }
        }
    }
}
