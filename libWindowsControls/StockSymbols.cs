using System;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using Gtk;
using System.Collections.Generic;

namespace Windows.Controls
{
    public class StockSymbols : ListStore
    {
        protected ConcurrentDictionary<string,string> MarketSymbols { get; private set; }
        public Dictionary<string,string> Symbols { get; private set; }

        public StockSymbols() : base(typeof(string), typeof(string))
        {
            LoadSymbols();
        }

        private void LoadSymbols()
        {
            MarketSymbols = new ConcurrentDictionary<string, string>();
            var resourceName = new string[] { 
                    "Windows.Controls.Resources.AMEX.csv", 
                    "Windows.Controls.Resources.NASDAQ.csv",
                    "Windows.Controls.Resources.NYSE.csv"                
                };

            var asm = Assembly.GetExecutingAssembly();
            Parallel.ForEach(resourceName,(s) => ReadResourceSymbolFile(s, asm));

            if (!MarketSymbols.Any())
                return;

            Symbols = MarketSymbols.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);

            foreach (var itm in Symbols)
                this.AppendValues(itm.Key, itm.Value);
        }

        private void ReadResourceSymbolFile(string resourceName, Assembly assembly = null)
        {
            if (null == assembly)
                assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var symbol = line.Replace("\"","").Split(new char[] { ',' });
                        MarketSymbols[symbol[0].Trim()] = symbol[1].Trim();
                    }
                }
            }
        }
    }
}

