using System;

namespace Billiards
{
	public static class BilliardsTask
	{
		public static double BounceWall(double directionRadians, double wallInclinationRadians) {
			var angle =  wallInclinationRadians - (directionRadians - wallInclinationRadians);
			return angle;
		}
	}
}