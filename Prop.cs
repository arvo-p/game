public class Prop{
	public Environment env = null!;
	public Sprite sprite;
	public RectangleF r;
	public float rotation;
	public int layer;

	public Image image{get => sprite.frame;}

	public Prop(string[] resources, RectangleF r, float rot){
		this.env = Game.env;
		this.rotation = rot;
		this.sprite = new Sprite(resources,0,6,true);
		this.r = r;
	}
	
	/*public void UpdatePosition(float x, float y, float rot){
		r.X = x;
		r.Y = y;
		this.rotation = rot;
	}*/
}
