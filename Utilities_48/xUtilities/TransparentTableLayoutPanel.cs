using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
