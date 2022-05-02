using System;
using NUnit.Framework;

namespace Manipulation
{
    public class TriangleTask
    {
        /// <summary>
        /// Возвращает угол (в радианах) между сторонами a и b в треугольнике со сторонами a, b, c 
        /// </summary>
        public static double GetABAngle(double a, double b, double c)
        {
            if (a + b >= c && b + c >= a && a + c >= b)
            {
                return Math.Acos((a * a + b * b - c * c) / (2 * a * b));
            }

            return double.NaN;
        }
    }

    [TestFixture]
    public class TriangleTask_Tests
    {
        [TestCase(3, 4, 5, Math.PI / 2)]
        [TestCase(1, 1, 1, Math.PI / 3)]
        [TestCase(1, 1, 2, Math.PI)]
        [TestCase(1, 0, 2, double.NaN)]
        [TestCase(1, 2, -2, double.NaN)]
        // добавьте ещё тестовых случаев!
        public void TestGetABAngle(double a, double b, double c, double expectedAngle)
        {
            Assert.AreEqual(TriangleTask.GetABAngle(a, b, c), expectedAngle, 1e-5);
        }
    }
}