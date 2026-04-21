public static class ItemDropFootprints{
	public static List<ItemDrop> ammo = new List<ItemDrop>();
	public static void Init(){
		ammo.Add(new ItemDrop(Resources.Item._bullet, ItemDropEffects.Ammo, ItemDrop.Type.Bullets, 64));
		ammo.Add(new ItemDrop(Resources.Item._smallbullets, ItemDropEffects.Ammo, ItemDrop.Type.Bullets, 64));
	}
	
	public static ItemDrop SelectRandom(){
     	Random r = new Random();
		int index = r.Next(0, ammo.Count());
		return ammo[index];
	}
}
