public static class ItemDropEffects{
	public static bool Ammo(Player player, int quantity, ItemDrop.Type type){
		if(player.inventory==null) return false;
		player.inventory.AddAmmo(quantity, type);
		return true;
	}
}
