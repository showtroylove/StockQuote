
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	
	private global::Gtk.Action FileAction;
	
	private global::Gtk.Action addAction;
	
	private global::Gtk.Action saveAction;
	
	private global::Gtk.Action _buggy;
	
	private global::Gtk.VBox vbox2;
	
	private global::Gtk.MenuBar menubar1;
	
	private global::Gtk.Frame frame2;
	
	private global::Gtk.Alignment GtkAlignment;
	
	private global::Gtk.VBox vbox4;
	
	private global::Gtk.Entry txtSymbols;
	
	private global::Gtk.Button btnQuote;
	
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	
	private global::Gtk.TreeView gridQuotes;
	
	private global::Gtk.Label GtkLabel1;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("_File"), null, null);
		this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_File");
		w1.Add (this.FileAction, null);
		this.addAction = new global::Gtk.Action ("addAction", global::Mono.Unix.Catalog.GetString ("_Add Portfolio"), null, "gtk-add");
		this.addAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Add Portfolio");
		w1.Add (this.addAction, null);
		this.saveAction = new global::Gtk.Action ("saveAction", global::Mono.Unix.Catalog.GetString ("_Save"), null, "gtk-save");
		this.saveAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Save");
		w1.Add (this.saveAction, null);
		this._buggy = new global::Gtk.Action ("Action", global::Mono.Unix.Catalog.GetString ("-"), null, null);
		this._buggy.ShortLabel = global::Mono.Unix.Catalog.GetString ("-");
		w1.Add (this._buggy, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("Stock Quote App");
		this.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-preferences", global::Gtk.IconSize.Menu);
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox2 = new global::Gtk.VBox ();
		this.vbox2.Name = "vbox2";
		this.vbox2.Spacing = 6;
		this.vbox2.BorderWidth = ((uint)(10));
		// Container child vbox2.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><menubar name='menubar1'><menu name='FileAction' action='FileAction'><menuitem name='addAction' action='addAction'/><separator/><menuitem name='saveAction' action='saveAction'/></menu></menubar></ui>");
		this.menubar1 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar1")));
		this.menubar1.Name = "menubar1";
		this.vbox2.Add (this.menubar1);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.menubar1]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.frame2 = new global::Gtk.Frame ();
		this.frame2.Name = "frame2";
		this.frame2.ShadowType = ((global::Gtk.ShadowType)(2));
		this.frame2.LabelXalign = 0.5F;
		this.frame2.LabelYalign = 0F;
		// Container child frame2.Gtk.Container+ContainerChild
		this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
		this.GtkAlignment.Name = "GtkAlignment";
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		this.vbox4 = new global::Gtk.VBox ();
		this.vbox4.Name = "vbox4";
		this.vbox4.Spacing = 5;
		this.vbox4.BorderWidth = ((uint)(10));
		// Container child vbox4.Gtk.Box+BoxChild
		this.txtSymbols = new global::Gtk.Entry ();
		this.txtSymbols.CanFocus = true;
		this.txtSymbols.Name = "txtSymbols";
		this.txtSymbols.Text = global::Mono.Unix.Catalog.GetString ("BAC, C, CS, GS, JPM, MS, MSFT, IBM, CSCO, AAPL, CSC, -DJIA, XOM, RTN");
		this.txtSymbols.IsEditable = true;
		this.txtSymbols.InvisibleChar = '●';
		this.vbox4.Add (this.txtSymbols);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.txtSymbols]));
		w3.Position = 0;
		w3.Expand = false;
		w3.Fill = false;
		// Container child vbox4.Gtk.Box+BoxChild
		this.btnQuote = new global::Gtk.Button ();
		this.btnQuote.CanDefault = true;
		this.btnQuote.CanFocus = true;
		this.btnQuote.Name = "btnQuote";
		this.btnQuote.UseUnderline = true;
		this.btnQuote.Label = global::Mono.Unix.Catalog.GetString ("Quote");
		global::Gtk.Image w4 = new global::Gtk.Image ();
		w4.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-refresh", global::Gtk.IconSize.Menu);
		this.btnQuote.Image = w4;
		this.vbox4.Add (this.btnQuote);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.btnQuote]));
		w5.Position = 1;
		w5.Expand = false;
		w5.Fill = false;
		// Container child vbox4.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.gridQuotes = new global::Gtk.TreeView ();
		this.gridQuotes.CanFocus = true;
		this.gridQuotes.Name = "gridQuotes";
		this.GtkScrolledWindow.Add (this.gridQuotes);
		this.vbox4.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.GtkScrolledWindow]));
		w7.Position = 2;
		this.GtkAlignment.Add (this.vbox4);
		this.frame2.Add (this.GtkAlignment);
		this.GtkLabel1 = new global::Gtk.Label ();
		this.GtkLabel1.WidthRequest = 250;
		this.GtkLabel1.HeightRequest = 42;
		this.GtkLabel1.Name = "GtkLabel1";
		this.GtkLabel1.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Enter comma seperated symbol(s)</b>");
		this.GtkLabel1.UseMarkup = true;
		this.frame2.LabelWidget = this.GtkLabel1;
		this.vbox2.Add (this.frame2);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame2]));
		w10.Position = 1;
		this.Add (this.vbox2);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 478;
		this.DefaultHeight = 421;
		this.btnQuote.HasDefault = true;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.addAction.Activated += new global::System.EventHandler (this.OnAddActionToggled);
		this.saveAction.Activated += new global::System.EventHandler (this.OnSaveActionToggled);
		this.btnQuote.Clicked += new global::System.EventHandler (this.QuoteButton_OnClick);
	}
}
