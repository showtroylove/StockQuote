using System;
using System.Xml;
using System.Xml.Linq;
using Windows.Controls.Data;
using System.Linq;
using Gtk;
using System.IO;

namespace QuoteView.UI.Helpers
{
    public static class PortfolioMenuItems
    {
        public static void AddPortfolios(this UIManager mgr, Book b, EventHandler actHndlr, string parent = "mnuPortfolios")
        {
            if (null == mgr || null == b)
                return;
            
            using (TextReader xml = new StringReader(mgr.Ui))
            {
                var doc = XDocument.Load(xml);           
                var inter = doc.Descendants("menu");
                inter = inter.Where(x => x.Attribute("name").Value == parent);
                var bookNode = inter.FirstOrDefault();

                foreach (var action in bookNode.Descendants().Skip(3).Attributes("action").Select(x => x.Value))
                {
                    var a = mgr.ActionGroups[0].GetAction(action);
                    if (null == a)
                        continue;
                    a.Visible = false;
                }

                // Should represents the -DJIA and -NASDAQ nodes and the seperator
                bookNode.Descendants().Skip(3).Remove();
                foreach (var p in b)
                {
                    var attr = "mnu" + p.Name;
                    if (string.IsNullOrEmpty(p.Name))
                        continue;                

                    var action = mgr.ActionGroups[0].GetAction(attr);
                    if (null == action)
                        AddMenuItem(mgr, p, actHndlr);
                    else
                        action.Visible = true;
                    
                    var newNode = new XElement("menuitem", new XAttribute("name", attr), new XAttribute("action", attr));
                    bookNode.Add(newNode);
                }

                mgr.AddUiFromString(doc.ToString());                       
                xml.Close();
            }
        }

        private static void AddMenuItem(UIManager mgr, Portfolio sym, EventHandler actHndlr, string iconname = "gtk-refresh")
        {
            var mnuItem = new global::Gtk.Action("mnu" + sym.Name, sym.Name, null, iconname);
            mnuItem.ShortLabel = sym.Name;
            mnuItem.Activated += new global::System.EventHandler(actHndlr);
            mgr.ActionGroups[0].Add(mnuItem, null);
        }
    }
}

