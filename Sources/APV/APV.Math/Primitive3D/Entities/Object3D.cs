
using System;
using System.Drawing;

namespace APV.Math.Primitive3D.Entities
{
    public class Object3D
    {
        public IPrimitive3D[] Primitives;

        public Matrix3D ModelTransform;

        public void Apply(Matrix3D model)
        {
            IPrimitive3D[] primitives = Primitives;
            int length = primitives.Length;
            for (int j = 0; j < length; j++)
            {
                IPrimitive3D primitive3D = primitives[j];
                IPrimitive3D transformedPrimitive3D = primitive3D.Tranform(model);
                primitives[j] = transformedPrimitive3D;
            }
        }

        public void Draw(Bitmap3D bitmap3D, Matrix3D globalTransform, float dist)
        {
            IPrimitive3D[] primitives = Primitives;
            Matrix3D modelTransform = ModelTransform;

            int length = primitives.Length;
            Matrix3D transform = globalTransform * modelTransform;
            for (int j = 0; j < length; j++)
            {
                IPrimitive3D primitive3D = primitives[j];
                IPrimitive3D transformedObject = primitive3D.Tranform(transform);
                if (transformedObject.Visible(-dist))
                {
                    transformedObject.Draw(bitmap3D);
                    //if (transformedObject is Triangle3D)
                    //{
                    //    var triangle = ((Triangle3D)transformedObject);
                    //    Line3D normal = Triangle3D.GetNormalLine(triangle, Color.LightGray.ToArgb());
                    //    try
                    //    {
                    //        normal.Draw(bitmap3D);
                    //    }
                    //    catch (Exception)
                    //    {
                    //    }
                    //}
                }
            }
        }
    }
}