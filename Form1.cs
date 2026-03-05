using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace game;

public partial class Form1 : Form{
	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool AllocConsole();

	private const short windowWidth = 1200;
	private const short windowHeight = 800;

	public Environment env;

	private System.Windows.Forms.Timer gametimer = new System.Windows.Forms.Timer();

    public Form1(){
		Tests();
		InitializeUI();
		InitializeGame();
	}

	Map map;
	private void Tests(){
		AllocConsole();
		map = new Map(new string[]{"Resources/Tiles/test_Tile Layer 1.csv"}, "Resources/Tiles/tilesheet.png");
	}

	private void InitializeUI(){
		BackColor = Color.White;
		ClientSize = new Size(windowWidth, windowHeight);
		MaximizeBox = false;
		KeyPreview = false;
		MinimizeBox = false;
		DoubleBuffered = true;
		Name = "Form1";
		Text = "";
	}

	private void InitializeGame(){
		env = new Environment();

		gametimer.Interval = 16;
		gametimer.Tick += (s, e) => {
			env.Update();
			this.Invalidate();
		};
		gametimer.Start();
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

	private void DrawRotated(PaintEventArgs e, Image image, RectangleF r, float rotation){
		var state = e.Graphics.Save();

		e.Graphics.TranslateTransform((float)(r.Width)/2+r.X, (float)r.Height / 2+r.Y);
		e.Graphics.RotateTransform(rotation);
		e.Graphics.DrawImage(image, -r.Width/2, -r.Height/2, r.Width, r.Height); 	

		e.Graphics.Restore(state);
	}	

	private void DrawPlayer(PaintEventArgs e, Player player){
		
		Graphics g = e.Graphics;
		//GraphicsState state = g.Save();
    	//g.TranslateTransform(offsetX, offsetY);
		DrawRotated(e, player.image, player.r, player.rotation);
		//g.Restore(state);

		Weapon? weapon = player.selectedWeapon;
		if(weapon == null) return;
		DrawRotated(e, weapon.image, player.r, player.rotation);
	}

	protected override void OnPaint(PaintEventArgs e){
		e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
	
		float offsetX = (windowWidth / 2) - env.p.r.X;
    	float offsetY = (windowHeight / 2) - env.p.r.Y;
		
		e.Graphics.TranslateTransform(offsetX, offsetY);

		int i = (int)env.p.r.X/map.tileRenderDimension;
		int maximumIndex = (int)(env.p.r.X+windowWidth)/map.tileRenderDimension;
		if(maximumIndex > map.map.GetLength(0)) maximumIndex = map.map.GetLength(0);

		int j = (int)env.p.r.Y/map.tileRenderDimension;
		int maximumJndex = (int)(env.p.r.Y+windowHeight)/map.tileRenderDimension;	
		if(maximumJndex > map.map.GetLength(0)) maximumJndex = map.map.GetLength(0);

		for(i=0;i<maximumIndex;i++){
			for(j=0;j<maximumJndex;j++){
				if(map.map[i,j] == -1) continue;
				e.Graphics.DrawImage(map.tileMap[map.map[i,j]], i*map.tileRenderDimension, j*map.tileRenderDimension, map.tileRenderDimension, map.tileRenderDimension); 
			}
		}

		foreach(var obj in env.objects) DrawRotated(e, obj.image, obj.r, obj.rotation);
		foreach(var obj in env.nonplayer_entities) DrawRotated(e, obj.image, obj.r, obj.rotation);
		DrawPlayer(e, env.p);
	}

}
