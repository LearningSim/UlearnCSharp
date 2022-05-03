using System;
using System.Linq;
using NUnit.Framework;

namespace Manipulation
{
    public static class ManipulatorTask
    {
        /// <summary>
        /// Возвращает массив углов (shoulder, elbow, wrist),
        /// необходимых для приведения эффектора манипулятора в точку x и y 
        /// с углом между последним суставом и горизонталью, равному alpha (в радианах)
        /// См. чертеж manipulator.png!
        /// </summary>
        public static double[] MoveManipulatorTo(double x, double y, double alpha)
        {
            var wristPos = new Vec(Manipulator.Palm, 0).Rotate(Math.PI - alpha) + new Vec(x, y);
            var elbow =
                TriangleTask.GetABAngle(Manipulator.UpperArm, Manipulator.Forearm, wristPos.Magnitude());
            var shoulder =
                TriangleTask.GetABAngle(wristPos.Magnitude(), Manipulator.UpperArm, Manipulator.Forearm) +
                Math.Atan2(wristPos.Y, wristPos.X);
            var wrist = -alpha - shoulder - elbow;

            var angles = new[] { shoulder, elbow, wrist };
            if (angles.Any(double.IsNaN))
            {
                return new[] { double.NaN, double.NaN, double.NaN };
            }

            return angles;
        }
    }

    class Vec
    {
        public double X { get; }
        public double Y { get; }

        public Vec(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vec Rotate(double angle)
        {
            var x = X * Math.Cos(angle) - Y * Math.Sin(angle);
            var y = X * Math.Sin(angle) + Y * Math.Cos(angle);
            return new Vec(x, y);
        }

        public static Vec operator -(Vec a, Vec b) => new Vec(a.X - b.X, a.Y - b.Y);
        public static Vec operator +(Vec a, Vec b) => new Vec(a.X + b.X, a.Y + b.Y);

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y);
        }
    }

    [TestFixture]
    public class ManipulatorTask_Tests
    {
        private static readonly Random _random = new Random();

        [TestCase(Manipulator.Forearm + Manipulator.Palm, Manipulator.UpperArm, 0)]
        [TestCase(Manipulator.Forearm, Manipulator.UpperArm - Manipulator.Palm, -Math.PI / 2)]
        [TestCase(0, -Manipulator.Forearm + Manipulator.Palm + Manipulator.UpperArm, -Math.PI / 2)]
        [TestCase(-Manipulator.Forearm + Manipulator.Palm + Manipulator.UpperArm, 0, -Math.PI)]
        public void TestMoveManipulatorTo(double x, double y, double angle)
        {
            var rads = ManipulatorTask.MoveManipulatorTo(x, y, angle);
            var angles = rads.Select(r => r * 180 / Math.PI).ToArray();
            var joints = AnglesToCoordinatesTask.GetJointPositions(rads[0], rads[1], rads[2]);
            Assert.AreEqual(x, joints[2].X, 1e-5, $"palm endX ({x}, {y}, {angle})");
            Assert.AreEqual(y, joints[2].Y, 1e-5, $"palm endY ({x}, {y}, {angle})");
        }

        [Test]
        public void TestMoveManipulatorToRandomized()
        {
            for (int i = 0; i < 30; i++)
            {
                var x = _random.NextDouble() * 400;
                var y = _random.NextDouble() * 400;
                var alpha = _random.NextDouble() * 2 * Math.PI;
                var angles = ManipulatorTask.MoveManipulatorTo(x, y, alpha);

                var rmin = Math.Abs(Manipulator.UpperArm - Manipulator.Forearm);
                var rmax = Manipulator.UpperArm + Manipulator.Forearm;
                var dist = new Vec(x, y).Magnitude();
                if (dist < rmin || dist > rmax)
                {
                    Assert.AreEqual(angles[0], double.NaN, 1e-5);
                    return;
                }

                var joints = AnglesToCoordinatesTask.GetJointPositions(angles[0], angles[1], angles[2]);
                Assert.AreEqual(x, joints[2].X, 1e-4, $"palm endX {i}({x}, {y}, {alpha})");
                Assert.AreEqual(y, joints[2].Y, 1e-4, $"palm endY {i}({x}, {y}, {alpha})");
            }
        }
    }
}