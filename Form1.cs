using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace game;

public partial class Form1 : Form{
	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool AllocConsole();

	private const short windowWidth = 1024+256;
	private const short windowHeight = 512+256;

    public Form1(){
		InitializeUI();
		Debug();
		Game.Init(this, windowWidth, windowHeight);
	}

	private void Debug(){
		AllocConsole();
		Console.WriteLine("New console");
	}

	private void InitializeUI(){
		BackColor = Color.White;
		ClientSize = new Size(windowWidth, windowHeight);
		MaximizeBox = false;
		KeyPreview = false;
		MinimizeBox = false;
		DoubleBuffered = true;
		Name = "Petty Goober";
		Text = "";

		this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); 
	    this.SetStyle(ControlStyles.UserPaint, true);           
    	this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		this.UpdateStyles();
	}

	private void SetFullscreen(bool fullscreen){
		if (fullscreen)
		{
			WindowState = FormWindowState.Maximized;
			FormBorderStyle = FormBorderStyle.None;
			ClientSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
		}
		else
		{
			WindowState = FormWindowState.Normal;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			ClientSize = new Size(windowWidth, windowHeight);
		}
		CenterToScreen();
    }

	protected override void OnPaintBackground(PaintEventArgs e){
	}
	protected override void OnPaint(PaintEventArgs e){
		Game.draw.Update(e);
	}

}
