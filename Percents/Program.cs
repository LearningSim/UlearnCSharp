using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percents {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine(Calculate(Console.ReadLine()));
			Console.ReadLine();
		}

		public static double Calculate(string userInput) {
			var data = userInput.Split(' ').Select(double.Parse).ToArray();
			return GetProfit(data[0], data[1], data[2]);
		}

		private static double GetProfit(double money, double rate, double depositTerm) {
			return money * Math.Pow(1 + rate / 12 / 100, depositTerm);
		}
	}
}