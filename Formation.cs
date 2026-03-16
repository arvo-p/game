public class Formation{
	
	Environment env;
	PointF _position;
	PointF last_position;
	public PointF position{
		get => _position;
		set{
			last_position = _position;
			_position = value;
		}
	}
	List<Enemy> enemies=new List<Enemy>();
	int index = 0;

	public Formation(){
		this.env = Game.env;
		
		position = new PointF(200, 200);
		DoVFormation(position, 6, 80, 80, NewEnemy);
	}
	
	public void DoVFormation(PointF leaderPos, int unitCount, float gapX, float gapY, Func<PointF,int> extFunction){
		if(extFunction == null) return;

		for (int i = 0; i < unitCount; i++){
			int side = (i % 2 == 0) ? 1 : -1; 
			int row = (i + 1) / 2;          

			float x = leaderPos.X + (side * row * gapX);
			float y = leaderPos.Y + (row * gapY); 
			
			index = i;

			extFunction(new PointF(x, y));
		}
	}

	public void Update(){
		DoVFormation(new PointF(200, 800), 6, 80, 80, SendTarget);
	}

	public int SendTarget(PointF pos){
		enemies[index].posTarget = pos;
		enemies[index].hasPosTarget = true;
		return 0;
	}

	public int NewEnemy(PointF pos){
		Enemy newEnemy = new Thug(pos,this);
		enemies.Add(newEnemy);
		env.All.Add(newEnemy);
		
		return 0;
	}
}
