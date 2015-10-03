using System;
using System.Collections.Generic;

namespace QuoteView
{
    public partial class PortfolioMgr : Gtk.Dialog
    {
        public PortfolioMgr ()
        {
            this.Build();    
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
    }
}

