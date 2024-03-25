using System.Drawing;

namespace APV.Math.Diagrams
{
	public interface IGraphDevice
	{
		int Width { get; }

		int Height { get; }

	    object Container { get; }

		void DrawLine(int x0, int y0, int x1, int y1, Pen pen);

		void DrawCircle(int x0, int y0, int d, Pen pen);

		void DrawReactangle(int x1, int x2, int width, int height, Pen pen);

		void DrawReversibleRectangle(int x0, int y0, int x1, int y1);

		void DrawReversibleCross(int x, int y);

		void Clear(Color color);

		void StartDrawing();

		void StopDrawing();

		void Resize(int width, int height);
	}
}