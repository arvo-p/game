public class Vehicle : Entity{

	Keyboard mykeyboard;
	public float stretchFactor = 1.014f;
	public float squashFactor = 0.986f;
	public int isAccelerating = 0;
	public int isTurning = 0;

	public Prop shadow;

	public Vehicle(){
		this.env = Game.env;
		this.friction = 0.94f;
		this.maxspeed = 23;
		this.inverted_vectors = true;
		
		_sprite = new Sprite(Resources.Vehicle._car);
		r.Location = new Point(600, 600);
		r.Size = new Size((int)(115), (int)(225));
		mass = 900;
		SetCollisionCircles();
		setHealth(100);

		props = new List<Prop>();
		shadow = new Prop(Resources.Vehicle._carshadow, new RectangleF(0, 0, r.Width, r.Height), this.rotation);
		props.Add(new Prop(Resources.Environments._smoke, new RectangleF(0, 20, 64, 64), this.rotation+90));
	}

	private void HandleInput(){
		mykeyboard.ReadKeys();

		isTurning = 0;
		if(Math.Abs(speed) > 1){
			float speedFactor = Math.Abs(speed) / (maxspeed);
			float deltarot=0;
			if(mykeyboard.GetKey(Keys.Q)){
				isTurning = -1;
				deltarot = -2/((1-speedFactor*0.5f));
			}
			if(mykeyboard.GetKey(Keys.D)){
				isTurning = 1;
				deltarot = 2/((1-speedFactor*0.5f));
			}

			if(deltarot != 0){
				if(speed < 0) deltarot *= -1;
				env.Rotate(this, deltarot);
			}
		}

		if(mykeyboard.GetKeyOnce(Keys.E)){
			//LeaveCar();
		}
		
		isAccelerating = 0;
		if(mykeyboard.GetKey(Keys.Z)){
			if(speed < maxspeed - 0.3f) isAccelerating = 1;
			speed += 1.6f;
		}
		if(mykeyboard.GetKey(Keys.S)){
			if(speed > -maxspeed/2 + 0.3f) isAccelerating = -1;
			speed -= 1.6f;
		}

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
