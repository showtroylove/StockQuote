using System;
using Gtk;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Windows.Controls
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ListBox : Gtk.Bin
    {
        ListStore portSymbols;
        StockSymbols mktSymbols;

        public ListBox()
        {
            this.Build();
            ConfigureListBox();
        }

        void ConfigureListBox()
        {
            portSymbols = new ListStore (typeof (string), typeof (string));
            btnAdd.Sensitive = false;
            // Listbox will display the symbol and company name
            var render = new CellRendererText[] { 
                    new CellRendererText(), 
                    new CellRendererText()
                };

            listbox.AppendColumn("Symbol", render[0], "Symbol").AddAttribute(render[0], "text", 0);        
            listbox.AppendColumn("Name", render[1], "Name").AddAttribute(render[1], "text", 1);        
            listbox.ShowAll();

            mktSymbols = new StockSymbols();
            comboboxentry.Model = mktSymbols;
            listbox.ShowAll();
            listbox.Model = portSymbols;
        }

        public void AddRange(IList<string> csvsymbol)
        {
            foreach (var i in csvsymbol)
                AddSymbol(i.Trim());
        }


        void AddSymbol(string sym)
        {
            if(!string.IsNullOrEmpty(sym) && mktSymbols.Symbols.ContainsKey(sym))
                portSymbols.AppendValues(sym, mktSymbols.Symbols[sym]);
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
            comboboxentry.Entry.Text = "Selected";
        }
    }
}

