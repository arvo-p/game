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
	bool autoaimRotation = 0;
	public bool AutoAim(){
		float closestDistance;
		float closestHit_angle;
		Entity closestHit;
		foreach(var obj in env.nonplayer_entities){
			PointF difference = new PointF(this.Y-entity.r.Y,this.X-entity.r.X);
			float new_aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;
			if(Math.Abs(this.rotation - new_aiming_rotation) > 30) continue;

			float dist = Tools.GetDistance(start, new PointF(obj.r.X, obj.r.Y));
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

	public void StartAttack(){
		isActionInProgress = true;
		if(countWeapon > 0){
			selectedWeapon.Shoot();
			HitscanCheck(this.GetCenter(), 400);   
			_sprite = fire;
		}else{
			attackTimer = 1;
			sprite.Trigger();
			_sprite = meleethrow;
		}
		return;
	}

	public void HandleInput(){
		if( (GetAsyncKeyState((int)Keys.Q) & 0x8000) != 0 ) rotation = (rotation-7)%360;
		if( (GetAsyncKeyState((int)Keys.D) & 0x8000) != 0 ) rotation = (rotation+7)%360; 
		
		if(isActionInProgress){
			if(selectedWeapon.sprite.isAnimationFinished){
				selectedWeapon.EndShoot();
				isActionInProgress = false;
			}else return;
		}

		if((GetAsyncKeyState((int)Keys.Space) & 0x8000)!=0){
			if(AutoAim() == false) StartAttack();
			return;
		}

		if( (GetAsyncKeyState((int)Keys.Z) & 0x8000) != 0 ) speed += 3;
		if( (GetAsyncKeyState((int)Keys.S) & 0x8000) != 0 ) speed -= 3;
		speed = Math.Clamp(speed, -15, 15);
	}

	private Sprite UpdateSprite(){
		if(isDead) return death;
		if(speed > 0.2 || speed < -0.2) return walk;
		return stand;
	}

	public override void Update(){
		HandleInput();

		if(isAutoaiming == true){
			if(this.rotation < aiming_rotation) this.rotation = (this.rotation+5)%360;
			if(this.rotation > aiming_rotation) this.rotation = (this.rotation-5)%360;
			if(Math.Abs(this.rotation-aiming_rotation) < 5){
				isAutoaiming = false;
				StartAttack();
			}
		}

		if(isActionInProgress == false)
			_sprite = UpdateSprite();
	}
}
