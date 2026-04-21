public class Autoaim{

	Entity parent;
	Environment env;
	public Crosshair crosshair;

	public bool isAutoaiming = false;
	public float rotation = 0;
	
	List<Entity> sortedTargets = new List<Entity>();
	int index = 0;

	public Autoaim(Entity parent, Crosshair crosshair){
		this.env = Game.env;
		this.parent = parent;
		this.crosshair = crosshair;
	}

	public bool SelectNext(int dir){
		crosshair.isOn = true;

		if(sortedTargets.Count == 0) return false;
		if(dir < 0) index++;
		if(dir > 0) index--;

		if(index < 0) index = sortedTargets.Count-1;
		index = (index%sortedTargets.Count);

		Entity target = sortedTargets[index];
		if(target.isDead){
			sortedTargets.RemoveAt(index);
			return SelectNext(dir);
		}

		float dist = Tools.GetDistanceSquared(parent.r.Location, new PointF(target.r.X+target.r.Width/2, target.r.Y+target.r.Height/2));

		PointF difference = new PointF(parent.r.Y-target.r.Y,parent.r.X-target.r.X);
		float new_aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;

		rotation = new_aiming_rotation;
		isAutoaiming = true;

		crosshair.NewTarget(target.center.X, target.center.Y);

		return true;
	}

	private float AngleDifference(Entity me, Entity other){
		PointF difference = new PointF(me.r.Y-other.r.Y,me.r.X-other.r.X);
		float aimingRotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180f;
		return Tools.GetAngleDifference(me.rotation, aimingRotation);
	}

	public bool UpdateList(){
		sortedTargets.Clear();
		index = 0;

		int size = 0;
		foreach(var ent in env.All.Entities.NPCs){
			if(ent.isDead == true) continue;
			float dist = Tools.GetDistanceSquared(parent.r.Location, new PointF(ent.r.X, ent.r.Y));
			if(dist > 360000) continue; 
			sortedTargets.Add(ent);
			size++;
		}

		if(size == 0) return false;

		sortedTargets.Sort((a, b) => 
		{
			float angA = AngleDifference(parent, a);
			float angB = AngleDifference(parent, b);
			return angA.CompareTo(angB);
		});
		
		// order by angle first and then distance
		sortedTargets.Sort((a, b) =>
		{
			float dist1 = Tools.GetDistanceSquared(parent.r.Location, a.r.Location);
			float dist2 = Tools.GetDistanceSquared(parent.r.Location, b.r.Location);
			return dist1.CompareTo(dist2);
		});

		crosshair.SetPosition(sortedTargets[0].center.X, sortedTargets[0].center.Y);
		return true;
	}

	public void Set(bool c){
		isAutoaiming = c;
		crosshair.isOn = c;
	}
}
