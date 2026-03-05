using System.Runtime.InteropServices;

public class Player : Entity{
	
	Environment env;

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
		r.Size = new Size(75, 75);
		
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
	public void startAttack(){
		isActionInProgress = true;
		if(countWeapon > 0){
			selectedWeapon.Shoot();
			_sprite = fire;
		}else{
			new Task(() => {sprite.Trigger(endAttack);}).Start();
			_sprite = meleethrow;
		}
		return;
	}

	public void endAttack(){
		isActionInProgress = false;
	}

	public void HandleInput(){
		if(isActionInProgress) return;
		
		if((GetAsyncKeyState((int)Keys.Space) & 0x8000)!=0){
			startAttack();
			return;
		}

		if( (GetAsyncKeyState((int)Keys.Z) & 0x8000) != 0 ) speed += 3;
		if( (GetAsyncKeyState((int)Keys.S) & 0x8000) != 0 ) speed -= 3;
		if( (GetAsyncKeyState((int)Keys.Q) & 0x8000) != 0 ) rotation = (rotation-6)%360;
		if( (GetAsyncKeyState((int)Keys.D) & 0x8000) != 0 ) rotation = (rotation+6)%360; 
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

		PointF movement = Scalar2Vect_Speed(rotation, speed); 
		r.Location = new PointF(movement.X + r.X, movement.Y + r.Y);
		
		if(speed > 4) speed = 4;
		if(speed < -4) speed = -4;
		speed *= (float)0.7;
	}
}
