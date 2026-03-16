using System.Windows.Input;

public class Player : Entity{
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

	Autoaim autoaim;
	Keyboard mykeyboard;

	public Player(){
		this.env = Game.env;

		mykeyboard = new Keyboard(new Keys[]{Keys.Z, Keys.Q, Keys.S, Keys.D, Keys.E, Keys.Space});
		autoaim = new Autoaim(this);

		LoadSprites();
		_sprite = stand;
		_spriteNext = stand;

		setHealth(100);
		
		r.Location = new Point(0, 0);
		r.Size = new Size(74, 74);
		mass = 90;
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

	bool isAttacking = false;
	float attackTimer = 0;
	bool lastGunHitSuccessful = false;
	public void StartAttack(){
		isAttacking = true;
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

	private void HandleInput(){
		mykeyboard.ReadKeys();

		if(isAttacking){
			if(mykeyboard.GetKeyOnce(Keys.Q)){
				if(isAttacking) autoaim.SelectNext(-1);
			}

			if(mykeyboard.GetKeyOnce(Keys.D)){
				if(isAttacking) autoaim.SelectNext(1);
			}

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
			} StartAttack();
			speed = 0;
			return;
		}
		lastGunHitSuccessful = false;

		if(mykeyboard.GetKeyOnce(Keys.E)){
			bool leave = ActionKey();
			if(leave) return;
		}
		
		if(mykeyboard.GetKey(Keys.Z)) speed += 3;
		if(mykeyboard.GetKey(Keys.S)) speed -= 3;
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
			float diff = Tools.GetAngleDifference(this.rotation, autoaim.rotation);
			if(diff > 0) this.rotation = (this.rotation+10)%360;
			if(diff < 0) this.rotation = (this.rotation-10)%360;
			if(Math.Abs(diff) < 10){
				autoaim.isAutoaiming = false;
				StartAttack();
			}
		}

		if(mykeyboard != null) HandleInput();
		
		if(isAttacking == false)
			_sprite = UpdateSprite();
	}
}
