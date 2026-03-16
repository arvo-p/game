public class Object{
	
	protected Environment env;
	
	public RectangleF r;
	public float speed;
	protected bool inverted_vectors=false;
	protected int mass;
	protected float friction=0.7f;

	public bool isSolid = true;
	
	float _rotation;
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
		PointF speed = new PointF(
			(float)(scalarspeed*Math.Cos(rot_radians)),
			(float)(scalarspeed*Math.Sin(rot_radians))
		);
		return speed;
	}

	public virtual void Update(){
	}

	public void Collision(Object obj){
	}

	public virtual void UpdateRoutine(){
		Update();
	
		if(speed < 0.2 && speed > -0.2) return;
		PointF movement = Scalar2Vect_Speed(rotation, speed); 
		if(this.inverted_vectors){
			movement = Tools.SwapPointF(movement);
		}
		env.Move(this, movement);
		speed *= (float)(friction);
	}

	public virtual void IsHit(){

	}

	public void PositionUpdated(){
		_center = new PointF(r.X+r.Width/2,r.Y+r.Height/2);
	}

	protected void SetCollisionCircles(){
		float radius = r.Height>r.Width?r.Width/2:r.Height/2;
		float totalLength = r.Height>r.Width?r.Height:r.Width;
		float remainingLength = totalLength;
		
		int numCircles = (int)Math.Ceiling(r.Height/r.Width);
		Console.WriteLine("Creating " + numCircles + "collision circles for " + this.GetType().Name);

		for(int i=0;i<numCircles;i++){
			float offset = (i==numCircles-1)?totalLength-radius*2:(totalLength-remainingLength); 
			offset += -(totalLength/2) + radius;
			
			CollisionCircle newCC = new CollisionCircle(offset, radius, this);
			remainingLength -= radius*2;
			hitboxes.Add(newCC);
		}
	}
}
