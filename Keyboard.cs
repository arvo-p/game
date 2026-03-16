using System.Runtime.InteropServices;
using System.Windows.Input;

public class Keyboard{

	[DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vkey);
	private const int KEY_PRESSED = 0x8000;

	List<Keys> registeredkeys = new List<Keys>();
	List<Keys> keysPressed = new List<Keys>();
	List<Keys> keysPressedOnce = new List<Keys>();

	public Keyboard(Keys[] keys){
		foreach(var key in keys){
			registeredkeys.Add(key);
		}
	}

	public void ReadKeys(){
		foreach(Keys k in registeredkeys){
			if((GetAsyncKeyState((int)k) & KEY_PRESSED) != 0){
				if(keysPressed.Contains(k) == false){
					keysPressed.Add(k);
					keysPressedOnce.Add(k);
				}
			}else{
				keysPressed.Remove(k);
				keysPressedOnce.Remove(k);
			}
		}
	}

	public bool GetKey(Keys k){
		return keysPressed.Contains(k);
	}

	public bool GetKeyOnce(Keys k){
		return keysPressedOnce.Remove(k);
	}
}
