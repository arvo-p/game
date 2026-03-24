using System.Drawing;
using System.Drawing.Drawing2D;

public class Draw{
	Environment env;
	int windowWidth;
	int windowHeight;
	
	private Matrix cameraMatrix = new Matrix(); 
	private Matrix m = new Matrix();
	private Matrix w = new Matrix();

	private GraphicsState DrawRotatedBegin(PaintEventArgs e, Image image, RectangleF r, float rotation){
		var state = e.Graphics.Save();
		e.Graphics.TranslateTransform((float)(r.Width)/2+r.X, (float)r.Height / 2+r.Y);
		e.Graphics.RotateTransform(rotation);
		e.Graphics.DrawImage(image, -r.Width/2, -r.Height/2, r.Width, r.Height);
		return state;
	}

	private void DrawRotated(PaintEventArgs e, Image image, RectangleF r, float rotation){e.Graphics.Restore(DrawRotatedBegin(e, image, r, rotation));}

	private void DrawEntity(PaintEventArgs e, Entity ent){
		var state = DrawRotatedBegin(e, ent.image, ent.r, ent.rotation);
		if(ent.props != null)
			foreach(var p in ent.props)
				e.Graphics.DrawImage(p.image, -p.r.Width/2+p.r.X, -p.r.Height/2+p.r.Y, p.r.Width, p.r.Height);
		e.Graphics.Restore(state);
	}
	
	private void DrawBuildings(PaintEventArgs e, List<Building> buildings){
		Graphics g = e.Graphics;
		RectangleF camera = Game.camera.r;

		float perspectiveFactor = 0.2f; 
		float ledgeThickness = 5f;
	
		int screenCX = windowWidth / 2;
		int screenCY = windowHeight / 2;
		
		RectangleF viewport = new RectangleF(camera.X - screenCX - 300, 
											 camera.Y - screenCY - 300, 
											 windowWidth + 500, 
											 windowHeight + 500);

		/*var visibleBuildings = buildings.Where(b => 
			viewport.IntersectsWith(new RectangleF(b.X, b.Y, b.Width, b.Height))
		).ToList();*/
		
		var visibleBuildings = buildings.Where(b => Tools.IsColliding(viewport, new RectangleF(b.X, b.Y, b.Width, b.Height))).ToList();

		var sortedBuildings = visibleBuildings.OrderByDescending(b => {
			float screenX = b.X - camera.X + screenCX;
			float screenY = b.Y - camera.Y + screenCY;
			double dx = screenX - screenCX;
			double dy = screenY - screenCY;
			return (dx * dx) + (dy * dy); 
    	}).ToList();
		
		foreach (var b in sortedBuildings){
			PointF sun = new PointF(2.8f, 3.0f);
			PointF worldBase = new PointF(b.X, b.Y);
			PointF distFromCam = new PointF(b.X - Game.camera.r.X, b.Y - Game.camera.r.Y);
			PointF offset = new PointF(
					distFromCam.X * perspectiveFactor * (b.Height / 50f),
				    distFromCam.Y * perspectiveFactor * (b.Height / 50f)
			);

			RectangleF baseRect = new RectangleF(worldBase.X, worldBase.Y , b.Width, b.Height);
			RectangleF roofRect = new RectangleF(worldBase.X + offset.X, worldBase.Y + offset.Y, b.Width, b.Height);
			PointF shadowOffset = new PointF(b.Height3D * sun.X, b.Height3D * sun.Y);

			PointF[] shadowPoints = {
				new PointF(worldBase.X, worldBase.Y),
				new PointF(worldBase.X + b.Width, worldBase.Y),
				new PointF(worldBase.X + b.Width + shadowOffset.X, worldBase.Y + shadowOffset.Y),
				new PointF(worldBase.X + shadowOffset.X, worldBase.Y + shadowOffset.Y)
			};

			using (Brush shadowBrush = new SolidBrush(Color.FromArgb(110, 0, 0, 0))){
				g.FillPolygon(shadowBrush, shadowPoints);
			}
			
			PointF[] leftWallPoints = {
				new PointF(roofRect.Left, roofRect.Top),      
				new PointF(roofRect.Left, roofRect.Bottom), 
				new PointF(baseRect.Left, baseRect.Bottom),
				new PointF(baseRect.Left, baseRect.Top)
			};
			PointF[] frontWallPoints = {
				new PointF(roofRect.Left, roofRect.Top),
				new PointF(roofRect.Right, roofRect.Top),
				new PointF(baseRect.Right, baseRect.Top),
				new PointF(baseRect.Left, baseRect.Top)
			};
			PointF[] rightWallPoints = {
				new PointF(roofRect.Right, roofRect.Top), 
				new PointF(roofRect.Right, roofRect.Bottom),
				new PointF(baseRect.Right, baseRect.Bottom),
				new PointF(baseRect.Right, baseRect.Top)
			};
			PointF[] botWallPoints = {
				new PointF(roofRect.Left, roofRect.Bottom), 
				new PointF(roofRect.Right, roofRect.Bottom),
				new PointF(baseRect.Right, baseRect.Bottom),
				new PointF(baseRect.Left, baseRect.Bottom)
			};
			
			RectangleF srcRect = new RectangleF(0, 0, b.wall.frame.Width, b.wall.frame.Height);
			using (Brush shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0))){
				if(roofRect.Left > baseRect.Left){
					g.DrawImage(b.wall.frame, new PointF[]{leftWallPoints[0], leftWallPoints[1], leftWallPoints[3]}, srcRect, GraphicsUnit.Pixel);
				}
				if(roofRect.Top > baseRect.Top){
					g.DrawImage(b.wall.frame, new PointF[]{frontWallPoints[0], frontWallPoints[1], frontWallPoints[3]}, srcRect, GraphicsUnit.Pixel);
				}
				if(roofRect.Right < baseRect.Right){
					g.DrawImage(b.wall.frame, new PointF[]{rightWallPoints[0], rightWallPoints[1], rightWallPoints[3]}, srcRect, GraphicsUnit.Pixel);
					g.FillPolygon(shadowBrush, rightWallPoints);
				}
				if(roofRect.Bottom < baseRect.Bottom){
					g.DrawImage(b.wall.frame, new PointF[]{botWallPoints[0], botWallPoints[1], botWallPoints[3]}, srcRect, GraphicsUnit.Pixel);
					g.FillPolygon(shadowBrush, botWallPoints);
				}
			}

