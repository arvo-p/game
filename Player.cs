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
	public void startAttack(){
		isActionInProgress = true;
		if(countWeapon > 0){
			selectedWeapon.Shoot();
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
			startAttack();
			/* TO-DO : auto-aim nearest enemy
			 */
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
		if(isActionInProgress == false)
			_sprite = UpdateSprite();
	}
}
