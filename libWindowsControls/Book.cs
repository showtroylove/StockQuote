using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Windows.Controls.Data
{
    [CollectionDataContract(Name = "Book", ItemName = "Portfolio")]
    [KnownType(typeof(Portfolio))]
    public class Book : List<Portfolio>
    {
        public Book()
            : base()
        {   
            IsDirty = false;
        }

        public bool IsDirty { get; set; }

        public Portfolio this[string name] => Find(x => x.Name == name.Trim().ToUpperInvariant());

        public new void Add(Portfolio p)
        {
            IsDirty = true;

            if (!this.Contains(p))
            {
                base.Add(p);
                return;
            }

            var folio = this.Find(x => x == p);
            if (null == folio)
                return;

            if (Remove(folio))
                base.Add(p);                     
        }

        public void Save(string filename = null)
        {
            var file = filename;
            if (string.IsNullOrEmpty(file))
                file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ".book.json");

            using (var jsonStream = new FileStream(file, FileMode.Truncate, FileAccess.ReadWrite))
            {
                var ser = new DataContractJsonSerializer(typeof(Book));
                ser.WriteObject(jsonStream, this);
            }

            using (var jsonStream = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(typeof(Book));
                ser.WriteObject(jsonStream, this);

            }
        }

        public static Book Load(string filename = null)
        {
            var file = filename;
            if (string.IsNullOrEmpty(file))
                file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ".book.json");

            using (var jsonStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read))
            {
                var ser = new DataContractJsonSerializer(typeof(Book));
                var obj = ser.ReadObject(jsonStream);
                return obj as Book ?? new Book();
            }
        }
    }
}

