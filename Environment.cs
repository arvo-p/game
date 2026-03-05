public class Environment{

	public WeaponFootprints weaponFootprints = new WeaponFootprints();

	public List<Object> objects = new List<Object>();
	public List<Entity> nonplayer_entities = new List<Entity>();
	public List<Entity> entities = new List<Entity>();
	public List<Entity> players = new List<Entity>();

	public Player p;

	public Environment(){
		p = new Player(this);
		nonplayer_entities.Add(new Thug(this));
		entities.AddRange(nonplayer_entities);
		entities.Add(p);
		players.Add(p);
		//objects.Add();
	}
	
	public void Update(){
		foreach(var obj in objects)
			obj.Update();

		foreach(var e in entities)
			e.Update();

		DoCollisionVerifications(objects, objects);
		DoCollisionVerifications(objects, entities);
		DoCollisionVerifications(entities, entities);
	}

	private void DoCollisionVerifications(IEnumerable<Object> list1, IEnumerable<Object> list2){
		foreach(var obj in list1){
			foreach(var obj2 in list2){
				if(Object.ReferenceEquals(obj2, obj)) continue;
				if(obj.r.IntersectsWith(obj2.r)){
					obj.Collision();
					obj2.Collision();
				}
			}
		}
	}
}
