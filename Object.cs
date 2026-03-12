public class Object{
	
	protected Environment env;
	
	public RectangleF r;
	public float speed;
	public float rotation{
		set{
			if(value < 0) _rotation =(value+360)%360;
			else _rotation = value%360;
		}
		get => _rotation;
	}


	PointF? _center=null;
	public PointF center{
		get{
			if(_center == null) PositionUpdated();
			return _center.Value;
		}
	}

	public List<CollisionCircle> hitboxes = new List<CollisionCircle>();

	float _rotation;

	public Sprite _sprite;
	public Sprite _spriteNext;

	public virtual Sprite sprite{
		get => _sprite;
	}

	public Image image{
		get => sprite.frame;
	}

	public Object(){
	}
	
	public PointF Scalar2Vect_Speed(float rot, float scalarspeed){
		double rot_radians = (rot)*0.0174533; 
		return new PointF(
				(float)(scalarspeed*Math.Cos(rot_radians)),
				(float)(scalarspeed*Math.Sin(rot_radians))
		);
	}

	public virtual void Update(){
	}

	public void Collision(Object obj){
	}

	public void UpdateRoutine(){
		Update();
	
		if(speed < 0.2 && speed > -0.2) return;
		PointF movement = Scalar2Vect_Speed(rotation, speed); 
		env.Move(this, movement);
		speed *= (float)0.7;
	}

	public virtual void IsHit(){

	}

	public void PositionUpdated(){
		_center = new PointF(r.X+r.Width/2,r.Y+r.Height/2);
		CollisionCircle.UpdateCenters(hitboxes,r);
	}

	protected void SetCollisionCircles(){
		int numCircles = (int)Math.Round(r.Height/r.Width, MidpointRounding.AwayFromZero);
		float remainingHeight = r.Height;
		Console.WriteLine("Creating " + numCircles + "collision circles for " + this.GetType().Name);
		for(int i=0;i<numCircles;i++){
			CollisionCircle newCC = new CollisionCircle(r.Height - remainingHeight, r.Width/2);
			remainingHeight -= r.Width;
			hitboxes.Add(newCC);
		}
		CollisionCircle.UpdateCenters(hitboxes,r);
	}
}
