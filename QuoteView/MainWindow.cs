using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bloomberg;
using Gdk;
using Gtk;
using Windows.Controls.Data;

internal class QuoteStateIcons
{
    public static Gdk.Pixbuf iconUnChanged = new Gdk.Pixbuf(Assembly.GetExecutingAssembly(), "QuoteView.Resources.coins.png");
    public static Gdk.Pixbuf iconIncreased = new Gdk.Pixbuf(Assembly.GetExecutingAssembly(), "QuoteView.Resources.coins_add.png");
    public static Gdk.Pixbuf iconDecreased = new Gdk.Pixbuf(Assembly.GetExecutingAssembly(), "QuoteView.Resources.coins_delete.png");
}

public partial class MainWindow: Gtk.Window
{
    Book book;
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        BuildInternal();
    }

    Portfolio currentPortfolio;
    public Portfolio CurrentPortfolio
    {
        get
        {
            return currentPortfolio;
        }
        set
        {
            this.txtSymbols.Text = value.csvSymbols;
            currentPortfolio = value;
        }
    }

    #region Initialization

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

	void BuildInternal()
	{
        book = Book.Load();
        if (book.Any())
            CurrentPortfolio = book[0];

        var ui = UIManager.Ui;
        var sb = new StringBuilder();
        var snip = "</menu>";

        foreach(var sym in book)
        {
            var mnu = "mnu" + sym.Name;
            var mnupf = new global::Gtk.Action(mnu, sym.Name, null, "gtk-refresh");
            mnupf.ShortLabel = sym.Name;
            UIManager.ActionGroups[0].Add(mnupf, null);
            mnupf.Activated += new global::System.EventHandler(this.OnPortfolioActivated); 
            sb.AppendFormat("<menuitem name=\"{0}\" action=\"{0}\"/>", mnu);
        }

        var idx = ui.IndexOf(snip);
        ui = ui.Insert(idx - 1, sb.ToString());
        UIManager.AddUiFromString(ui);

        this.GtkLabel1.HeightRequest = 17;
        var render = new CellRendererText[] { new CellRendererText(), 
                                              new CellRendererText(), 
                                              new CellRendererText(),
                                              new CellRendererText()
                                            };

        gridQuotes.AppendColumn(" ", new CellRendererPixbuf(), "pixbuf", 0);   
        gridQuotes.AppendColumn("Name", render[0], "text", 1);        
        gridQuotes.AppendColumn("Symbol", render[1], "text", 2);        
        gridQuotes.AppendColumn("Last", render[2], "text", 3);
        gridQuotes.AppendColumn("Change", render[3], "text", 4);
		
        gridQuotes.ShowAll();
	}

    #endregion

	void UpdateGrid (List<StockQuote> quotes)
	{
        var gridModel = new ListStore (typeof(Gdk.Pixbuf), typeof (string), typeof (string), typeof (string), typeof(string));
        foreach (var q in quotes)
        {            
            gridModel.AppendValues(DetermineState(q.Change), q.Name, q.Symbol, q.Last.ToString("C"), q.Change);
        }

        gridQuotes.Model = gridModel;
        gridQuotes.ColumnsAutosize();
	}

    Gdk.Pixbuf DetermineState(string change)
    {
        var d = 0.00;
        if(!double.TryParse(change, out d))
            return QuoteStateIcons.iconUnChanged;

        return d == 0 ? QuoteStateIcons.iconUnChanged:
               d > 0 ? QuoteStateIcons.iconIncreased: QuoteStateIcons.iconDecreased;
    }

    void OnPortfolioActivated(object sender, EventArgs e)
    {
        if (!(sender is Gtk.Action))
            return;

        var s = sender as Gtk.Action;
        CurrentPortfolio = book.Find(x => x.Name == s.Label);
    }

    protected async void QuoteButton_OnClick(object sender, EventArgs e)
    {
        var cursor = Gdk.CursorType.LeftPtr;

        try 
        {
            this.GdkWindow.Cursor = new Cursor(Gdk.CursorType.Watch); 
            btnQuote.Sensitive = false;
            var symbols = txtSymbols.Text;

            var qs = new QuoteService ();
            var quotes = qs.GetStockQuoteAsync(symbols);
            UpdateGrid(await quotes);            
        } 
        finally
        {
            btnQuote.Sensitive = true;
            this.GdkWindow.Cursor = new Cursor(cursor);
        }
    }

    protected void OnSaveActionToggled(object sender, EventArgs e)
    {        
        var symbols = this.txtSymbols.Text.Trim().ToUpper();
        var dialog = new QuoteView.PortfolioMgr();

        try
        {
            dialog.Title = "Save Portfolio";
            dialog.AddRange(symbols.Split(new char[] { ',' }));

            var dlgresult = dialog.Run();
            if(!dialog.Portfolio.Symbols.Any() || dlgresult != (int)ResponseType.Ok)
                return;
            
            book.Add(dialog.Portfolio);
            CurrentPortfolio = dialog.Portfolio;
            book.Save();
        }
        finally
        {
            dialog.Destroy();
        }
    }
 
    protected void OnAddActionToggled(object sender, EventArgs e)
    {        
        var dialog = new QuoteView.PortfolioMgr();

        try
        {
            var dlgresult = dialog.Run();
            if(!dialog.Portfolio.Symbols.Any() || dlgresult != (int)ResponseType.Ok)
                return;
            
            book.Add(dialog.Portfolio);
            CurrentPortfolio = dialog.Portfolio;
            book.Save();
        }
        finally
        {
            dialog.Destroy();
        }
    }

    public void GuiWaitCursorAction(System.Action waitcursoraction)
    {
        var cursor = Gdk.CursorType.LeftPtr;

        try
        {
            this.GdkWindow.Cursor = new Cursor(Gdk.CursorType.Watch);    
            waitcursoraction();
        }
        finally
        {
            this.GdkWindow.Cursor = new Cursor(cursor);
        }
    }

    protected void OnTxtSymbolsChanged(object sender, EventArgs e)
    {
        currentPortfolio = null;
    }
}
