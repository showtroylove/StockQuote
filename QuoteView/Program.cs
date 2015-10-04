using System;
using Gtk;

namespace QuoteView
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            var win = new MainWindow();

            try
            {
                win.Show();
                Application.Run();           
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
        }
    }
}
