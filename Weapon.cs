using System.Threading;
using System.Threading.Tasks;

public class Weapon{
	
	public Size dimensions;
	Sprite sprite;
	short phase;
	Player owner;

	public Image image{
		get => sprite.frame;
	}

	public Weapon(string[] resources, Size dimensions, short phase, Projectile projectile){
		this.sprite = new Sprite(resources, 0);
		this.dimensions = dimensions;
		this.phase = phase;
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
		new Task(() => {sprite.Trigger(EmitProjectile);}).Start();
	}

	public void EmitProjectile(){
		isShooting = false;
		owner.endAttack();
	}
}
