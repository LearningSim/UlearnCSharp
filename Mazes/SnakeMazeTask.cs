using System;

namespace Mazes {
	public static class SnakeMazeTask {
		public static void MoveOut(Robot robot, int width, int height) {
			int y = 0;
			while (true) {
				MoveHorizontal(robot, width, Direction.Right);
				if (!SafeMoveDown(robot, height, y)) break;
				y += 2;

				MoveHorizontal(robot, width, Direction.Left);
				if (!SafeMoveDown(robot, height, y)) break;
				y += 2;
			}
		}

		private static bool SafeMoveDown(Robot robot, int height, int y) {
			if (y > height - 4) {
				return false;
			}

			robot.MoveTo(Direction.Down);
			robot.MoveTo(Direction.Down);
			return true;
		}

		private static void MoveHorizontal(Robot robot, int width, Direction dir) {
			for (int i = 0; i < width - 3; i++) {
				robot.MoveTo(dir);
			}
		}
	}
}