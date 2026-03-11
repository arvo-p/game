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
		
		nonplayer_entities.Add(new Thug(this));
		players.Add(p);
		
		entities.AddRange(nonplayer_entities);
		entities.AddRange(players);
		objects.AddRange(entities);
	}
	
	public void Update(){
		foreach(var obj in objects){
			obj.UpdateRoutine();
			//if(CheckTileCollision(obj.r) != 0) obj.RollBackPosition();
		}	
		/*	
		DoCollisionVerifications(objects, objects);
		DoCollisionVerifications(objects, entities);
		DoCollisionVerifications(entities, entities);*/
	}


	private bool IsColliding(RectangleF a, RectangleF b){
		if (a.X + a.Width  < b.X) return false; 
		if (a.X > b.X + b.Width)  return false;
		if (a.Y + a.Height < b.Y) return false; 
		if (a.Y > b.Y + b.Height) return false;

		return true;
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

	private void DoCollisionVerifications(IEnumerable<Object> list1, IEnumerable<Object> list2){
		foreach(var obj in list1){
			foreach(var obj2 in list2){
				if(Object.ReferenceEquals(obj2, obj)) continue;
				if(IsColliding(obj.r, obj2.r)){
					obj.Collision();
					obj2.Collision();
				}
			}
		}
	}

	public void Move(Object obj, PointF speed){
		//obj.r.Location = new PointF(movement.X + r.X, movement.Y + r.Y);
		short nulx = 1;
		short nuly = 1;

		RectangleF r = obj.r;
		
		r.Location = new PointF(r.X + speed.X, r.Y);
		if(CheckTileCollision(r) == 1) nulx = 0;

		r.Location = new PointF(r.X, r.Y + speed.Y);
		if(CheckTileCollision(r) == 1) nuly = 0;
	
		r = obj.r;
		obj.r.Location = new PointF(r.X + speed.X*nulx, r.Y + speed.Y*nuly);
	}
}
