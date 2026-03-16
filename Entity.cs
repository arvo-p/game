public class Entity : Object{
	
	protected bool isAttacking = false;
	protected bool isDead = false;
	protected bool isStunned = false;
	
	protected float maxspeed;
	
	int _health;
	public int maxhealth;
	public int health{
		get => _health;
		set{
			if(_health-value < 0) _health=0;
			if(health+value > maxhealth) _health=maxhealth;
		}
	}

	public bool HitscanCheck(PointF start, float range){
		float angle;
		angle = this.rotation*0.0174533f;

		PointF targetPoint = start;
		for (float d = 0; d < range; d += 16){
			targetPoint.X = start.X + (float)Math.Cos(angle) * d;
			targetPoint.Y = start.Y + (float)Math.Sin(angle) * d;

			var r = env.map.GetTileFromCoordinates(targetPoint);
			if(r.Item1 == -1 || env.map.collision[r.Item1, r.Item2] == 1){
				break;  
			}
		}

		float closestDistance = float.MaxValue;
		Object closestHit = null;

		foreach(var obj in env.objects){
			if(obj == this) continue;
			if(Tools.IsLineIntersectingRect(start, targetPoint, obj.r)){
				float dist = Tools.GetDistance(start, new PointF(obj.r.X, obj.r.Y));
				if(dist < closestDistance){
					closestDistance = dist; 
					closestHit = obj;
				}
			}
		}

		Game.draw.DebugSetLine(start, targetPoint);
		if(closestHit != null){
			closestHit.IsHit();
			return true;
		}

		return false;
	}

	public Entity(){
	}

	protected void setHealth(int health){
		this.maxhealth = health;
		this.health = health;
	}

	public override void IsHit(){
	}

	public override void Update(){
	}
}
