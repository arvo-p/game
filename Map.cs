using System.Text;

public class Map{
	
	public Image[,] gmap;
	public int[,] map;
	public int[,] collision;

	public Size worldsize;

	public Image[] tileMap;
	public int tileDimension;
	public int tileRenderDimension;

	public Map(string[] filepathMap, string filepathTileset){
		if(File.Exists(filepathMap[0]) == false) throw new Exception("Map file inexistant");
		if(File.Exists(filepathTileset) == false) throw new Exception("Tileset inexistant");

		var dimension = GetMapDimension(filepathMap[0]);

		map = CreateMapArray(filepathMap[0],dimension);
		int[,] map_layer2 = CreateMapArray(filepathMap[1],dimension);

		collision = CreateMapArray(filepathMap[2],dimension);

		tileDimension = 128;
		tileRenderDimension = 64;
		tileMap = ExtractTiles(filepathTileset, tileDimension);

		gmap = BuildMapImages(map, map_layer2, tileMap);
		this.worldsize = new Size(dimension.width*tileRenderDimension, dimension.height*tileRenderDimension);

		/*for(int i=0;i<mapArray.GetLength(0);i++){

		for(int j=0;j<mapArray.GetLength(1);j++){
			Console.Write(mapArray[i,j]+" ");
		}
		}*/
	}

	Image[,] BuildMapImages(int[,] pmap, int[,] map_layer2, Image[] pTiles){
		Size mapSize = new Size(tileRenderDimension*map.GetLength(0), tileRenderDimension*map.GetLength(1));
		int maxColumns = (int)Math.Round((float)mapSize.Width/Game.windowWidth, MidpointRounding.AwayFromZero);
		int maxRows = (int)Math.Round((float)mapSize.Height/Game.windowHeight, MidpointRounding.AwayFromZero);

		Image[,] mapImages = new Image[maxColumns, maxRows]; 
		for(int i=0;i<maxColumns;i++){
			for(int j=0;j<maxRows;j++){
				mapImages[i,j] = BuildMapImage(pmap, map_layer2, pTiles, i, j, mapSize);
				Console.WriteLine(i + " " + j + " written");
			}
		}
	
		return mapImages;
	}

	Image BuildMapImage(int[,] pmap, int[,] pmap2, Image[] pTiles, int column, int row, Size mapSize){
		Image mapImage = new Bitmap(Game.windowWidth+1, Game.windowHeight+1);
		Point position = new Point(column * Game.windowWidth, row * Game.windowHeight);

		if(position.X > mapSize.Width) return null;
		if(position.Y > mapSize.Height) return null;

		using (Graphics g = Graphics.FromImage(mapImage)){
			int initI = (int)position.X/tileRenderDimension;
			int maxCol = (int)(position.X+Game.windowWidth)/tileRenderDimension;
			if(maxCol > pmap.GetLength(0)) maxCol = pmap.GetLength(0);

			int initJ = (int)position.Y/tileRenderDimension;
			int maxRow = (int)(position.Y+Game.windowHeight)/tileRenderDimension;	
			if(maxRow > pmap.GetLength(1)) maxRow = pmap.GetLength(1);

			int i,j;
			for(i=initI;i<maxCol;i++){
				for(j=initJ;j<maxRow;j++){
					if(map[i,j] == -1) continue;
					g.DrawImage(tileMap[pmap[i,j]], (i-initI)*tileRenderDimension, (j-initJ)*tileRenderDimension, tileRenderDimension+1, tileRenderDimension+1); 
					if(pmap2[i,j] == -1) continue;
					g.DrawImage(tileMap[pmap2[i,j]], (i-initI)*tileRenderDimension, (j-initJ)*tileRenderDimension, tileRenderDimension+1, tileRenderDimension+1); 
				}
			}
		}

		return mapImage;
	}

	public (int x, int y) GetTileFromCoordinates(float x, float y){
		int c = (int)Math.Floor(x/tileRenderDimension);
		if(c >= collision.GetLength(0) || c < 0) return (-1,-1);
		
		int r = (int)Math.Floor(y/tileRenderDimension);
		if(r >= collision.GetLength(1) || r < 0) return (-1,-1);

		return (c, r);
	}

	public (int x, int y) GetTileFromCoordinates(PointF dot){
		return GetTileFromCoordinates(dot.X, dot.Y);
	}

	Image[] ExtractTiles(string filepath, int tileSize){
		
		Image tileset = Image.FromStream(new MemoryStream(File.ReadAllBytes(filepath)));

		int columns = tileset.Width / tileSize;
		int rows = tileset.Height / tileSize;
		
		Image[] tileMap = new Image[rows*columns];

		for(int r=0; r<rows; r++){
			for(int c=0;c<columns; c++){
				tileMap[ (r*columns)+c ] = ExtractTile(tileset, c, r, tileSize);
			}
		}
		
		return tileMap;
	}

	Image ExtractTile(Image tileset, int column, int row, int tileSize){
		int x = column * tileSize;
		int y = row * tileSize;

		Image tile = new Bitmap(tileSize, tileSize);

		using (Graphics g = Graphics.FromImage(tile))
		{
			Rectangle sourceRect = new Rectangle(x, y, tileSize, tileSize);
			Rectangle destRect = new Rectangle(0, 0, tileSize, tileSize);
			g.DrawImage(tileset, destRect, sourceRect, GraphicsUnit.Pixel);
		}
	
		return tile;
	}

	(int width, int height) GetMapDimension(string path){

		int countLine=0;
		int countCommas = 0;

		const Int32 BufferSize = 512;
		string line;
		
		var fileStream = File.OpenRead(path);
		var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize);
		
		while ((line = streamReader.ReadLine()) != null){
			countLine++;
			if(countLine != 1) continue;
			countCommas = line.Count(f => f == ',')+1;
		}
		
		return (countLine, countCommas);
	}

	int[,] CreateMapArray(string path, (int, int) dimension){
		int[,] newArray = new int[dimension.Item2, dimension.Item1];

		const Int32 BufferSize = 512;
		string line;
		
		var fileStream = File.OpenRead(path);
		var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize);
		
		int j = 0;
		while ((line = streamReader.ReadLine()) != null){
			int i = 0;
			foreach(var strNum in line.Split(',')){
				newArray[i,j] = Int32.Parse(strNum);
				i++;
			}
			j++;
		}
		
		return newArray;
	}
}
