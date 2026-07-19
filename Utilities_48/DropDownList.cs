using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CustomControls
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
            BeforeUpdate?.Invoke(this, e);
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
                CancelEventArgs cea = new();
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
