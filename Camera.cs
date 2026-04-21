public class Camera{
	
	public Object? follow;
	public RectangleF r{get => follow?.r ?? RectangleF.Empty;}

	public Camera(){}

	public void Follow(Object f){
		this.follow = f;
	}
}
