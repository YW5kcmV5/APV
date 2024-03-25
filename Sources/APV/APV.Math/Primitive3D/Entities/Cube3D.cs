
using System.Drawing;

namespace APV.Math.Primitive3D.Entities
{
    public class Cube3D : Object3D
    {
        public Cube3D(float width, int color)
        {
            float w = width/2.0f;
            Primitives = new IPrimitive3D[]
            {
                new Triangle3D(new Vector3D(-w, -w, w), new Vector3D(w, -w, w), new Vector3D(-w, w, w), color),
                new Triangle3D(new Vector3D(w, -w, w), new Vector3D(w, w, w), new Vector3D(-w, w, w), color),

                new Triangle3D(new Vector3D(-w, -w, -w), new Vector3D(-w, w, -w), new Vector3D(w, -w, -w), color),
                new Triangle3D(new Vector3D(w, -w, -w), new Vector3D(-w, w, -w), new Vector3D(w, w, -w), color),

                new Triangle3D(new Vector3D(w, -w, w), new Vector3D(w, -w, -w), new Vector3D(w, w, -w), color),
                new Triangle3D(new Vector3D(w, -w, w), new Vector3D(w, w, -w), new Vector3D(w, w, w), color),

                new Triangle3D(new Vector3D(-w, -w, w), new Vector3D(-w, w, -w), new Vector3D(-w, -w, -w), color),
                new Triangle3D(new Vector3D(-w, -w, w), new Vector3D(-w, w, w), new Vector3D(-w, w, -w), color),

                new Triangle3D(new Vector3D(-w, w, w), new Vector3D(w, w, -w), new Vector3D(-w, w, -w), color),
                new Triangle3D(new Vector3D(-w, w, w), new Vector3D(w, w, w), new Vector3D(w, w, -w), color),

                new Triangle3D(new Vector3D(-w, -w, w), new Vector3D(-w, -w, -w), new Vector3D(w, -w, -w), color),
                new Triangle3D(new Vector3D(-w, -w, w), new Vector3D(w, -w, -w), new Vector3D(w, -w, w), color),
            };
        }
    }
}