using System;
using System.Linq;
using Windows.Controls.Data;
using Gtk;
using static QuoteView.Properties.Settings;

namespace QuoteView
{
    public partial class PortfolioMgr : Gtk.Dialog
    {
        Gtk.Image lockedIcon;
        Gtk.Image unlockedIcon;
        Gdk.Pixbuf portfolioIcon;
        int newportfolioCounter;
        EventHandler IsDirty;

        public PortfolioMgr(Book b, Portfolio current = null)
        {
            this.Build();    
            Initialize(b, current);
        }

        void Initialize(Book b, Portfolio current = null)
        {                   
            // Initialize member variables
            book = b;
            CurrentPortfolio = current;
            newportfolioCounter = 1;
            state = WindowStateBehavior.Default;

            // Initialize widgets to the starting state
            WindowState = WindowStateBehavior.Default;

            // Images to make the gui pop...pop...pop
            lockedIcon = (Gtk.Image)btnRename.Image;
            unlockedIcon = new Gtk.Image();
            unlockedIcon.Pixbuf = Gdk.Pixbuf.LoadFromResource("QuoteView.Resources.rename-locked.png");
            portfolioIcon = Gdk.Pixbuf.LoadFromResource("QuoteView.Resources.portfolio.png");

            // Load combobox with Portfolios model
            var portfolios = new Gtk.ListStore(typeof(string), typeof(Gdk.Pixbuf));
            foreach (var pfolio in book)
                portfolios.AppendValues(pfolio.Name, portfolioIcon); 

            // Configure combobox
            var imgRndr = new CellRendererPixbuf();
            comboPortfolios.PackEnd(imgRndr, false);
            comboPortfolios.AddAttribute(imgRndr, "pixbuf", 1);  

            comboPortfolios.Model = portfolios;
            if (book.Any())
                comboPortfolios.Active = 0;
            comboPortfolios.ShowAll();

            // Setup change event on listbox
            IsDirty = (object sender, EventArgs e) => book.IsDirty = true;

            listSymbols.ListBoxChanged += IsDirty;

            WindowStateChanged(WindowStateBehavior.Default);
        }

        Book book;

        public Book Book
        {
            get
            { 
                return book;
            }

            private set
            { 
                if (null == value || value.Count == 0)
                    return;
                
                book = value;
                txtPortfolioName.Text = CurrentPortfolio?.Name ?? value[0].Name;
            }
        }

        public Portfolio CurrentPortfolio
        { 
            get
            { 
                return listSymbols.Portfolio; 
            }
            protected set
            {                 
                listSymbols.Portfolio = value;
                txtPortfolioName.Text = value?.Name;
            }
        }

        protected void OnButtonOkClicked(object sender, EventArgs e)
        {   
            var file = Default.BookFilePath;
            Book.Save(file);
        }

        protected void OnButtonCancelClicked(object sender, EventArgs e)
        {
            WindowState = WindowStateBehavior.Default;

            if (book.Any())
                comboPortfolios.Active = 0;           
        }

        protected void OnTxtPortfolioNameChanged(object sender, EventArgs e)
        {
            
        }

        protected void OnBtnNewClicked(object sender, EventArgs e)
        {
            CurrentPortfolio = new Portfolio("New Portfolio" + newportfolioCounter++);
            WindowState = WindowStateBehavior.Add;
        }

        protected void AddNewPortfolioComplete(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPortfolioName.Text) && !CurrentPortfolio.Symbols.Any())
                return;

            CurrentPortfolio.Name = txtPortfolioName.Text.Trim().ToUpperInvariant();

            // Add the new portfolio to both the combobox and book object
            var model = (Gtk.ListStore)comboPortfolios.Model;
            var itr = model.AppendValues(CurrentPortfolio.Name, portfolioIcon); 
            book.Add(CurrentPortfolio);

            // make it the active item in the combobox 
            comboPortfolios.SetActiveIter(itr);

