using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Windows.Controls.Data
{
    [DataContract]
    public class Portfolio
    {
        public Portfolio(): this(string.Empty)
        {
            
        }

        public Portfolio(string name)
        {
            Name = name;
            Symbols = new List<string>();
        }

        #region Plubming

        public override bool Equals(object obj)
        {
            if (!(obj is Portfolio) || null == obj)
                return false;

            Portfolio p = obj as Portfolio;
            return p.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Portfolio a, Portfolio b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Name == b.Name;
        }

        public static bool operator !=(Portfolio a, Portfolio b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return false;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return true;
            }

            return a.Name != b.Name;
        }

        #endregion

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<string> Symbols {get; internal set;}
        public string csvSymbols { get { return string.Join(",", Symbols); } }
    }

    [CollectionDataContract(Name = "Book", ItemName = "Portfolio")]
    [KnownType(typeof(Portfolio))]
    public class Book : List<Portfolio>
    {
        public Book() : base()
        {                        
         
        }

        public new void Add(Portfolio p)
        {
            if(!this.Contains(p))
            {
                base.Add(p);
                return;
            }

            var folio = this.FirstOrDefault( x => x == p);
            if(null == folio)
                return;

            var c = folio.Symbols.Count;
            folio = p;                            
        }

        public void Save(string filename = null)
        {
            var file = filename;
            if (string.IsNullOrEmpty(file))
                file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "book.json");
            using (var stream1 = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Book));
                ser.WriteObject(stream1, this);
            }
        }
    }
}

