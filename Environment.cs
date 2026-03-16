using System.Drawing.Drawing2D;

public class Environment{

	public WeaponFootprints weaponFootprints = new WeaponFootprints();

	public Map map;

	public Player p;

	public ObjectsManager All = new ObjectsManager();
	
	/* TODO
	 * - List organizer
	 * - Formation rotation
	 * - Spaw area and regular spawn each 5 minutes if entity number < X
	 * - Damage and all that
	 * - Car collision
	 * - Pick up items
	 * - Sounds
	 */

	Formation fTemp;
	public Environment(){
		Game.env = this;
		map = new Map(new string[]{"Resources/Tiles/test_Background.csv","Resources/Tiles/test_Decorations.csv","Resources/Tiles/test_Collisions.csv"}, "Resources/Tiles/tilesheet.png");

		p = new Player();
		Game.camera.Follow(p);

		All.Add(new Vehicle());
		/*nonplayer_entities.Add(new Thug(new Point(300, 300)));
		nonplayer_entities.Add(new Thug(new Point(300, 500)));
		nonplayer_entities.Add(new Thug(new Point(300, 700)));*/
		
		All.Add(p);
		
		fTemp = new Formation();
	}
	
	public void Update(){
		fTemp.Update();
		foreach(var obj in All)
			obj.UpdateRoutine();
	}

	private int ResolveRectangleTileCollision(RectangleF rect){
		PointF[] poss = {
			new PointF(rect.X+10, rect.Y+10),
			new PointF(rect.X+10, rect.Y+rect.Height-10),
			new PointF(rect.X + rect.Width - 10,rect.Y + 10),
			new PointF(rect.X + rect.Width-10,rect.Y+rect.Height-10)
		};
		
		foreach(var p in poss){
			var r = map.GetTileFromCoordinates(p);
			if(r.x == -1) return 1;
			if(map.collision[r.x,r.y] == 1) return 1;
		}	

		return 0;
	}

	private bool ResolveCircleTileCollision(CollisionCircle cc, int tilex, int tiley, int mode){
		return ResolveCircleTileCollision(cc.center, cc.radius, tilex, tiley, cc.parent, mode);
	}

	private bool ResolveCircleTileCollision(PointF center, float radius, int tilex, int tiley, Object parent, int mode){
		int tileSize = map.tileRenderDimension;
		
		PointF closest = new PointF(Math.Clamp(center.X, tileSize*tilex, tileSize*(tilex+1)),
									Math.Clamp(center.Y, tileSize*tiley, tileSize*(tiley+1)));

		PointF distance = new PointF(center.X - closest.X, center.Y - closest.Y);

		float distancesquared = distance.X*distance.X+distance.Y*distance.Y;
		if(distancesquared < radius*radius){
			if(parent == null) return true;
			var r = GetCirclesCollisionOverlap(distance, distancesquared, radius, 0); 
			PointF movement = new PointF(r.direction.X*(r.overlap+0.01f), r.direction.Y*(r.overlap+0.01f));
			
			if(mode == 0){
				parent.speed *= -0.6f;
				UpdatePosition(parent, movement);
			}

			if(mode == 1){
				float cross = center.X * closest.Y - center.Y * closest.X;
				float newrotation = (r.overlap/radius)*0.0174533f;
				
				if(cross < 0) parent.rotation += newrotation;
				else parent.rotation -= newrotation;
				UpdatePosition(parent, movement);
			}

			return true;
		}

		return false;
	}
	
	private void UpdatePosition(Object target, PointF movement){
		target.r.X += movement.X;
		target.r.Y += movement.Y;

		target.PositionUpdated();
		CollisionCircle.UpdateCenters(target.hitboxes, movement); 
	}

	public PointF CheckRectangleTileCollision(CollisionCircle cc, PointF speed){
		RectangleF futureX = new RectangleF(cc.center.X - cc.radius + speed.X, cc.center.Y - cc.radius, 2*cc.radius, 2*cc.radius);
		if(ResolveRectangleTileCollision(futureX) == 1) speed.X = Math.Sign(speed.X)*0.01f;

		RectangleF futureY = new RectangleF(cc.center.X - cc.radius, cc.center.Y - cc.radius + speed.Y, 2*cc.radius, 2*cc.radius);;
		if(ResolveRectangleTileCollision(futureY) == 1) speed.Y = Math.Sign(speed.Y)*0.01f; 
		
		return speed;
	}

	public bool CheckCircleTileCollision(CollisionCircle cc, PointF mov, int mode){
		float tileSize = map.tileRenderDimension;

		PointF center = cc.center;
		float radius = cc.radius;
		
		PointF start = new PointF((center.X + mov.X - radius)/tileSize, (center.Y + mov.Y - radius)/tileSize);
		PointF end = new PointF((center.X + mov.X + radius)/tileSize, (center.Y + mov.Y + radius)/tileSize);
		
		for(int x=(int)start.X;x<=(int)end.X; x++)
			for(int y=(int)start.Y;y<=(int)end.Y;y++)
				if(map.collision[x,y] == 1) return ResolveCircleTileCollision(cc, x, y, mode);

		return false;
	}

