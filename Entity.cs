public class Entity : Object{
	
	protected bool isAttacking = false;
	protected bool isDead = false;
	
	int _health;
	public int maxhealth;
	public int health{
		get => _health;
		set{
			if(_health-value < 0) _health=0;
			if(health+value > maxhealth) _health=maxhealth;
		}
	}

	public bool HitscanCheck(PointF start, float angle, float range, RectangleF target){
		for (float d = 0; d < range; d += 32){
			float checkX = start.X + (float)Math.Cos(angle) * d;
			float checkY = start.Y + (float)Math.Sin(angle) * d;

			if (target.Contains(checkX, checkY)) return true;
		}
		return false;
	}

	public Entity(){
	}

	protected void setHealth(int health){
		this.maxhealth = health;
		this.health = health;
	}

	public override void Update(){
	}
}
