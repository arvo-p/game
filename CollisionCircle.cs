public class CollisionCircle{

	public float offset{
		get => _offset;
	}
	public float radius{
		get => _radius;
		set{
			_radius = value;
		}
	}
	public PointF center{
		get => _center;
	}

	float _offset;
	float _radius;
	PointF _center;

	public CollisionCircle(float offset, float radius){
		this._offset = offset;
		this._radius = radius;
	}
	
	public static void UpdateCenters(List<CollisionCircle> l, RectangleF r){
		foreach(var cc in l){
			cc._center = new PointF(r.X+cc.radius,r.Y+cc.radius+cc.offset);
		}
	}
}
