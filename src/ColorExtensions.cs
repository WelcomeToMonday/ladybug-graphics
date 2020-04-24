using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ladybug.Graphics
{
	public static class ColorExtensions
	{
		public static string GetHexString(this Color c)
		{
			return $"{c.R:X2}{c.G:X2}{c.B:X2}";
		}

		public static Color GetColorFromHex(string hex)
		{
			var value= hex.TrimStart('#');
			
			var hexr = Convert.ToInt32(value.Substring(0,2), 16);
			var hexg = Convert.ToInt32(value.Substring(2,2), 16);
			var hexb = Convert.ToInt32(value.Substring(4,2), 16);
			
			return new Color(hexr, hexg, hexb);
		}

		public static string GetHexFromColor(Color color)
		{
			return color.GetHexString();
		}
	}
}