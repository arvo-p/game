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


	public Entity(){
	}

	protected void setHealth(int health){
		this.maxhealth = health;
		this.health = health;
	}

	public override void Update(){
	}
}
