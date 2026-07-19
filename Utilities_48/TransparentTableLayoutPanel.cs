using System.Windows.Forms;

namespace CustomControls
{
    public class TransparentTableLayoutPanel : TableLayoutPanel
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //This returns the transparent (no background painting) mode
                return cp;
            }
        }
    }
}