public class Environment{

	public WeaponFootprints weaponFootprints = new WeaponFootprints();

	public Map map;
	
	public List<Object> objects = new List<Object>();
	public List<Entity> nonplayer_entities = new List<Entity>();
	public List<Entity> entities = new List<Entity>();
	public List<Entity> players = new List<Entity>();

	public Player p;

	public Environment(){
		map = new Map(new string[]{"Resources/Tiles/test_Background.csv","Resources/Tiles/test_Decorations.csv","Resources/Tiles/test_Collisions.csv"}, "Resources/Tiles/tilesheet.png");
		p = new Player(this);
	
		nonplayer_entities.Add(new Vehicle(this));
		nonplayer_entities.Add(new Thug(this));
		players.Add(p);
		
		entities.AddRange(nonplayer_entities);
		entities.AddRange(players);
		objects.AddRange(entities);
	}
	
	public void Update(){
		foreach(var obj in objects){
			obj.UpdateRoutine();
		}	
	}

	private int CheckTileCollision(RectangleF rect){

		PointF[] poss = {
			new PointF(rect.X+rect.Width/2, rect.Y+15),
			new PointF(rect.X+15, rect.Y+rect.Height/2),
			new PointF(rect.X+rect.Width-15,rect.Y + rect.Height/2),
			new PointF(rect.X+rect.Width/2,rect.Y+rect.Height-15)
		};


		foreach(var p in poss){
			var r = map.GetTileFromCoordinates(p);
			if(r.Item1 == -1) return 1;
			if(map.collision[r.Item1,r.Item2] == 1) return 1;
		}	

		return 0;
	}

	public PointF CheckTileCollision(Object obj, PointF speed){
		RectangleF futureX = obj.r;
		futureX.Location = new PointF(futureX.X + speed.X, futureX.Y);
		bool collidedTile = (CheckTileCollision(futureX) == 1);
		if(collidedTile) speed.X = 0;

		RectangleF futureY = obj.r;
		futureY.Location = new PointF(futureY.X, futureY.Y + speed.Y);
		collidedTile = (CheckTileCollision(futureY) == 1);
		if(collidedTile) speed.Y = 0; 

		return speed;
	}

	private (float overlap, PointF direction) GetCirclesCollisionOverlap(CollisionCircle hitbox1, CollisionCircle hitbox2){
		PointF direction = new PointF(hitbox1.center.X - hitbox2.center.X, hitbox1.center.Y - hitbox2.center.Y);
		float distance = (float)Math.Sqrt(direction.X*direction.X+direction.Y*direction.Y); 
		
		if (distance == 0){
			direction = new PointF(1, 0);
			distance = 1;
		}
		direction.X /= distance;
		direction.Y /= distance;

		float overlap = (hitbox1.radius + hitbox2.radius) - distance;
		return (overlap, direction);
	}

	public Object CheckObjectCollision(Object obj, float padding){
		foreach(var obj2 in objects){
			if(Object.ReferenceEquals(obj2, obj)) continue;
			if(((float)Math.Pow(obj.r.X - obj2.r.X, 2) + Math.Pow(obj.r.Y - obj2.r.Y, 2)) >= 90000) continue;

			foreach(var hitbox1 in obj.hitboxes){
				foreach(var hitbox2 in obj2.hitboxes){
					if(Tools.IsCircleColliding(hitbox1.center, hitbox1.radius+padding, hitbox2.center, hitbox2.radius) == false) continue;
					return obj2;
				}
			}
		}
		return null;
	}

	public bool DoObjectCollision(Object obj, PointF speed){
		bool collidedWithObject = false;
		foreach(var obj2 in objects){
			if(Object.ReferenceEquals(obj2, obj)) continue;
			if(((float)Math.Pow(obj.r.X - obj2.r.X, 2) + Math.Pow(obj.r.Y - obj2.r.Y, 2)) >= 90000) continue;

			bool resolvedThisFrame = false;
			foreach(var hitbox1 in obj.hitboxes){
				foreach(var hitbox2 in obj2.hitboxes){
					if(Tools.IsCircleColliding(hitbox1, hitbox2) == false) continue;
					collidedWithObject = true;
					
					var ret = GetCirclesCollisionOverlap(hitbox1, hitbox2);
					float overlap = ret.overlap;
					PointF direction = ret.direction;

					obj.r.X += direction.X*(overlap+0.01f);
					obj.r.Y += direction.Y*(overlap+0.01f);
					
					obj.PositionUpdated();
					
					resolvedThisFrame = true;
					break;
				}
				if(resolvedThisFrame) break;
			}
		}

		return collidedWithObject;
	}

	public void Move(Object obj, PointF speed){
		speed = CheckTileCollision(obj, speed);
		if(speed.X == 0 && speed.Y == 0) return;

		bool collidedWithObject = DoObjectCollision(obj, speed);

		if(collidedWithObject == false){
			obj.r.Y += speed.Y;
			obj.r.X += speed.X;
			obj.PositionUpdated();
		}
	}
}
