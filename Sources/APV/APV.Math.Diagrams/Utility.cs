
namespace APV.Math.Diagrams
{
	public static class Utility
	{
		public static void Swap(ref int v1, ref int v2)
		{
			int v = v1;
			v1 = v2;
			v2 = v;
		}

		public static void Swap(ref float v1, ref float v2)
		{
			float v = v1;
			v1 = v2;
			v2 = v;
		}
	}
}
