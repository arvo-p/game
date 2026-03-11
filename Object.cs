public class Object{
	
	protected Environment env;
	
	public RectangleF r;
	public float speed;
	public float rotation;

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

	public void Collision(){
	}

	public virtual void Update(){
	}

	public void UpdateRoutine(){
		Update();
	
		if(speed < 0.2 && speed > -0.2) return;
		PointF movement = Scalar2Vect_Speed(rotation, speed); 
		env.Move(this, movement);
		speed *= (float)0.7;
	}

	public PointF GetCenter(){
		return new PointF(r.X + r.Width/2, r.Y + r.Height/2);
	}

	public virtual void IsHit(){

	}
}
