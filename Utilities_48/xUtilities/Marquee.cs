using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Marquee : Label
{
	#region Constants

	#endregion

	#region Delegates

	#endregion

	#region Events

	#endregion

	#region Enums

	#endregion

	#region DLL Imports

	#endregion

	#region Fields

	private int _Offset = 0;
	private SolidBrush _BackgroundBrush = null;
	private SolidBrush _ForegroundBrush = null;

	#endregion

	#region Properties

	public Timer MarqueeTimer
	{
		get; set;
	}

	public int Speed
	{
		get; set;
	}

	public int yOffset
	{
		get; set;
	}

	#endregion

	#region Constructors and Destructor

	public Marquee()
	{
		_ForegroundBrush = new SolidBrush(ForeColor);
		_BackgroundBrush = new SolidBrush(BackColor);

		yOffset = 0;
		Speed = 1;

		MarqueeTimer = new Timer();
		MarqueeTimer.Interval = 15;
		MarqueeTimer.Enabled = Text.Trim().Length > 0;

		MarqueeTimer.Tick += MarqueeTimer_Tick;

		TextChanged += Marquee_TextChanged;
	}

	#endregion

	#region Event Handlers

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);

		e.Graphics.FillRectangle(_BackgroundBrush, e.ClipRectangle);
		e.Graphics.DrawString(Text, Font, _ForegroundBrush, _Offset, yOffset);
		e.Graphics.DrawString(Text, Font, _ForegroundBrush, ClientSize.Width + _Offset, yOffset);
	}

	private void MarqueeTimer_Tick(object sender, EventArgs e)
	{
		_Offset = (_Offset - Speed);
		if (_Offset < -ClientSize.Width)
		{
			_Offset = 0;
		}

		Invalidate();
	}

	private void Marquee_TextChanged(object sender, EventArgs e)
	{
		MarqueeTimer.Enabled = Text.Trim().Length > 0;
	}

	#endregion

	#region Private Methods

	#endregion

	#region Public Methods

	public void Start()
	{
		MarqueeTimer.Start();
	}

	public void Stop()
	{
		MarqueeTimer.Stop();
	}

	#endregion

	#region Classes

	// By my own convention all classes should be in their own file, however sometimes it makes sense to include a class within the same file as it's parent Namespace

	#endregion

}
