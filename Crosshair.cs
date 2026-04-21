public class Crosshair{
	public bool isOn = false;
	public RectangleF r;
	
	bool hasTarget=false;
	public bool isLockedOnTarget=false;

	PointF target;
	
	Sprite onTarget;
	Sprite defaultC;

	Sprite sprite;

	public Image image{get => sprite.frame;}
	
	public Crosshair(string[] crosshair1, string[] crosshair2){
		r.Size = new Size(70, 70);
		r.Location = new PointF(20, 20);

		defaultC = new Sprite(crosshair1);
		onTarget = new Sprite(crosshair2);
		
		sprite = defaultC;
	}

	public void Update(){
		if(hasTarget == false || isLockedOnTarget == true) return;
		float lerpFactor = 0.15f;

		PointF diff = new PointF(target.X - r.X, target.Y - r.Y);
		r.Y += diff.Y*lerpFactor;
		r.X += diff.X*lerpFactor;

		if(Math.Abs(diff.Y) < 4 && Math.Abs(diff.X) < 4){
			sprite = onTarget;
			hasTarget = false;
			isLockedOnTarget = true;
		}
	}

	public void NewTarget(float x, float y){
		hasTarget = true;
		isLockedOnTarget = false;
		sprite = defaultC; 
		target = new PointF(x-r.Width/2, y-r.Height/2);
	}

	public void SetPosition(float x, float y){
		r.X = x-r.Width/2;
		r.Y = y-r.Height/2;
	}
}
