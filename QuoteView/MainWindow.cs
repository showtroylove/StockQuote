using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bloomberg;
using Gtk;
using System.Windows.Input;
using Gdk;
using System.Linq;

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
        this.GtkLabel1.HeightRequest = 17;
        var render = new CellRendererText[] { new CellRendererText(), 
                                              new CellRendererText(), 
                                              new CellRendererText(),
                                              new CellRendererText()
                                            };

        gridQuotes.AppendColumn("Name", render[0], "Name").AddAttribute(render[0], "text", 0);        
        gridQuotes.AppendColumn("Symbol", render[1], "Symbol").AddAttribute(render[1], "text", 1);        
        gridQuotes.AppendColumn("Last", render[2], "Last").AddAttribute(render[2], "text", 2);
        gridQuotes.AppendColumn("Change", render[3], "Change").AddAttribute(render[3], "text", 3);
		
        gridQuotes.ShowAll();
	}

    #endregion

	void UpdateGrid (List<StockQuote> quotes)
	{
        var gridModel = new ListStore (typeof (string), typeof (string), typeof (string), typeof(string));
        foreach (var q in quotes)
            gridModel.AppendValues(q.Name, q.Symbol, q.Last.ToString("C"), q.Change);

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
        //this.txtSymbols.Text = "BAC,C,CS,GS,JPM,MS";

        var symbols = this.txtSymbols.Text;
        var dialog = new QuoteView.PortfolioMgr();
        dialog.Modal = true;
        dialog.AddRange(symbols.Split(new char[] {','}));
        dialog.Run();
        dialog.Destroy();
    }
 
    protected void OnAddActionToggled(object sender, EventArgs e)
    {
        //this.txtSymbols.Text = "RTN,LMT, MSFT,GOOG,GOOGL,AAPL,IBM,CSCO";
        var dialog = new QuoteView.PortfolioMgr();
        dialog.Modal = true;
        dialog.Run();
        dialog.Destroy();
    }
}
