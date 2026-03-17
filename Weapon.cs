using System.Threading;
using System.Threading.Tasks;

public class Weapon{
	public float damage;
	public Size dimensions;
	public Sprite sprite;
	short phase;
	Player owner;

	public Image image{
		get => sprite.frame;
	}

	public Weapon(string[] resources, Size dimensions, short phase, Projectile projectile, float damage){
		this.sprite = new Sprite(resources, 0);
		this.dimensions = dimensions;
		this.phase = phase;
		this.damage = 35;
	}
	
	#region ICloneable Members
	public object Clone(Player owner){
		this.owner = owner;
		return this.MemberwiseClone();
	}
	#endregion

	bool isShooting = false;
	public void Shoot(){
		if(isShooting) return;
		isShooting = true;
		sprite.Trigger();
	}

	public void EndShoot(){
		isShooting = false;
	}
}
