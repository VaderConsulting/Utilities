using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class CueTextBox : TextBox
{
	private string _Cue;

	public string Cue
	{
		get
		{
			return _Cue;
		}
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
		SendMessage(this.Handle, 0x1501, (IntPtr)1, mem);
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