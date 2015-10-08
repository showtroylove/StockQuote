using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Windows.Controls.Data
{
    [DataContract]
    public class Portfolio
    {
        public Portfolio() : this(string.Empty)
        {            
        }

        public Portfolio(string name)
        {
            Name = name;
            Symbols = new List<string>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<string> Symbols { get; internal set; }

        public string csvSymbols => string.Join(",", Symbols);

        #region Plubming

        public override bool Equals(object obj)
        {
            if (!(obj is Portfolio) || null == obj)
                return false;

            var p = obj as Portfolio;
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
    }
}

