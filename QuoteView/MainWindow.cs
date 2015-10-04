using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Bloomberg;
using Gdk;
using Gtk;
using Windows.Controls.Data;
using System.Reflection;

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

    #region Initialization

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

	void BuildInternal()
	{
        book = new Book();
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
        return d > 0 ? QuoteStateIcons.iconIncreased: QuoteStateIcons.iconDecreased;
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
        catch(Exception ex)
        {
            var md = new MessageDialog (null, 
                                        DialogFlags.DestroyWithParent,
                                        MessageType.Error, 
                                        ButtonsType.Ok, ex.Message);
            md.Run ();
            md.Destroy();
        }
        finally
        {
            btnQuote.Sensitive = true;
            this.GdkWindow.Cursor = new Cursor(cursor);
        }
    }

    protected void OnSaveActionToggled(object sender, EventArgs e)
    {        
        var symbols = this.txtSymbols.Text;
        var dialog = new QuoteView.PortfolioMgr();

        try
        {
            dialog.Title = "Save Portfolio";
            dialog.AddRange(symbols.Split(new char[] { ',' }));
            var dlgresult = dialog.Run();
            if(!dialog.Portfolio.Symbols.Any() || dlgresult != (int)ResponseType.Ok)
                return;
            
            book.Add(dialog.Portfolio);
            this.txtSymbols.Text = dialog.Portfolio.csvSymbols;
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
            this.txtSymbols.Text = dialog.Portfolio.csvSymbols;
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
}
