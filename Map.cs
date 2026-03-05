using System.Text;

public class Map{
	
	public int[,] map;
	public Image[] tileMap;
	public int tileDimension;
	public int tileRenderDimension;

	public Map(string[] filepathMap, string filepathTileset){
		if(File.Exists(filepathMap[0]) == false) throw new Exception("Map file inexistant");
		if(File.Exists(filepathTileset) == false) throw new Exception("Tileset inexistant");

		var dimension = GetMapDimension(filepathMap[0]);
		map = CreateMapArray(filepathMap[0],dimension);

		tileDimension = 128;
		tileRenderDimension = tileDimension/2;
		tileMap = ExtractTiles(filepathTileset, tileDimension);
		/*for(int i=0;i<mapArray.GetLength(0);i++){

		for(int j=0;j<mapArray.GetLength(1);j++){
			Console.Write(mapArray[i,j]+" ");
		}
		}*/
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

	(int, int) GetMapDimension(string path){

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
