using APV.Math.Primitive3D.Entities;

namespace APV.Math.Primitive3D
{
    public interface IPrimitive3D
    {
        IPrimitive3D Tranform(Matrix3D transform);

        void Draw(Bitmap3D bitmap);

        bool Visible(float dist);
    }
}
