using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CustomControls
{
    public class CueTextBox : TextBox
    {
        private string _Cue;

        public string Cue
        {
            get => _Cue;
            set
            {
                _Cue = value;
                updateCue();
            }
        }

        private void updateCue()
        {
            if (!this.IsHandleCreated || string.IsNullOrEmpty(_Cue))
            {
                return;
            }

            IntPtr mem = Marshal.StringToHGlobalUni(_Cue);
            var unused = SendMessage(Handle, 0x1501, (IntPtr)1, mem);
            Marshal.FreeHGlobal(mem);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            updateCue();
        }

        // P/Invoke
        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }
}