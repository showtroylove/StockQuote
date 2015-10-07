using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bloomberg;
using Gdk;
using Gtk;
using Windows.Controls.Data;
using QuoteView.UI.Helpers;
using appconfig = QuoteView.Properties.Settings;

internal class QuoteStateIcons
{
    public static Gdk.Pixbuf iconUnChanged = Gdk.Pixbuf.LoadFromResource("QuoteView.Resources.coins.png");
    public static Gdk.Pixbuf iconIncreased = Gdk.Pixbuf.LoadFromResource("QuoteView.Resources.change-increase.png");
    public static Gdk.Pixbuf iconDecreased = Gdk.Pixbuf.LoadFromResource("QuoteView.Resources.change-decrease.png");
}

public partial class MainWindow: Gtk.Window
{
    Book book;
    QuoteView.PortfolioMgr dialog;
    ListStore gridModel;

    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
    {
        Build();
        BuildInternal();
    }

    #region Initialization

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Close(a);
    }

    void BuildInternal()
    {        
        // Set explicitly to prevent the flaky designer from creating a huge label
        this.grpboxLabel.HeightRequest = 17;

        // Setup gridQuotes model
        gridModel = new ListStore(typeof(Gdk.Pixbuf), typeof(string), typeof(string), typeof(string), typeof(string));

        // Setup the grid to display icon, and other columns of data returned
        // from the webservice where the quotes are retrieved.
        gridQuotes.AppendColumn(" ", new CellRendererPixbuf(), "pixbuf", 0);   
        gridQuotes.AppendColumn("Name", new CellRendererText(), "text", 1);        
        gridQuotes.AppendColumn("Symbol", new CellRendererText(), "text", 2);        
        gridQuotes.AppendColumn("Last", new CellRendererText(), "text", 3);
        gridQuotes.AppendColumn("Change", new CellRendererText(), "text", 4);

        gridQuotes.Model = gridModel;
        gridQuotes.ShowAll();

        // Loads pre-existing file called book.json 
        // from /home/{user} if not otherwise specified.
        book = Book.Load();
        if (book.Any())
        {
            // Add new menu items under Portfolios menu for each portfolio
            // read in from the file above e.g. Book.Load(file);
            UIManager.AddPortfolios(book, this.OnPortfolioActivated);

            // Select the first Portfolio in list and retrieve quote(s)
            LoadStartupQuoteOrDefaut();
        }
    }

    #endregion

    void LoadStartupQuoteOrDefaut()
    {
        var pd = appconfig.Default.StartUpQuotePortfolio;
        var p = book.Find(x => x.Name == pd.ToUpper().Trim());
        if (!string.IsNullOrEmpty(pd) && null != p)
            ActivatePortfolio(p);
    }

    void ActivatePortfolio(Portfolio p)
    {
        if (null == p)
            return;
        UIManager.ActionGroups[0].GetAction("mnu" + p.Name).Activate();
    }

    void UpdateGrid(List<StockQuote> quotes)
    {        
        gridModel.Clear();

        foreach (var q in quotes)
            gridModel.AppendValues(DetermineState(q.Change), q.Name, q.Symbol, q.Last.ToString("C"), q.Change);
        
        gridQuotes.ColumnsAutosize();
    }

    /// <summary>
    /// Method is used to help determine the icon that should be displayed
    /// when a change in quote has occurred.
    /// </summary>
    /// <returns>An image of Gdk.Pixbuf that reflects one of three states increase, decrease, unchanged.</returns>
    /// <param name="change">Change argument represents the price change that has occurred since the 
    /// last buy/sell of this security.</param>
    Gdk.Pixbuf DetermineState(string change)
    {
        var d = 0.00;
        if (!double.TryParse(change, out d))
            return QuoteStateIcons.iconUnChanged;

        return d == 0 ? QuoteStateIcons.iconUnChanged :
               d > 0 ? QuoteStateIcons.iconIncreased : QuoteStateIcons.iconDecreased;
    }

    void OnPortfolioActivated(object sender, EventArgs e)
    {
        if (!(sender is Gtk.Action))
            return;

        var s = sender as Gtk.Action;
        if (s.Label == "-DJIA" || s.Label == "-NASDAQ")
            txtSymbols.Text = s.Label;
        else
            txtSymbols.Text = book.Find(x => x.Name == s.Label).csvSymbols;

        btnQuote.Activate();
    }

    protected async void QuoteButton_OnClick(object sender, EventArgs e)
    {
        var cursor = Gdk.CursorType.LeftPtr;

        try
        {
            this.GdkWindow.Cursor = new Cursor(Gdk.CursorType.Watch); 
            btnQuote.Sensitive = false;
            txtSymbols.Sensitive = false;
            var symbols = txtSymbols.Text;

            var qs = new QuoteService();
            var quotes = qs.GetStockQuoteAsync(symbols);
            UpdateGrid(await quotes);            
        }
        finally
        {
            btnQuote.Sensitive = true;
            txtSymbols.Sensitive = true;
            this.GdkWindow.Cursor = new Cursor(cursor);
        }
    }

    protected void OnManagePortfolios(object sender, EventArgs e)
    {   
        // This is the dialog used to CRUD our Portfolios
        if (null == dialog)
            dialog = new QuoteView.PortfolioMgr(book);        

        //var dlgresult = dialog.Run();
        dialog.Run();
        dialog.Hide();

        // Update the portfolios menu, if applicable...
        //if (dlgresult == (int)ResponseType.Ok)
        if (book.IsDirty)
            UIManager.AddPortfolios(book, this.OnPortfolioActivated);

        // If a selected Portfolio is set use it
        //ActivatePortfolio(dialog.CurrentPortfolio);        
    }

    protected void OnMnuQuitActivated(object sender, EventArgs e)
    {
        Close(new DeleteEventArgs());
    }

    public void Close(DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;    

        if (null != dialog)
            dialog.Destroy();
    }
}
