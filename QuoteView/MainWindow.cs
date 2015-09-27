using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bloomberg;
using Gtk;
using System.Windows.Input;
using Gdk;

public partial class MainWindow: Gtk.Window
{
    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
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
        var render = new CellRendererText[] { new CellRendererText(), 
                                              new CellRendererText(), 
                                              new CellRendererText() 
                                            };

        gridQuotes.AppendColumn("Name", render[0], "Name").AddAttribute(render[0], "text", 0);        
        gridQuotes.AppendColumn("Symbol", render[1], "Symbol").AddAttribute(render[1], "text", 1);        
        gridQuotes.AppendColumn("Last", render[2], "Last").AddAttribute(render[2], "text", 2);
		
        gridQuotes.ShowAll();
	}

    #endregion

	void UpdateGrid (List<StockQuote> quotes)
	{
        var gridModel = new ListStore (typeof (string), typeof (string), typeof (string));
        foreach (var q in quotes)
            gridModel.AppendValues(q.Name, q.Symbol, q.Last.ToString("C"));

        gridQuotes.Model = gridModel;
        gridQuotes.ColumnsAutosize();
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
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
        finally
        {
            btnQuote.Sensitive = true;
            this.GdkWindow.Cursor = new Cursor(cursor);
        }
    }
}
