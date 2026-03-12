public static class Tools{
	public static bool IsLineIntersectingRect(PointF p1, PointF p2, RectangleF r){
		if (r.Contains(p1) || r.Contains(p2)) return true;

		return LineIntersectsLine(p1, p2, new PointF(r.Left, r.Top), new PointF(r.Right, r.Top)) ||
			   LineIntersectsLine(p1, p2, new PointF(r.Left, r.Bottom), new PointF(r.Right, r.Bottom)) ||
			   LineIntersectsLine(p1, p2, new PointF(r.Left, r.Top), new PointF(r.Left, r.Bottom)) ||
			   LineIntersectsLine(p1, p2, new PointF(r.Right, r.Top), new PointF(r.Right, r.Bottom));
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
}
