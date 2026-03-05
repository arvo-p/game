public class WeaponFootprints{
	public List<Weapon> list = new List<Weapon>();
	
	public WeaponFootprints(){
		list.AddRange(
			new Weapon(Resources.Weapon._weapon1, new Size(20, 20), 1000, new Projectile()),
			new Weapon(Resources.Weapon._weapon2, new Size(20, 20), 1000, new Projectile()),
			new Weapon(Resources.Weapon._weapon3, new Size(20, 20), 1000, new Projectile()),
			new Weapon(Resources.Weapon._weapon4, new Size(20, 20), 1000, new Projectile())
		);
	}
}