            // unregister from listbox addbutton clicked as it WOULD be called
            // even during normal portfolio editing.
            this.listSymbols.AddButtonClicked -= this.AddNewPortfolioComplete;
            WindowState = WindowStateBehavior.Default;
        }

        enum WindowStateBehavior
        {
            Default,
            Add,
            Rename,
            Delete
        }

        WindowStateBehavior state;

        WindowStateBehavior WindowState
        { 
            get
            {
                return state;
            }
            set
            {
                if (state == value)
                    return;

                state = value;
                WindowStateChanged(state);
            }
        }

        void WindowStateChanged(WindowStateBehavior winstate)
        {
            switch (winstate)
            {
                case WindowStateBehavior.Add:
                    {
                        listSymbols.Sensitive = true;
                        buttonOk.Sensitive = false;
                        btnNew.Sensitive = false;
                        btnDelete.Sensitive = false;
                        btnRename.Sensitive = false;
                        comboPortfolios.Hide();
                        txtPortfolioName.ShowAll();                         
                        listSymbols.AddButtonClicked += this.AddNewPortfolioComplete;
                        txtPortfolioName.TooltipText = "Add one or more symbols to save";
                    }
                    break;

                case WindowStateBehavior.Rename:
                    {
                        listSymbols.Sensitive = true;
                        buttonOk.Sensitive = false;
                        btnNew.Sensitive = false;
                        btnDelete.Sensitive = false;
                        btnRename.Sensitive = true;

                        comboPortfolios.Hide();
                        txtPortfolioName.Text = CurrentPortfolio.Name;
                        txtPortfolioName.Visible = true;

                        btnRename.Image = txtPortfolioName.Visible ? lockedIcon : unlockedIcon;
                        btnRename.TooltipText = "Press again to to save name";
                        btnRename.Relief = ReliefStyle.Normal;
                        btnRename.Show();
                    }
                    break;

                case WindowStateBehavior.Delete:
                    {
                        this.btnNew.Sensitive = false;
                        this.btnDelete.Sensitive = false;
                        this.btnRename.Sensitive = false;                       
                    }
                    break;

                default:
                    {
                        listSymbols.Sensitive = book.Any();
                        this.buttonOk.Sensitive = true;
                        this.btnNew.Sensitive = book.Count < 20;
                        this.btnDelete.Sensitive = book.Any();
                        this.btnRename.Sensitive = book.Any();
                        this.comboPortfolios.ShowAll();
                        this.txtPortfolioName.Visible = false;
                        txtPortfolioName.TooltipText = string.Empty;
                        txtPortfolioName.Text = comboPortfolios.ActiveText;
                        btnRename.Image = txtPortfolioName.Visible ? lockedIcon : unlockedIcon;
                        btnRename.TooltipText = "Rename portfolio";
                        btnRename.Relief = ReliefStyle.None;
                        btnRename.Show();
                    }
                    break;
            }

        }

        protected void OnBtnDeleteClicked(object sender, EventArgs e)
        {
            WindowState = WindowStateBehavior.Delete;

            TreeIter treeitr;
            var model = (Gtk.ListStore)comboPortfolios.Model;

            // Remove from the model and the combobox will update accordingly...
            if (comboPortfolios.GetActiveIter(out treeitr))
            {
                //Remove portfolio name from combobox
                model.Remove(ref treeitr);

                // Now we remove from our book collection
                // which will take care of our main window's menu
                if (book.Contains(CurrentPortfolio))
                    book.Remove(CurrentPortfolio);

                // If we still have portfolios in the book / combobox
                // lets select the first one in the list.
                if (book.Any())
                    comboPortfolios.Active = 0;
                else
                    CurrentPortfolio = null;

                book.IsDirty = true;
            }
            else
            {
                var dlg = 
                    new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, 
                        string.Format("An error occurred while deleting the Portfolio [{0}].", CurrentPortfolio.Name));
                dlg.Run();
                dlg.Destroy();
            }

            WindowState = WindowStateBehavior.Default;
        }

        protected void OnComboPortfoliosChanged(object sender, EventArgs e)
        {
            var name = string.Empty;
            if (sender is Gtk.ComboBox)
                name = (sender as Gtk.ComboBox).ActiveText;
           
            if (string.IsNullOrEmpty(name) || !book.Exists(x => x.Name == name))
                return;

            CurrentPortfolio = book.FirstOrDefault(x => x.Name == name);
        }

        protected void OnBtnRenameClicked(object sender, EventArgs e)
        {
            TreeIter itr;
            if (WindowState == WindowStateBehavior.Rename && comboPortfolios.GetActiveIter(out itr))
            {            
                // Update the name on the portfolio object
                CurrentPortfolio.Name = txtPortfolioName.Text.Trim().ToUpperInvariant();

                // Update the name on the combobox item
                var model = (Gtk.ListStore)comboPortfolios.Model;
                model.SetValue(itr, 0, CurrentPortfolio.Name);

                // Optionally set the combobox item to renamed portfolio
                comboPortfolios.SetActiveIter(itr);

                book.IsDirty = true;

                // Return the dialog back to normal state
                WindowState = WindowStateBehavior.Default;
                return;
            }
            else if (WindowState == WindowStateBehavior.Rename)
            {
                var dlg = 
                    new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, 
                        string.Format("An error occurred while renaming the Portfolio [{0}].", CurrentPortfolio.Name));
                dlg.Run();
                dlg.Destroy();
            }

            WindowState = WindowStateBehavior.Rename;
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            this.listSymbols.AddButtonClicked -= this.AddNewPortfolioComplete;
            this.listSymbols.ListBoxChanged -= IsDirty;
        }
    }
}

