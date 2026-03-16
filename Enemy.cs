public class Enemy : Entity{

	public Sprite walk;
	public Sprite shoot;  
	public Sprite stand;
	public Sprite dead;

	protected Player local_player;

	protected bool isActionInProgress = false;
	protected float aiming_rotation;
	
	PointF velRepulsion = new PointF(0,0);

	public float tAttack = 1;
	public float tStun = 1;
	
	protected Formation myFormation = null;
	public bool hasPosTarget=false;
	public PointF posTarget = new PointF();

	public void Action(){

		PointF difference = new PointF(this.r.Y-local_player.r.Y,this.r.X-local_player.r.X);
		aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;

		if(this.rotation-aiming_rotation > 180){
			this.rotation -= 180;
			speed *= -1;
		}

		if(isStunned == true){
			tStun+=-0.1f;
			if(tStun < 0) isStunned = false;
			else return;
		}
		
		/*
		 * NOT WORKING YET
		 * foreach(var other in env.nonplayer_entities){
    		float distsq = Tools.GetDistanceSquared(this.r.Location, other.r.Location);
    		if(distsq < 10000){
				if(distsq < 2){
					velRepulsion = new PointF(2, 2);
					continue;
				}
				float dist = (float)Math.Sqrt(distsq);
				PointF normDiff = new PointF((this.r.X-other.r.X)/dist, (this.r.Y-other.r.Y)/dist);
        		velRepulsion.X += normDiff.X*3;
        		velRepulsion.Y += normDiff.Y*3;
    		}
		}*/

		if(isActionInProgress){
			tAttack+=-0.052f;
			if(tAttack <= 0) isActionInProgress = false;
			else return;
		}
		
		float distance = Tools.GetDistanceSquared(local_player.r.Location, this.r.Location);
		if(distance > 90000){
			if(_sprite != walk) _sprite = walk;
			speed = Math.Clamp(speed + 1, -3, 3);
		}else{
			if(_sprite != shoot) _sprite = shoot;
			isActionInProgress = true;
			tAttack = 1;
			_sprite.Trigger();
			speed = 0;
		}
	}

	public void ActionFormation(){
		if(hasPosTarget == false) return;

		PointF difference = new PointF(this.r.Y-posTarget.Y,this.r.X-posTarget.X);
		aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;

		if(this.rotation-aiming_rotation > 180){
			this.rotation -= 180;
			speed *= -1;
		}

		float distance = Tools.GetDistanceSquared(local_player.r.Location, this.r.Location);
		if(distance > 0){
			if(_sprite != walk) _sprite = walk;
			speed = Math.Clamp(speed + 1, -3, 3);
		}
	}

	public override void IsHit(){
		_sprite = stand;
		speed = -5;
		
		isStunned = true;
		tStun = 1;
	}

	public override void UpdateRoutine(){
		if(myFormation == null) Action();
		else ActionFormation();

		this.rotation = aiming_rotation;

		if(this.rotation < aiming_rotation) this.rotation += 5;
		if(this.rotation > aiming_rotation) this.rotation -= 5;
		
		if(speed < 0.2 && speed > -0.2) return;
		
		PointF movement = Scalar2Vect_Speed(rotation, speed); 
		movement.X += velRepulsion.X;
		movement.Y += velRepulsion.Y;

		if(this.inverted_vectors) movement = Tools.SwapPointF(movement);
		env.Move(this, movement);
		speed *= (float)(friction);
	}
	
	public Enemy(){

	}
}
