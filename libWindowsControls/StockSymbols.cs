using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using System.Reflection;

namespace Windows.Controls.Data
{
    public class StockSymbols : ListStore
    {
        public Dictionary<string,string> Symbols { get; private set; }

        protected ConcurrentDictionary<string,string> MarketSymbols { get; private set; }

        protected Gdk.Pixbuf[] icon;
        string[] resourceName;

        public StockSymbols()
            : base(typeof(string), typeof(string), typeof(Gdk.Pixbuf))
        {            
            icon = new Gdk.Pixbuf[]
            { 
                Gdk.Pixbuf.LoadFromResource("Windows.Controls.Resources.nyse.jpg"),
                Gdk.Pixbuf.LoadFromResource("Windows.Controls.Resources.nasdaq.jpg"),
                Gdk.Pixbuf.LoadFromResource("Windows.Controls.Resources.amex.jpg")
            };

            LoadSymbols();
        }

        private void LoadSymbols()
        {
            MarketSymbols = new ConcurrentDictionary<string, string>();
            resourceName = new string[]
            { 
                "Windows.Controls.Resources.NYSE.csv",                     
                "Windows.Controls.Resources.NASDAQ.csv",
                "Windows.Controls.Resources.AMEX.csv"                                    
            };

            var asm = Assembly.GetExecutingAssembly();
            Parallel.ForEach(resourceName, (s) => ReadResourceSymbolFile(s, asm));

            if (!MarketSymbols.Any())
                return;

            Symbols = MarketSymbols.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);
            for (int i = 0; i < Symbols.Count; i++)
            {
                var itm = Symbols.ElementAt(i);
                var idx = int.Parse(itm.Value.Substring(0, 1));

                // Stripping the market index char
                var val = itm.Value.Substring(1);
                Symbols[itm.Key] = val;
                this.AppendValues(itm.Key, val, icon[idx]);
            }
        }

        private void ReadResourceSymbolFile(string embeddedresname, Assembly assembly = null)
        {
            
            if (null == assembly)
                assembly = Assembly.GetExecutingAssembly();

            var mktidx = Array.FindIndex(resourceName, x => x == embeddedresname).ToString();
            
            using (Stream stream = assembly.GetManifestResourceStream(embeddedresname))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();                        
                        var symbol = line.Split(new char[] { ',' });
                        MarketSymbols[symbol[0].Trim()] = mktidx + symbol[1].Trim();
                    }
                }
            }
        }
    }
}

