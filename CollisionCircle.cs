public class CollisionCircle{

	private float _offset;
	public float offset{get => _offset;}

	float _radius;
	public float radius{get => _radius; set => _radius = value;}
	
	private PointF previousCenter;
	private PointF _center;
	public PointF center{
		set{
			previousCenter = _center;
			_center = value;
		}
		get => _center;
	}

	public Object parent;
	public PointF pcenter{get => parent.center;}
	public RectangleF pr{get => parent.r;}
	public float protation{get => parent.rotation;}

	public CollisionCircle(float offset, float radius, Object parent){
		this._offset = offset;
		this._radius = radius;
		this.parent = parent;
		
		_center = new PointF(parent.r.X+radius,parent.r.Y+offset+parent.r.Height/2);
	}
	
	public static void UpdateCenters(List<CollisionCircle> l, PointF mov){
		foreach(var cc in l){
			cc._center = new PointF(cc._center.X+mov.X,cc._center.Y+mov.Y);
		}
	}

	public void RollBackCenter(){
		
		_center = previousCenter;

		/*center = new PointF(
				(float)(Math.Sin(-rot_radians)*offset) + pcenter.X,
				(float)(Math.Cos(rot_radians)*offset) + pcenter.Y);*/
	}
}
