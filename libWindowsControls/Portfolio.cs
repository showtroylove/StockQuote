using System.Collections.Generic;

namespace Windows.Controls.Data
{
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

        public string Name { get; set; }
        public List<string> Symbols {get; internal set;}
    }
}

