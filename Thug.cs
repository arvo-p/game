public class Thug : Entity{

	public Sprite walk;
	public Sprite shoot;  
	public Sprite stand;
	public Sprite dead;

	Environment env;
	Player local_player;

	public Thug(Environment env){
		this.env = env;
		this.local_player = env.p;

		LoadSprites();
		_sprite = stand;

		r.Location = new Point(300, 300);
		r.Size = new Size(75, 75);
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

	bool isActionInProgress = false;
	float aiming_rotation;

	public void Action(){
		float distance = (float)Math.Sqrt(Math.Pow(this.r.X - local_player.r.X, 2) + Math.Pow(this.r.Y - local_player.r.Y, 2));
		PointF difference = new PointF(this.r.Y-local_player.r.Y,this.r.X-local_player.r.X);
		aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;

		if(this.rotation-aiming_rotation > 180){
			this.rotation -= 180;
			speed *= -1;
		}

		if(isActionInProgress) return;
		if(distance > 300){
			speed += 2;
		}else{
			_sprite = shoot;
			isActionInProgress = true;
			new Task(() => {sprite.Trigger(endAttack);}).Start();
			speed = 0;
		}
	}

	public void endAttack(){
		isActionInProgress = false;
	}

	public override void Update(){
		Action();
		if(isActionInProgress == false) _sprite = UpdateSprite();

		PointF movement = Scalar2Vect_Speed(rotation, speed); 
		r.Location = new PointF(movement.X + r.X, movement.Y + r.Y);

		this.rotation = aiming_rotation;

		if(this.rotation < aiming_rotation) this.rotation = (this.rotation+5)%360;
		if(this.rotation > aiming_rotation) this.rotation = (this.rotation-5)%360;

		if(speed > 4) speed = 4;
		if(speed < -4) speed = -4;
		speed *= (float)0.7;
	}
}
