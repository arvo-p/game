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
	
	private GraphicsState DrawRotatedBegin(PaintEventArgs e, Image image, RectangleF r, float rotation){
		var state = e.Graphics.Save();
		e.Graphics.TranslateTransform((float)(r.Width)/2+r.X, (float)r.Height / 2+r.Y);
		e.Graphics.RotateTransform(rotation);
		e.Graphics.DrawImage(image, -r.Width/2, -r.Height/2, r.Width, r.Height);
		return state;
	}

	private void DrawEntity(PaintEventArgs e, Entity ent){
		var state = DrawRotatedBegin(e, ent.image, ent.r, ent.rotation);
		if(ent.props != null)
			foreach(var p in ent.props)
				e.Graphics.DrawImage(p.image, -p.r.Width/2+p.r.X, -p.r.Height/2+p.r.Y, p.r.Width, p.r.Height);
		e.Graphics.Restore(state);
	}

	private void DrawPlayer(PaintEventArgs e, Player player){
		Graphics g = e.Graphics;
		DrawRotated(e, player.image, player.r, player.rotation);

		Weapon? weapon = player.selectedWeapon;
		if(weapon == null) return;
		DrawRotated(e, weapon.image, player.r, player.rotation);
	}
	
	private void DrawMap(PaintEventArgs e, Map map){
		int currentI = (int)Math.Floor(Game.camera.r.X/(Game.windowWidth));
		int currentJ = (int)Math.Floor(Game.camera.r.Y/(Game.windowHeight));
		
		for(int i=currentI-1; i<currentI+2;i++){
			if(i < 0 || i > map.gmap.GetLength(0) - 1) continue;
			for(int j=currentJ-1; j<currentJ+2;j++){
				if(j < 0 || j > map.gmap.GetLength(1) - 1) continue;
				e.Graphics.DrawImageUnscaled(map.gmap[i,j], i*Game.windowWidth, j*Game.windowHeight); 
			}
		}
	}

	PointF start, end;
	public void DebugSetLine(PointF start, PointF end){
		this.start = start;
		this.end = end;
	}

	public void Update(PaintEventArgs e){
		e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
		e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
	
		float offsetX = (windowWidth / 2) - Game.camera.r.X;
    	float offsetY = (windowHeight / 2) - Game.camera.r.Y;
		e.Graphics.TranslateTransform(offsetX, offsetY);
		
		DrawMap(e, env.map);

		foreach(var prp in env.props) e.Graphics.DrawImage(prp.image, prp.r.X, prp.r.Y, prp.r.Width, prp.r.Height);
		foreach(var obj in env.All.Objects) DrawRotated(e, obj.image, obj.r, obj.rotation);
		foreach(var obj in env.All.Entities.NPCs) DrawEntity(e, obj);
		foreach(var obj in env.All.Entities.Players) DrawPlayer(e, obj);

		/*
		 * DEBUG collision
		Pen myPen = new Pen(Color.Red);
        myPen.Width = 8;
		foreach(var ent in env.entities){
			foreach(var hit in ent.hitboxes){
				e.Graphics.DrawEllipse(myPen, hit.center.X-hit.radius, hit.center.Y-hit.radius, hit.radius*2, hit.radius*2); 
				float diffHeight = hit.offset;
        		e.Graphics.DrawLine(myPen, ent.r.X+ent.r.Width/2, ent.r.Y+ent.r.Height/2, ent.r.X+ent.r.Width/2, diffHeight+ent.r.Y+ent.r.Height/2);
			}
		}*/
	}

	public Draw(Environment e, int windowWidth, int windowHeight){
		this.env = e;
		this.windowWidth = windowWidth;
		this.windowHeight = windowHeight;
	}
}
