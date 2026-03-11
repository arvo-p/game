public class Thug : Entity{

	public Sprite walk;
	public Sprite shoot;  
	public Sprite stand;
	public Sprite dead;

	Player local_player;

	public Thug(Environment env){
		this.env = env;
		this.local_player = env.p;

		LoadSprites();
		_sprite = stand;

		r.Location = new Point(300, 300);
		r.Size = new Size(72, 72);
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

	public override void IsHit(){
		isDead = true;
	}
	
	public void Action(){
		
		PointF difference = new PointF(this.r.Y-local_player.r.Y,this.r.X-local_player.r.X);
		aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;

		if(this.rotation-aiming_rotation > 180){
			this.rotation -= 180;
			speed *= -1;
		}

		if(isActionInProgress){
			attackTimer+=-0.052f;
			if(attackTimer <= 0){
				isActionInProgress = false;
			}else{
				return;
			}
		}

		float dx = this.r.X - local_player.r.X;
		float dy = this.r.Y - local_player.r.Y;

		float distance = (float)(dx*dx+dy*dy);
		if(distance > 90000){
			if(_sprite != walk) _sprite = walk;
			speed = Math.Clamp(speed + 1, -4, 4);
		}else{
			if(_sprite != shoot) _sprite = shoot;
			isActionInProgress = true;
			attackTimer = 1;
			_sprite.Trigger();
			speed = 0;
		}
	}

	float attackTimer = 1;
	public void endAttack(){
		isActionInProgress = false;
	}

	public override void Update(){
		Action();

		this.rotation = aiming_rotation;

		if(this.rotation < aiming_rotation) this.rotation = (this.rotation+5)%360;
		if(this.rotation > aiming_rotation) this.rotation = (this.rotation-5)%360;
	}
}
