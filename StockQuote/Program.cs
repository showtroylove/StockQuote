﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Bloomberg;
using static System.Console;

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
				WriteLine (ex.Message);
			}
        }

        static void DisplayQuotes(List<StockQuote> quotes)
        {
            foreach (var result in quotes)
            {
                var last = 0.0;
                var qresult = double.TryParse(result.Last, out last);
                var qlast = qresult ? last.ToString("C") : "N/A";
                WriteLine("{0, -40} | Symbol [{1, -6}] | Last [{2, -10}] | Elapsed time: [{3}ms]",
                    string.IsNullOrEmpty(result.Name) ? "Market Quote " : result.Name, result.Symbol, qlast, result.ElapsedMilliseconds);            
            }
        }

        static void StartQuoteBox()
        {
            var qs = new QuoteService();
            var someValue = "-q";
            do
            {
				WriteLine("Enter one or more comma seperated stock symbol(s) for quote(s)\nor -nasdaq | -djia to use a default symbol set\nor type -q to exit.");
				someValue = ReadLine();
                
                if (someValue != "-q" && !string.IsNullOrEmpty(someValue))
                {
                    var quotes = qs.GetStockQuoteAsync(someValue, false);
                    DisplayQuotes(quotes.Result);      
                }

            } while (someValue != "-q");		

			WriteLine("Exiting QuoteBox...");
        }		

        static void SQLServerConnect()
        {
            const string BloombergCS = "Data Source=192.168.1.73;Initial Catalog=QuoteApp.ReutersDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;";
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
