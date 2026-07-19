using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;

namespace Utilities
{ 
    public class DropDownList : ComboBox
    {
        private bool _Busy = false;
        private int _PreviousIndex = -1;

        public event CancelEventHandler BeforeUpdate;

        public DropDownList()
        {
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }


        protected virtual void OnBeforeUpdate(CancelEventArgs e)
        {
            if (BeforeUpdate != null)
            {
                BeforeUpdate(this, e);
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (_Busy)
            {
                return;
            }

            _Busy = true;

            try
            {
                CancelEventArgs cea = new CancelEventArgs();
                OnBeforeUpdate(cea);

                if (cea.Cancel)
                {
                    // Restore previous index
                    this.SelectedIndex = _PreviousIndex;
                    return;
                }

                _PreviousIndex = this.SelectedIndex;
                base.OnSelectedIndexChanged(e);
            }
            finally
            {
                _Busy = false;
            }
        }
    }

}
