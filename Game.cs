using System.Windows.Forms;

public static class Game{
	public static Environment env;
	public static Map map;
	public static Draw draw;
	public static int windowWidth;
	public static int windowHeight;

	private static System.Windows.Forms.Timer gametimer = new System.Windows.Forms.Timer();

	public static void Init(Form window, int pwindowWidth, int pwindowHeight){
		windowWidth = pwindowWidth;
		windowHeight = pwindowHeight;

		env = new Environment();
		draw = new Draw(env, windowWidth, windowHeight);
		
		map = new Map(new string[]{"Resources/Tiles/test_Tile Layer 1.csv","Resources/Tiles/test_Tile Layer 2.csv"}, "Resources/Tiles/tilesheet.png");

		gametimer.Interval = 18;
		gametimer.Tick += (s, e) => {
			env.Update();
			window.Invalidate();
		};
		gametimer.Start();
	}
}
