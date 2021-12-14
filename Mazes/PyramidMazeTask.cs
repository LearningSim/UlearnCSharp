namespace Mazes {
	public static class PyramidMazeTask {
		public static void MoveOut(Robot robot, int width, int height) {
			int y = 0;
			int w = width - 3;
			while (true) {
				MoveHorizontal(robot, w, Direction.Right);
				w -= 2;
				if (!SafeMoveUp(robot, height, y)) break;
				y += 2;

				MoveHorizontal(robot, w, Direction.Left);
				w -= 2;
				if (!SafeMoveUp(robot, height, y)) break;
				y += 2;
			}
		}

		private static bool SafeMoveUp(Robot robot, int height, int y) {
			if (y > height - 4) {
				return false;
			}

			robot.MoveTo(Direction.Up);
			robot.MoveTo(Direction.Up);
			return true;
		}

		private static void MoveHorizontal(Robot robot, int width, Direction dir) {
			for (int i = 0; i < width; i++) {
				robot.MoveTo(dir);
			}
		}
	}
}