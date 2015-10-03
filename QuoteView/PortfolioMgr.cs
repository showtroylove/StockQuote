using System;
using System.Collections.Generic;
using Windows.Controls.Data;

namespace QuoteView
{
    public partial class PortfolioMgr : Gtk.Dialog
    {
        public PortfolioMgr ()
        {
            this.Build();    
        }

        public Portfolio Portfolio
        {
            get{ return listbox2.Portfolio;}
        }

        public void AddRange(IList<string> csvsymbol) 
        {
            listbox2.AddRange(csvsymbol);
        }

        protected void OnButtonOkClicked(object sender, EventArgs e)
        {            
        }

        protected void OnButtonCancelClicked(object sender, EventArgs e)
        {
        }

        protected void OnTxtPortfolioNameEditingDone(object sender, EventArgs e)
        {
            listbox2.Portfolio.Name = txtPortfolioName.Text;
        }
    }
}

