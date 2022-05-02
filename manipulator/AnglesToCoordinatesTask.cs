using System;
using System.Drawing;
using NUnit.Framework;

namespace Manipulation
{
    public static class AnglesToCoordinatesTask
    {
        /// <summary>
        /// По значению углов суставов возвращает массив координат суставов
        /// в порядке new []{elbow, wrist, palmEnd}
        /// </summary>
        public static PointF[] GetJointPositions(double shoulder, double elbow, double wrist)
        {
            var elbowPos = new PointF(Manipulator.UpperArm, 0).Rotate(shoulder);

            var elbowAngle = shoulder + elbow - Math.PI;
            var wristPos = new PointF(Manipulator.Forearm, 0).Rotate(elbowAngle);

            var wristAngle = wrist + elbowAngle - Math.PI;
            var palmEndPos = new PointF(Manipulator.Palm, 0).Rotate(wristAngle);
            
            return new[]
            {
                elbowPos,
                elbowPos + new SizeF(wristPos),
                elbowPos + new SizeF(wristPos) + new SizeF(palmEndPos)
            };
        }

        private static PointF Rotate(this PointF point, double angle)
        {
            var x = point.X * Math.Cos(angle) - point.Y * Math.Sin(angle);
            var y = point.X * Math.Sin(angle) + point.Y * Math.Cos(angle);
            return new PointF((float)x, (float)y);
        }
    }

    [TestFixture]
    public class AnglesToCoordinatesTask_Tests
    {
        // Доработайте эти тесты!
        // С помощью строчки TestCase можно добавлять новые тестовые данные.
        // Аргументы TestCase превратятся в аргументы метода.
        [TestCase(Math.PI / 2, Math.PI / 2, Math.PI, Manipulator.Forearm + Manipulator.Palm, Manipulator.UpperArm)]
        [TestCase(Math.PI / 2, Math.PI / 2, Math.PI / 2, Manipulator.Forearm, Manipulator.UpperArm - Manipulator.Palm)]
        [TestCase(Math.PI / 2, 0, 0, 0, -Manipulator.Forearm + Manipulator.Palm + Manipulator.UpperArm)]
        [TestCase(0, 0, 0, -Manipulator.Forearm + Manipulator.Palm + Manipulator.UpperArm, 0)]
        [TestCase(0, Math.PI, Math.PI, Manipulator.Forearm + Manipulator.Palm + Manipulator.UpperArm, 0)]
        public void TestGetJointPositions(double shoulder, double elbow, double wrist, double palmEndX, double palmEndY)
        {
            var joints = AnglesToCoordinatesTask.GetJointPositions(shoulder, elbow, wrist);
            Assert.AreEqual(palmEndX, joints[2].X, 1e-5, "palm endX");
            Assert.AreEqual(palmEndY, joints[2].Y, 1e-5, "palm endY");

            // проверить, что расстояния между суставами равны длинам сегментов манипулятора
            var upperArm = Magnitude(joints[0] - SizeF.Empty);
            Assert.AreEqual(Manipulator.UpperArm, upperArm, 1e-5, "upperArm");

            var forearm = Magnitude(joints[1] - new SizeF(joints[0]));
            Assert.AreEqual(Manipulator.Forearm, forearm, 1e-5, "forearm");

            var palm = Magnitude(joints[2] - new SizeF(joints[1]));
            Assert.AreEqual(Manipulator.Palm, palm, 1e-5, "palm");
        }

        private double Magnitude(PointF vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }
    }
}