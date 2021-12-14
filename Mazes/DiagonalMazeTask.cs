using System;

namespace Mazes {
	public static class DiagonalMazeTask {
		public static void MoveOut(Robot robot, int width, int height) {
			int steps = Math.Min(width - 3, height - 3);
			int stepX = (width - 3) / steps;
			int stepY = (height - 3) / steps;
			var right = width - 3 >= height - 3;
			for (int i = 0; i < 2 * steps + 1; i++) {
				if (right) {
					robot.Move(stepX, Direction.Right);
				}
				else {
					robot.Move(stepY, Direction.Down);
				}

				right = !right;
			}
		}

		private static void Move(this Robot robot, int dist, Direction dir) {
			for (int i = 0; i < dist; i++) {
				robot.MoveTo(dir);
			}
		}
	}
}