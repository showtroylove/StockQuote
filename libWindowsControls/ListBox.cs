﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using Windows.Controls.Data;
using System.Reflection;

namespace Windows.Controls
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ListBox : Gtk.Bin
    {
        ListStore portSymbols;
        StockSymbols mktSymbols;
        Dictionary<string,string> builtinPortfolios;
        Gdk.Pixbuf icon;

        public ListBox()
        {
            this.Build();
            ConfigureListBox();
        }

        void ConfigureListBox()
        {
            // class member init
            builtinPortfolios = 
                new Dictionary<string,string> {{"-DJIA", "Dow Jones Industrial Avg"}, {"-NASDAQ", "All NASDAQ Market Symbols"}};
            icon = new Gdk.Pixbuf(Assembly.GetExecutingAssembly(), "Windows.Controls.Resources.meeting-observer.png");
            mktSymbols = new StockSymbols();
            portSymbols = new ListStore (typeof(Gdk.Pixbuf), typeof (string), typeof (string));
            portfolio = new Portfolio();

            // local var init
            var imgRndr = new CellRendererPixbuf();
            var render = new CellRendererText[] { new CellRendererText(), new CellRendererText()};

            // Disable the add symbol button (+)
            btnAdd.Sensitive = false;

            // ComboBoxEntry will display the symbol and company name
            // symbol is being used by default as the control takes the first
            // colum in the model by default.
            comboboxentry.TextColumn = 0;

            // Image
            comboboxentry.PackStart(imgRndr, false);
            comboboxentry.AddAttribute(imgRndr, "pixbuf", 2);        

            // Symbol (appears to load these by default).
            // comboboxentry.PackStart(render[0], true);
            // comboboxentry.AddAttribute(render[0], "text", 0);        

            // Company Name
            comboboxentry.PackStart(render[1], true);
            comboboxentry.AddAttribute(render[1], "text", 1);        

            // Apply / Bind the data the combobox
            comboboxentry.Model = mktSymbols;

            // Listbox will display the symbol and company name
            listbox.AppendColumn("-", imgRndr, "pixbuf", 0);        
            listbox.AppendColumn("Symbol", render[0], "text", 1);        
            listbox.AppendColumn("Name", render[1], "text", 2);    
            listbox.Model = portSymbols;
            listbox.ShowAll();
        }

        Portfolio portfolio;
        public Portfolio Portfolio
        {
            get
            {
                return portfolio;
            }
            set
            {
                if (!value.Symbols.Any())
                    return;

                portSymbols.Clear();
                portfolio.Symbols.Clear();
                AddRange(value.Symbols);
                portfolio = value;
            }
        }

        public void AddRange(IList<string> csvsymbol)
        {
            foreach (var i in csvsymbol)
                AddSymbol(i.Trim());
        }

        public void AddSymbol(string sym)
        {
            sym = sym.Trim().ToUpper();

            if (!string.IsNullOrEmpty(sym) && mktSymbols.Symbols.ContainsKey(sym))
                portSymbols.AppendValues(icon, sym, mktSymbols.Symbols[sym]);
            else if (builtinPortfolios.ContainsKey(sym))
                portSymbols.AppendValues(icon, sym, builtinPortfolios[sym]);
            else
                return;

            //Maintain list of symbols for caller.
            if(!Portfolio.Symbols.Contains(sym))
                Portfolio.Symbols.Add(sym);
            listbox.ColumnsAutosize();
            comboboxentry.Entry.Text = string.Empty;
        }

        protected void OnBtnAddClicked(object sender, EventArgs e)
        {
            AddSymbol(comboboxentry.ActiveText);
            OnAddButtonClicked(sender, e);
        }

        public event EventHandler AddButtonClicked;
        protected void OnAddButtonClicked(object sender, EventArgs e)
        {
            if (null != AddButtonClicked)
                AddButtonClicked(sender, e);
        }

        protected void OnComboboxentryChanged(object sender, EventArgs e)
        {
            btnAdd.Sensitive = !string.IsNullOrEmpty(comboboxentry.ActiveText);
        }

        protected void OnListboxKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key != Gdk.Key.Delete)
                return;

            var tp = listbox.Selection.GetSelectedRows();

            foreach (var path in tp)
            {
                TreeIter itr;
                if (!portSymbols.GetIter(out itr, path))
                    continue;

                var symbol = portSymbols.GetValue(itr, 1);
                portSymbols.Remove(ref itr);
                
                if (symbol is string)
                    Portfolio.Symbols.Remove(symbol.ToString());                            
            }
        }
    }
}

