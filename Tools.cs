public static class Tools{
	public static bool IsLineIntersectingRect(PointF p1, PointF p2, RectangleF r){
		if (r.Contains(p1) || r.Contains(p2)) return true;

		return LineIntersectsLine(p1, p2, new PointF(r.Left, r.Top), new PointF(r.Right, r.Top)) ||
			   LineIntersectsLine(p1, p2, new PointF(r.Left, r.Bottom), new PointF(r.Right, r.Bottom)) ||
			   LineIntersectsLine(p1, p2, new PointF(r.Left, r.Top), new PointF(r.Left, r.Bottom)) ||
			   LineIntersectsLine(p1, p2, new PointF(r.Right, r.Top), new PointF(r.Right, r.Bottom));
	}

	public static float RandomFloat(int min, int max){
		int diff = max-min;
		return (Random.Shared.NextSingle() * (float)diff)+(float)min;
	}

	public static bool IsColliding(RectangleF a, RectangleF b){
		if (a.X + a.Width  < b.X) return false; 
		if (a.X > b.X + b.Width)  return false;
		if (a.Y + a.Height < b.Y) return false; 
		if (a.Y > b.Y + b.Height) return false;

		return true;
	}

	public static bool LineIntersectsLine(PointF a1, PointF a2, PointF b1, PointF b2){
		float d = (a2.X - a1.X) * (b2.Y - b1.Y) - (a2.Y - a1.Y) * (b2.X - b1.X);
		if (d == 0) return false; // Parallel lines

		float u = ((b1.X - a1.X) * (b2.Y - b1.Y) - (b1.Y - a1.Y) * (b2.X - b1.X)) / d;
		float v = ((b1.X - a1.X) * (a2.Y - a1.Y) - (b1.Y - a1.Y) * (a2.X - a1.X)) / d;

		return (u >= 0 && u <= 1) && (v >= 0 && v <= 1);
	}
	
	public static float GetDistance(PointF p1, PointF p2){
		return (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
	}
	
	public static float GetDistanceSquared(PointF p1, PointF p2){
		return (float)(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
	}
	
	public static float GetAngleDifference(float angle1, float angle2){
		float diff = (angle2 - angle1 + 180) % 360 - 180;
		return diff < -180 ? diff + 360 : diff;
	}

	public static bool IsCircleColliding(PointF center1, float radius1, PointF center2, float radius2){
		float combinedradius = radius1 + radius2;
		
		float dx = center1.X - center2.X;
		float dy = center1.Y - center2.Y;
		float distanceSquared = (dx * dx) + (dy * dy);

		return distanceSquared < (combinedradius * combinedradius);
	}

	public static bool IsCircleColliding(CollisionCircle obj1, CollisionCircle obj2){
		return IsCircleColliding(obj1.center, obj1.radius, obj2.center, obj2.radius); 
	}

	public static PointF SwapPointF(PointF p){
		float holder = p.X;
		p.X = -p.Y;
		p.Y = holder;

		return p;
	}

	// not used
	public static PointF RotateVector(PointF vector, float rotationDegrees){
		double radians = rotationDegrees * (Math.PI / 180.0);

		float cos = (float)Math.Cos(radians);
		float sin = (float)Math.Sin(radians);

		float newX = vector.X * cos - vector.Y * sin;
		float newY = vector.X * sin + vector.Y * cos;

		return new PointF(newX, newY);
	}
}
