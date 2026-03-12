using System.Runtime.InteropServices;

public class Player : Entity{
	

	[DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
	private const int KEY_PRESSED = 0x8000;

	List<Weapon> weapons = new List<Weapon>();
	short idxSelectedWeapon = 0;
	short countWeapon = 0;
	
	public Weapon selectedWeapon{
		get{
			return countWeapon!=0?weapons[idxSelectedWeapon]:null;
		}
	}

	Sprite walk;
	Sprite meleethrow;  
	Sprite stand;
	Sprite death;
	Sprite fire;

	public Player(Environment env){
		this.env = env;

		LoadSprites();
		_sprite = stand;
		_spriteNext = stand;

		r.Location = new Point(0, 0);
		r.Size = new Size(74, 74);
		SetCollisionCircles();

		weapons.Add(
			(Weapon)env.weaponFootprints.list[3].Clone(this)
		);

		countWeapon = (short)weapons.Count; 
		idxSelectedWeapon = 0;
	}

	void LoadSprites(){
		walk = new Sprite(Resources.Player._walk);
		meleethrow = new Sprite(Resources.Player._meleethrow,0);
		stand = new Sprite(Resources.Player._stand);
		death = new Sprite(Resources.Player._death,-1);
		fire = new Sprite(Resources.Player._fire,0);
	}

	bool isActionInProgress = false;
	float attackTimer = 0;

	bool isAutoaiming = false;
	float autoaimRotation = 0;
	public bool AutoAim(){
		float closestDistance=float.MaxValue;
		float closestHit_angle=0;
		Entity closestHit=null;
		foreach(var obj in env.nonplayer_entities){
			PointF difference = new PointF(this.r.Y-obj.r.Y,this.r.X-obj.r.X);
			float new_aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;
			if(Math.Abs(this.rotation - new_aiming_rotation) > 140) continue;

			float dist = Tools.GetDistance(this.r.Location, new PointF(obj.r.X, obj.r.Y));
			if(dist < closestDistance){
				closestHit_angle = new_aiming_rotation;
				closestDistance = dist; 
				closestHit = obj;
			}
		}

		if(closestHit == null) return false;
		
		isAutoaiming = true;
		autoaimRotation = closestHit_angle;

		return true;
	}

	bool lastGunHitSuccessful = false;
	public void StartAttack(){
		isActionInProgress = true;
		if(countWeapon > 0){
			selectedWeapon.Shoot();
			lastGunHitSuccessful = HitscanCheck(this.center, 400);   
			_sprite = fire;
		}else{
			attackTimer = 1;
			sprite.Trigger();
			_sprite = meleethrow;
		}
		return;
	}

	bool ActionKeyPressed = true;
	public void HandleInput(){
		if( (GetAsyncKeyState((int)Keys.Q) & 0x8000) != 0 ) rotation -= 7;
		if( (GetAsyncKeyState((int)Keys.D) & 0x8000) != 0 ) rotation += 7; 
		
		if(isActionInProgress){
			if(selectedWeapon.sprite.isAnimationFinished){
				selectedWeapon.EndShoot();
				isActionInProgress = false;
			};
		}

		if((GetAsyncKeyState((int)Keys.Space) & 0x8000)!=0){
			if(lastGunHitSuccessful == false){
				if(AutoAim() == false) StartAttack();
			}else{
				StartAttack();
			}
			return;
		}
		lastGunHitSuccessful = false;

		if((GetAsyncKeyState((int)Keys.E) & 0x8000) != 0){
			if(!ActionKeyPressed){
				ActionKeyPressed = true;
				ActionKey();
			}
		} else ActionKeyPressed = false;
		
		if((GetAsyncKeyState((int)Keys.Z) & 0x8000) != 0 ) speed += 3;
		if((GetAsyncKeyState((int)Keys.S) & 0x8000) != 0 ) speed -= 3;
		speed = Math.Clamp(speed, -15, 15);
	}

	private void ActionKey(){
		Object collided = env.CheckObjectCollision(this, 10f);
		if(collided != null){
			MessageBox.Show(collided.GetType().Name);
		}
	}

	private Sprite UpdateSprite(){
		if(isDead) return death;
		if(speed > 0.2 || speed < -0.2) return walk;
		return stand;
	}

	public override void Update(){
		if(isAutoaiming == true){
			if(this.rotation < autoaimRotation) this.rotation = (this.rotation+10)%360;
			if(this.rotation > autoaimRotation) this.rotation = (this.rotation-10)%360;
			if(Math.Abs(this.rotation-autoaimRotation) < 10){
				isAutoaiming = false;
				StartAttack();
			}
		}

		HandleInput();
		
		if(isActionInProgress == false)
			_sprite = UpdateSprite();
	}
}
