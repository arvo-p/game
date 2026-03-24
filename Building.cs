public class Building{
    public float X, Y, Width, Height, Height3D;
	
	private static int counterElements = 0;
	private static string[] srcWalls = new string[]{"Tiles/Buildings/Wall1.png"};

	public Sprite roof;
	public Sprite wall;

    public Building(float x, float y, float w, float h, float tileRenderDimension, float h3D){
		X = x*tileRenderDimension+10;
		Y = y*tileRenderDimension+10;
		Width = w*tileRenderDimension-20;
		Height = h*tileRenderDimension-20;
		Height3D = h3D;
		wall = new Sprite(new string[]{srcWalls[counterElements % srcWalls.Count()]});

		counterElements++;
	}
}
