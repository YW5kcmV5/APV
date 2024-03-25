using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APV.Math.Primitive3D.Entities
{
    public struct Camera3D
    {
        public Vector3D P;
        public Vector3D Q;
        public Vector3D R;
        public Vector3D S;

        public static Camera3D Standard(float xSize, float ySize, float dist)
        {
            var result = new Camera3D();
            result.P.I3 = dist;
            result.Q.I1 = xSize / 2;
            result.R.I2 = ySize / 2;
            result.S.I3 = -dist;
            return result;
        }
    }
}
