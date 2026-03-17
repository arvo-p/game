using System.Windows.Input;

public class Player : Entity{
	List<Weapon> weapons = new List<Weapon>();
	
	short countWeapon = 0;
	short _idxSelectedWeapon = 0;
	public short idxSelectedWeapon{get=>_idxSelectedWeapon;set=>_idxSelectedWeapon=value<countWeapon?value:(short)(countWeapon-1);}
	public Weapon selectedWeapon{get => countWeapon!=0?weapons[_idxSelectedWeapon]:null;}

	Sprite walk;
	Sprite meleethrow;  
	Sprite stand;
	Sprite death;
	Sprite fire;

	Autoaim autoaim;
	Keyboard mykeyboard;

	public Player(Crosshair crosshair){
		this.env = Game.env;

		mykeyboard = new Keyboard(new Keys[]{Keys.Z, Keys.Q, Keys.S, Keys.D, Keys.E, Keys.Space, Keys.D1, Keys.D2, Keys.D3, Keys.D4});
		autoaim = new Autoaim(this, crosshair);

		LoadSprites();
		_sprite = stand;
		_spriteNext = stand;

		setHealth(100);
		
		r.Location = new Point(0, 0);
		r.Size = new Size(70, 70);
		mass = 90;
		SetCollisionCircles();

		weapons.AddRange(
			(Weapon)env.weaponFootprints.list[4].Clone(this),
			(Weapon)env.weaponFootprints.list[5].Clone(this),
			(Weapon)env.weaponFootprints.list[6].Clone(this)
		);
		
		countWeapon = (short)weapons.Count; 
		idxSelectedWeapon = 3;
	}

	void LoadSprites(){
		walk = new Sprite(Resources.Player._walk);
		meleethrow = new Sprite(Resources.Player._meleethrow,0);
		stand = new Sprite(Resources.Player._stand);
		death = new Sprite(Resources.Player._death,-1);
		fire = new Sprite(Resources.Player._fire,0);
	}

	float attackTimer = 0;
	bool lastGunHitSuccessful = false;
	public void StartAttack(){
		isAttacking = true;
		if(countWeapon > 0){
			selectedWeapon.Shoot();
			Object who = HitscanCheck(this.center, 400);   
			if(lastGunHitSuccessful = (who != null))
				who.IsHit(selectedWeapon.damage, rotation);
			_sprite = fire;
		}else{
			attackTimer = 1;
			sprite.Trigger();
			_sprite = meleethrow;
		}
		return;
	}

	private void HandleInput(){
		mykeyboard.ReadKeys();

		if(mykeyboard.GetKeyOnce(Keys.D1)) idxSelectedWeapon = 0;
		if(mykeyboard.GetKeyOnce(Keys.D2)) idxSelectedWeapon = 1;
		if(mykeyboard.GetKeyOnce(Keys.D3)) idxSelectedWeapon = 2;
		if(mykeyboard.GetKeyOnce(Keys.D4)) idxSelectedWeapon = 3;

		if(autoaim.isAutoaiming){
			if(mykeyboard.GetKeyOnce(Keys.Q)){
				autoaim.SelectNext(-1);
			}

			if(mykeyboard.GetKeyOnce(Keys.D)){
				autoaim.SelectNext(1);
			}
		}

		if(isAttacking){
			if(selectedWeapon.sprite.isAnimationFinished){
				selectedWeapon.EndShoot();
				isAttacking = false;
			}
			return;
		}

		if(mykeyboard.GetKey(Keys.Q))
			rotation -= 7;

		if(mykeyboard.GetKey(Keys.D))
			rotation += 7;
		
		if(mykeyboard.GetKey(Keys.Space)){
			if(lastGunHitSuccessful == false){
				autoaim.UpdateList();
				if(!autoaim.SelectNext(0))
				StartAttack();
			}
			StartAttack();
			speed = 0;
			return;
		}

		if(mykeyboard.GetKeyOnce(Keys.E)){
			bool leave = ActionKey();
			if(leave) return;
		}
		
		if(mykeyboard.GetKey(Keys.Z)){
			lastGunHitSuccessful = false;
			autoaim.Set(false);
			speed += 3;
		}

		if(mykeyboard.GetKey(Keys.S)){
			lastGunHitSuccessful = false;
			autoaim.Set(false);
			speed -= 3;
		}

		speed = Math.Clamp(speed, -15, 15);
	}

	private bool ActionKey(){
		Object collided = env.IsObjectColliding(this, 10f, null, 0);
		if(collided != null){
			if(collided.GetType().Name == "Vehicle"){
				Vehicle vehicle = (Vehicle)collided;
				vehicle.MountPassenger(this,mykeyboard);
				mykeyboard = null;
				return true;
			}
		}
		return false;
	}

	private Sprite UpdateSprite(){
		if(isDead) return death;
		if(speed > 0.2 || speed < -0.2) return walk;
		return stand;
	}

	public override void Update(){
		if(autoaim.isAutoaiming == true){
			if(autoaim.crosshair.isOn) autoaim.crosshair.Update();
			float diff = Tools.GetAngleDifference(this.rotation, autoaim.rotation);
			float lerpFactor = 0.1f;
			this.rotation += diff*lerpFactor;
		}

		if(mykeyboard != null) HandleInput();
		if(isAttacking == false)
			_sprite = UpdateSprite();
	}
}