	private (float overlap, PointF direction, float distance) GetCirclesCollisionOverlap(PointF direction, float distancesquared, float radius1, float radius2){
		float distance = (float)Math.Sqrt(distancesquared); 
		
		if (distance == 0){
			direction = new PointF(1, 0);
			distance = 1;
		}
		direction.X /= distance;
		direction.Y /= distance;
		
		float overlap = (radius1 + radius2) - distance;
		return (overlap, direction, distance);
	}
 
	private (float overlap, PointF direction) GetCirclesCollisionOverlap(CollisionCircle hitbox1, CollisionCircle hitbox2){
		PointF direction = new PointF(hitbox1.center.X - hitbox2.center.X, hitbox1.center.Y - hitbox2.center.Y);
		float distancesquared = (float)(direction.X*direction.X+direction.Y*direction.Y); 
		var r = GetCirclesCollisionOverlap(direction, distancesquared, hitbox1.radius, hitbox2.radius);
		return (r.overlap, r.direction);
	}

	public Object IsObjectColliding(Object obj, float padding, Func<Object, CollisionCircle, CollisionCircle, int, int> extfunction, int mode){
		foreach(var obj2 in All){
			if(Object.ReferenceEquals(obj2, obj)) continue;
			if(obj2.isSolid == false) continue;
			if(((float)Math.Pow(obj.r.X - obj2.r.X, 2) + Math.Pow(obj.r.Y - obj2.r.Y, 2)) >= 90000) continue;

			foreach(var hitbox1 in obj.hitboxes)
				foreach(var hitbox2 in obj2.hitboxes){
					int ret;
					if(Tools.IsCircleColliding(hitbox1.center, hitbox1.radius+padding, hitbox2.center, hitbox2.radius) == false) continue;
					if(extfunction != null) ret = extfunction(obj,hitbox1, hitbox2, 0);
					return obj2;
				}
		}
		return null;
	}

	private int AdjustCirclesOverlap(Object obj, CollisionCircle hitbox1, CollisionCircle hitbox2, int mode){
		var r = GetCirclesCollisionOverlap(hitbox1, hitbox2);
		PointF movement = new PointF((r.direction.X * (r.overlap+0.01f)), r.direction.Y * (r.overlap+0.01f)); 

		if(mode == 1){
			float cross = hitbox1.center.X * hitbox2.center.Y - hitbox1.center.Y * hitbox2.center.X;
			float newrotation = (r.overlap/hitbox1.radius)*0.0174533f;
				
			if(cross < 0) hitbox1.parent.rotation += newrotation;
			else obj.rotation -= newrotation;
		}

		UpdatePosition(obj, movement);
		return 0;
	}

	public void Rotate(Object obj, float addRotation){
		short hitboxCount = (short)obj.hitboxes.Count;

		float baseRotation = obj.rotation;
		float newRotation = (baseRotation+addRotation);
		
		if(hitboxCount == 1){
			obj.rotation = newRotation;
			return;
		}

		List<PointF> centers = new List<PointF>();
		for(int i=0;i<hitboxCount;i++){
			CollisionCircle hitbox = obj.hitboxes[i];

			float diffHeight = hitbox.offset;
			double rot_radians = (obj.rotation)*0.0174533; 
			
			PointF rotatedcoords = new PointF(
				((float)(Math.Sin(-rot_radians)*diffHeight) + hitbox.pcenter.X),
				((float)(Math.Cos(rot_radians)*diffHeight) + hitbox.pcenter.Y)
			);
		
			if(CheckCircleTileCollision(hitbox, new PointF(rotatedcoords.X-hitbox.center.X, rotatedcoords.Y-hitbox.center.Y),1)) return; 
			centers.Add(rotatedcoords);
		}
		
		
		if(IsObjectColliding(obj, 0, AdjustCirclesOverlap, 1) != null){
			return;
		}
		obj.rotation = newRotation;
		for(int i=0;i<hitboxCount;i++) obj.hitboxes[i].center = centers[i];
	}

	public void Move(Object obj, PointF movement){
		int countHitboxes = obj.hitboxes.Count;

		if(countHitboxes == 1) movement = CheckRectangleTileCollision(obj.hitboxes[0], movement);
		else foreach(var cc in obj.hitboxes) if(CheckCircleTileCollision(cc, movement, 0)) return;
		
		if(movement.X == 0 && movement.Y == 0) return;
		
		if(obj.isSolid == true){
			Object return_object = IsObjectColliding(obj, 0, AdjustCirclesOverlap, 0); 
			if(return_object != null) return;
		}

		obj.r.Y += movement.Y;
		obj.r.X += movement.X;
		obj.PositionUpdated();
		CollisionCircle.UpdateCenters(obj.hitboxes, movement); 
		return;
	}
}
