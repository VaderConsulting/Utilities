using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ListboxItem
    {
        private string _Value = "";
        private object _Tag = new object();

        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        public object Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                _Tag = value;
            }
        }

        public ListboxItem()
        {
        }

        public ListboxItem(string Value)
        {
            _Value = Value;
        }

        public ListboxItem(string Value, object Tag)
        {
            _Value = Value;
            _Tag = Tag;
        }

        public override string ToString()
        {
            return _Value;
        }
    }
}
