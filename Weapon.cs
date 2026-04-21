using System.Threading;
using System.Threading.Tasks;

public class Weapon{
	public required float damage{ get; set; }
	public required Size dimensions{ get; set; }
	public required Sprite sprite{ get; set; }
	public required short phase{ get; set; }
	Player? owner;

	public enum Type{
     	Melee,
		Gun,
	};
	public required Type type{get; set;}

	// control cooldown time 
	// player.cs : reload
	
	public required Sprite icon;
	public required ItemDrop.Type ammoType;

	public int currentClip;
	public int maxClip{ get; set; }

	public Image image{
		get => sprite.frame;
	}
	
	public Weapon(int maxClip){
		this.maxClip = maxClip;
		this.currentClip = maxClip;
	}
	
	
	public void Reload(){
		if(owner == null || owner.inventory == null) return;
		if(type == Type.Melee) return;
		
		ref int reserve = ref GetReserve(ammoType, owner.inventory);
		if(reserve <= 0) return;
		
		if(reserve > maxClip){
			currentClip = maxClip;
			reserve -= maxClip;
		}else{
			currentClip = maxClip;
			reserve = 0;
		}
	}

	private ref int GetReserve(ItemDrop.Type type, Inventory inv){
		switch(type){
			case ItemDrop.Type.Grenades: return ref inv.grenades;
			case ItemDrop.Type.Bullets: return ref inv.bullets;
			case ItemDrop.Type.Batteries: return ref inv.batteries;
			case ItemDrop.Type.Rockets: return ref inv.rockets;
			case ItemDrop.Type.Shells: return ref inv.shells;
			case ItemDrop.Type.Smallbullets: return ref inv.smallbullets;
			default: return ref inv.bullets;
		}
	}
	
	#region ICloneable Members
	public object Clone(Player owner){
		this.owner = owner;
		return this.MemberwiseClone();
	}
	#endregion

	bool isShooting = false;
	
	public bool Shoot(){
		if(isShooting) return false;
		if(type != Type.Melee && currentClip <= 0) Reload();
		else{
			isShooting = true;
			sprite.Trigger();
			currentClip--;
			return true;
		}
		return false;
	}
	
	public void EndShoot(){
		isShooting = false;
	}
}
