public class Camera{
	
	Object follow;
	public RectangleF r{
		get{
			return follow.r;
		}
	}

	public Camera(){}

	public void Follow(Object f){
		this.follow = f;
	}
}
