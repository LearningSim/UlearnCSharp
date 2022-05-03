using System;
using System.Drawing;
using System.Windows.Forms;

namespace Manipulation
{
	public static class VisualizerTask
	{
		public static double X = 220;
		public static double Y = -100;
		public static double Alpha = 0.05;
		public static double Wrist = 2 * Math.PI / 3;
		public static double Elbow = 3 * Math.PI / 4;
		public static double Shoulder = Math.PI / 2;

		private static readonly double step = Math.PI / 180;
		private static readonly double alphaStep = Math.PI / 180 / 120;

		public static Brush UnreachableAreaBrush = new SolidBrush(Color.FromArgb(255, 255, 230, 230));
		public static Brush ReachableAreaBrush = new SolidBrush(Color.FromArgb(255, 230, 255, 230));
		public static Pen ManipulatorPen = new Pen(Color.Black, 3);
		public static Brush JointBrush = Brushes.Gray;

		public static void KeyDown(Form form, KeyEventArgs key)
		{
			switch (key.KeyCode)
			{
				case Keys.Q:
					Shoulder += step;
					break;
				case Keys.A:
					Shoulder -= step;
					break;
				case Keys.W:
					Elbow += step;
					break;
				case Keys.S:
					Elbow -= step;
					break;
			}

			Wrist = -Alpha - Shoulder - Elbow;
			form.Invalidate();
		}

		public static void MouseMove(Form form, MouseEventArgs e)
		{
			var mouse = ConvertWindowToMath(new PointF(e.X, e.Y), GetShoulderPos(form));
			X = mouse.X;
			Y = mouse.Y;

			UpdateManipulator();
			form.Invalidate();
		}

		public static void MouseWheel(Form form, MouseEventArgs e)
		{
			Alpha += e.Delta * alphaStep;

			UpdateManipulator();
			form.Invalidate();
		}

		public static void UpdateManipulator()
		{
			var angles = ManipulatorTask.MoveManipulatorTo(X, Y, Alpha);
			if (double.IsNaN(angles[0])) return;
			Shoulder = angles[0];
			Elbow = angles[1];
			Wrist = angles[2];
		}

		public static void DrawManipulator(Graphics graphics, PointF shoulderPos)
		{
			var joints = AnglesToCoordinatesTask.GetJointPositions(Shoulder, Elbow, Wrist);

			graphics.DrawString($"X={X:0}, Y={Y:0}, Alpha={Alpha:0.00}", 
                new Font(SystemFonts.DefaultFont.FontFamily, 12), 
                Brushes.DarkRed, 
                10, 
                10);
			DrawReachableZone(graphics, ReachableAreaBrush, UnreachableAreaBrush, shoulderPos, joints);

			var elbowPos = ConvertMathToWindow(joints[0], shoulderPos);
			var wristPos = ConvertMathToWindow(joints[1], shoulderPos);
			var palmEndPos = ConvertMathToWindow(joints[2], shoulderPos);
			graphics.DrawLine(ManipulatorPen, shoulderPos.X, shoulderPos.Y, elbowPos.X, elbowPos.Y);
			graphics.DrawLine(ManipulatorPen, elbowPos.X, elbowPos.Y, wristPos.X, wristPos.Y);
			graphics.DrawLine(ManipulatorPen, wristPos.X, wristPos.Y, palmEndPos.X, palmEndPos.Y);

			const float joint = 10f;
			graphics.FillEllipse(JointBrush, shoulderPos.X - joint / 2, shoulderPos.Y - joint / 2, joint, joint);
			graphics.FillEllipse(JointBrush, elbowPos.X - joint / 2, elbowPos.Y - joint / 2, joint, joint);
			graphics.FillEllipse(JointBrush, wristPos.X - joint / 2, wristPos.Y - joint / 2, joint, joint);
		}

		private static void DrawReachableZone(
            Graphics graphics, 
            Brush reachableBrush, 
            Brush unreachableBrush, 
            PointF shoulderPos, 
            PointF[] joints)
		{
			var rmin = Math.Abs(Manipulator.UpperArm - Manipulator.Forearm);
			var rmax = Manipulator.UpperArm + Manipulator.Forearm;
			var mathCenter = new PointF(joints[2].X - joints[1].X, joints[2].Y - joints[1].Y);
			var windowCenter = ConvertMathToWindow(mathCenter, shoulderPos);
			graphics.FillEllipse(reachableBrush, windowCenter.X - rmax, windowCenter.Y - rmax, 2 * rmax, 2 * rmax);
			graphics.FillEllipse(unreachableBrush, windowCenter.X - rmin, windowCenter.Y - rmin, 2 * rmin, 2 * rmin);
		}

		public static PointF GetShoulderPos(Form form)
		{
			return new PointF(form.ClientSize.Width / 2f, form.ClientSize.Height / 2f);
		}

		public static PointF ConvertMathToWindow(PointF mathPoint, PointF shoulderPos)
		{
			return new PointF(mathPoint.X + shoulderPos.X, shoulderPos.Y - mathPoint.Y);
		}

		public static PointF ConvertWindowToMath(PointF windowPoint, PointF shoulderPos)
		{
			return new PointF(windowPoint.X - shoulderPos.X, shoulderPos.Y - windowPoint.Y);
		}
	}
}