			RectangleF innerRoof = new RectangleF(
				roofRect.X + ledgeThickness, 
				roofRect.Y + ledgeThickness, 
				roofRect.Width - (ledgeThickness * 2), 
				roofRect.Height - (ledgeThickness * 2)
			);
			
			g.FillRectangle(Brushes.Gray, roofRect);

			/*
			float parallaxShiftX = (roofRect.X - screenCX) * 0.05f; 
			float parallaxShiftY = (roofRect.Y - screenCY) * 0.05f;

			RectangleF parallaxSrcRect = new RectangleF(
				parallaxShiftX, 
				parallaxShiftY, 
				roofTexture.Width, 
				roofTexture.Height
			);
			g.DrawImage(roofTexture, innerRoof, parallaxSrcRect, GraphicsUnit.Pixel);
			*/

			using(Pen darkPen = new Pen(Color.FromArgb(120, 0, 0, 0), 2)){
				g.DrawLine(darkPen, innerRoof.Left, innerRoof.Top, innerRoof.Right, innerRoof.Top);
				g.DrawLine(darkPen, innerRoof.Left, innerRoof.Top, innerRoof.Left, innerRoof.Bottom);
			}
		}
	}

	private void DrawVehicle(PaintEventArgs e, Vehicle ent){
		var initialState = e.Graphics.Save();
		
		RectangleF r = ent.r;
		float fakeHeight = ent.speed * 0.12f;
		float offset = 0;

		if(ent.isTurning != 0) offset = ent.isTurning==1?-6:4;
		if(ent.isAccelerating==1&&ent.speed>0.2){
			r.Height *= ent.stretchFactor;
			r.Width *= ent.squashFactor;
		}
		if((ent.isAccelerating==-1&&ent.speed>0.2)||(ent.isAccelerating==1&&ent.speed<(-0.2))){
			r.Width *= ent.stretchFactor;
			r.Height *= ent.squashFactor;
		}
		
		var shadowState = e.Graphics.Save();
		w.Reset();
		w.Translate(r.Width/2f+r.X, r.Height/2f+r.Y);
		w.Rotate(ent.rotation);
		e.Graphics.MultiplyTransform(w); 
		e.Graphics.DrawImage(ent.shadow.image, -r.Width/2+offset+fakeHeight, -r.Height/2+fakeHeight, ent.r.Width, ent.r.Height);
		e.Graphics.Restore(shadowState);

		m.Reset();
		m.Translate(r.Width/2f+r.X, r.Height/2f + r.Y);
		m.Rotate(ent.rotation);
		e.Graphics.MultiplyTransform(m); 
		e.Graphics.DrawImage(ent.image, -r.Width/2, -r.Height/2, r.Width, r.Height);
		
		if(ent.props != null)
			foreach(var p in ent.props)
				e.Graphics.DrawImage(p.image, -p.r.Width/2+p.r.X, -p.r.Height/2+p.r.Y, p.r.Width, p.r.Height);

		e.Graphics.Restore(initialState);
	}

	private void DrawPlayer(PaintEventArgs e, Player player){
		Graphics g = e.Graphics;
		DrawRotated(e, player.image, player.r, player.rotation);

		Weapon? weapon = player.selectedWeapon;
		if(weapon == null) return;
		DrawRotated(e, weapon.image, player.r, player.rotation);
	}
	
	private void DrawCrosshair(PaintEventArgs e, Crosshair c){
		e.Graphics.DrawImage(c.image, c.r.X, c.r.Y, c.r.Width, c.r.Height);
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
		
		cameraMatrix.Reset();
		cameraMatrix.Translate(offsetX, offsetY);
		e.Graphics.Transform = cameraMatrix;
		
		DrawMap(e, env.map);

		foreach(var prp in env.props) e.Graphics.DrawImage(prp.image, prp.r.X, prp.r.Y, prp.r.Width, prp.r.Height);
		foreach(var obj in env.All.Objects) DrawRotated(e, obj.image, obj.r, obj.rotation);
		
		if(env.crosshair.isOn) DrawCrosshair(e, env.crosshair);

		foreach(var obj in env.All.Entities.NPCs) DrawEntity(e, obj);
		foreach(var obj in env.All.Entities.Vehicles) DrawVehicle(e, obj);
		foreach(var obj in env.All.Entities.Players) DrawPlayer(e, obj);
		
		DrawBuildings(e, env.map.buildings);

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
