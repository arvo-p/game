public class Autoaim{

	Entity parent;
	Environment env;

	public bool isAutoaiming = false;
	public float rotation = 0;
	
	List<Entity> sortedTargets = new List<Entity>();
	int index = 0;

	public Autoaim(Entity parent){
		this.env = Game.env;
		this.parent = parent;
	}

	public bool SelectNext(int dir){
		if(sortedTargets.Count == 0) return false;
		if(dir < 0) index++;
		if(dir > 0) index--;

		if(index < 0) index = sortedTargets.Count-1;
		index = (index%sortedTargets.Count);

		Entity target = sortedTargets[index];
		float dist = Tools.GetDistanceSquared(parent.r.Location, new PointF(target.r.X, target.r.Y));

		PointF difference = new PointF(parent.r.Y-target.r.Y,parent.r.X-target.r.X);
		float new_aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;

		rotation = new_aiming_rotation;
		isAutoaiming = true;

		return true;
	}

	public void UpdateList(){
		sortedTargets.Clear();

		foreach(var obj in env.nonplayer_entities){
			float dist = Tools.GetDistanceSquared(parent.r.Location, new PointF(obj.r.X, obj.r.Y));
			if(dist > 490000) continue; 

			PointF difference = new PointF(parent.r.Y-obj.r.Y,parent.r.X-obj.r.X);
			float aimingRotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180f;
			if(Tools.GetAngleDifference(parent.rotation, aimingRotation) > 180) continue;
			
			sortedTargets.Add(obj);
		}

		sortedTargets.Sort((a, b) => 
		{
			float distA = Tools.GetDistanceSquared(parent.r.Location, a.r.Location);
			float distB = Tools.GetDistanceSquared(parent.r.Location, b.r.Location);
			return distA.CompareTo(distB);
		});
	}
}
