public class Vehicle : Entity{
	
	public Vehicle(Environment env){
		this.env = env;

		_sprite = new Sprite(Resources.Vehicle._car);
		r.Location = new Point(600, 600);
		r.Size = new Size((int)(95*1.2), (int)(210*1.2));
		SetCollisionCircles();
	}
}
