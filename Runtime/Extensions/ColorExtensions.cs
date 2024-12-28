using UnityEngine;

namespace RDTools.Runtime
{
	public static class ColorExtensions
	{
		public static Color SetFullAlpha(this ref Color color)
		{
			color.a = 1f;

			return color;
		}

		public static Color SetNoAlpha(this ref Color color)
		{
			color.a = 0f;
			return color;
		}

		public static Color SetAlpha(this ref Color color, float alpha)
		{
			color.a = alpha;
			return color;
		}
	}
}
