public class Thug : Enemy{

	public Thug(){
		Init();
	}

	private void Init(){
		Console.WriteLine("New thug created");
		this.env = Game.env;
		this.local_player = env.p;

		r.Size = new Size(72, 72);
		setHealth(100);
		LoadSprites();
		_sprite = stand;

		mass = 90;

		PositionUpdated();
		SetCollisionCircles();
	}

	public Thug(PointF pos, Formation formation){
		this.myFormation = formation;
		this.r.Location = pos;
		Init();
	}

	public Thug(PointF pos){
		r.Location = pos;
		Init();
	}

	void LoadSprites(){
		walk = new Sprite(Resources.Thug._walk);
		shoot = new Sprite(Resources.Thug._shoot, 0);
		stand = new Sprite(Resources.Thug._stand);
		dead = new Sprite(Resources.Thug._death);
	}

	private Sprite UpdateSprite(){
		if(isDead) return dead;
		if(speed > 0.2) return walk;
		return stand;
	}

}
