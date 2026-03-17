public class Sprite{
	
	List<Image> frames = new List<Image>();
	int index=0;
	int length=0;
	int slowness=1;

	bool _isInfiniteLoop = true;
	bool isAnimationTriggered = false;
	bool _isAnimationFinished = false;
	int restingframe_idx = 0;

	public bool isInfiniteLoop{
		get => _isInfiniteLoop;
	}

	public bool isAnimationFinished{
		get => _isAnimationFinished;
	}

	public Image frame{
		get{
			if(length == 0) return frames[0];

			if(isInfiniteLoop == true){
				index = (index+1)%(length*slowness);
				return frames[index/slowness];
			}
			
			if(isAnimationTriggered == true){
				if((++index) >= length*slowness-1){
					isAnimationTriggered = false;
					_isAnimationFinished = true;
				}
				return frames[index/slowness];
			}else return frames[restingframe_idx];
		}
	}

	public Sprite(string[] filepaths){
		foreach(string pre_filepath in filepaths){
			string filepath = Resources.root + "/" + pre_filepath;
			Image i = Image.FromStream(new MemoryStream(File.ReadAllBytes(filepath)));
			frames.Add(i);
		}

		length = frames.Count;
		if(length < 8) slowness = 1;
		else if(length <= 5) slowness = 1; 
	}

	public Sprite(string[] filepaths, int restingframe_idx){
		this._isInfiniteLoop = false;
		this.restingframe_idx = restingframe_idx;

		foreach(string pre_filepath in filepaths){
			string filepath = Resources.root + "/" + pre_filepath;
			Image i = Image.FromStream(new MemoryStream(File.ReadAllBytes(filepath)));
			frames.Add(i);
		}

		length = frames.Count;
		if(length < 8) slowness = 4;
		if(restingframe_idx == -1) this.restingframe_idx = length-1;
	}

	private static Dictionary<string, Image> _imageLibrary = new Dictionary<string, Image>();
	
	public static Image GetImage(string filepath){
		filepath = Resources.root + "/" + filepath;
		
		if (!_imageLibrary.ContainsKey(filepath))
			_imageLibrary[filepath] = Image.FromStream(new MemoryStream(File.ReadAllBytes(filepath)));

		return _imageLibrary[filepath];
	}

	public Sprite(string[] filepaths, int restingframe_idx, int slowness, bool infinite){
		this._isInfiniteLoop = infinite;
		this.restingframe_idx = restingframe_idx;
		this.slowness = slowness;

		foreach(string pre_filepath in filepaths)
			frames.Add(GetImage(pre_filepath));

		length = frames.Count;
		if(restingframe_idx == -1) this.restingframe_idx = length-1;
	}

	public void Trigger(){
		_isAnimationFinished = false;
		index = 0;
		isAnimationTriggered = true;
	}

	public void Trigger(Action NextFunction){
		_isAnimationFinished = false;
		index = 0;
		isAnimationTriggered = true;
		while(!isAnimationFinished);
		NextFunction();
	}
}
