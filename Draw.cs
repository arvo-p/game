using System.Drawing;
using System.Drawing.Drawing2D;

public class Draw{

	Environment env;
	int windowWidth;
	int windowHeight;
	
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
	
	private void DrawMap(PaintEventArgs e, Map map){
		int i = (int)env.p.r.X/(Game.windowWidth*2);
		int j = (int)env.p.r.Y/(Game.windowHeight*2);

		if(i < 0) return;
		if(j < 0) return;
		if(i > map.gmap.GetLength(0)) return;
		if(j > map.gmap.GetLength(1)) return;

		e.Graphics.DrawImageUnscaled(map.gmap[0,0], 0*Game.windowWidth*2, 0*Game.windowHeight*2); 
	}

	public void Update(PaintEventArgs e){
		e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
	
		float offsetX = (windowWidth / 2) - env.p.r.X;
    	float offsetY = (windowHeight / 2) - env.p.r.Y;
		
		e.Graphics.TranslateTransform(offsetX, offsetY);
		
		DrawMap(e, Game.map);

		foreach(var obj in env.objects) DrawRotated(e, obj.image, obj.r, obj.rotation);
		foreach(var obj in env.nonplayer_entities) DrawRotated(e, obj.image, obj.r, obj.rotation);
		DrawPlayer(e, env.p);
	}

	public Draw(Environment e, int windowWidth, int windowHeight){
		this.env = e;
		this.windowWidth = windowWidth;
		this.windowHeight = windowHeight;
	}
}
