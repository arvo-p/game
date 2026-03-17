public class Vehicle : Entity{

	Keyboard mykeyboard;

	public Vehicle(){
		this.env = Game.env;
		this.friction = 0.94f;
		this.maxspeed = 23;
		this.inverted_vectors = true;
		
		_sprite = new Sprite(Resources.Vehicle._car);
		r.Location = new Point(600, 600);
		r.Size = new Size((int)(110), (int)(220));
		mass = 900;
		SetCollisionCircles();
		setHealth(100);

		props = new List<Prop>();
		props.Add(new Prop(Resources.Environments._smoke, new RectangleF(0, 20, 64, 64), this.rotation+90));
	}

	private void HandleInput(){
		mykeyboard.ReadKeys();

		if(Math.Abs(speed) > 1){
			float speedFactor = Math.Abs(speed) / (maxspeed);
			float deltarot=0;
			if(mykeyboard.GetKey(Keys.Q)) deltarot = -2/((1-speedFactor*0.5f));
			if(mykeyboard.GetKey(Keys.D)) deltarot = 2/((1-speedFactor*0.5f));

			if(deltarot != 0){
				if(speed < 0) deltarot *= -1;
				env.Rotate(this, deltarot);
			}
		}

		if(mykeyboard.GetKeyOnce(Keys.E)){
			//LeaveCar();
		}
		
		if(mykeyboard.GetKey(Keys.Z)) speed += 1.2f;
		if(mykeyboard.GetKey(Keys.S)) speed -= 1.2f;
		speed = Math.Clamp(speed, -maxspeed/2, maxspeed);
	}

	public void MountPassenger(Entity entity, Keyboard mykeyboard){
		this.mykeyboard = mykeyboard;
		Game.camera.Follow(this);
	}

	public override void Update(){
		if(mykeyboard != null) HandleInput();
	}
}
