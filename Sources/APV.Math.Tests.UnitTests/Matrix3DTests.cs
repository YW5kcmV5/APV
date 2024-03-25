using APV.Math.Primitive3D.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Math.Tests.UnitTests
{
    [TestClass]
    public sealed class Matrix3DTests
    {
        [TestMethod]
        public void MulMatrixTest()
        {
            var x = new Matrix3D();
            x.I11 = 01; x.I12 = 02; x.I13 = 03; x.I14 = 04;
            x.I21 = 05; x.I22 = 06; x.I23 = 07; x.I24 = 08;
            x.I31 = 09; x.I32 = 10; x.I33 = 11; x.I34 = 12;
            x.I41 = 13; x.I42 = 14; x.I43 = 15; x.I44 = 16;

            var y = new Matrix3D();
            y.I11 = 16; y.I12 = 15; y.I13 = 14; y.I14 = 13;
            y.I21 = 12; y.I22 = 11; y.I23 = 10; y.I24 = 09;
            y.I31 = 08; y.I32 = 07; y.I33 = 06; y.I34 = 05;
            y.I41 = 04; y.I42 = 03; y.I43 = 02; y.I44 = 01;

            var z = new Matrix3D();
            z.I11 = 080; z.I12 = 070; z.I13 = 060; z.I14 = 050;
            z.I21 = 240; z.I22 = 214; z.I23 = 188; z.I24 = 162;
            z.I31 = 400; z.I32 = 358; z.I33 = 316; z.I34 = 274;
            z.I41 = 560; z.I42 = 502; z.I43 = 444; z.I44 = 386;

            var r = Matrix3D.Mul(x, y);

            Assert.AreEqual(z.I11, r.I11);
            Assert.AreEqual(z.I12, r.I12);
            Assert.AreEqual(z.I13, r.I13);
            Assert.AreEqual(z.I14, r.I14);
            Assert.AreEqual(z.I21, r.I21);
            Assert.AreEqual(z.I22, r.I22);
            Assert.AreEqual(z.I23, r.I23);
            Assert.AreEqual(z.I24, r.I24);
            Assert.AreEqual(z.I31, r.I31);
            Assert.AreEqual(z.I32, r.I32);
            Assert.AreEqual(z.I33, r.I33);
            Assert.AreEqual(z.I34, r.I34);
            Assert.AreEqual(z.I41, r.I41);
            Assert.AreEqual(z.I42, r.I42);
            Assert.AreEqual(z.I43, r.I43);
            Assert.AreEqual(z.I44, r.I44);
        }

        [TestMethod]
        public void DetTest()
        {
            var x = new Matrix3D();
            x.I11 = 01; x.I12 = 02; x.I13 = 03;
            x.I21 = 05; x.I22 = 06; x.I23 = 07;
            x.I31 = 09; x.I32 = 10; x.I33 = 11;
            float det = Matrix3D.GetDet(x);

            Assert.AreEqual(0, det);

            x.I11 = 02; x.I12 = 03; x.I13 = 04;
            x.I21 = 05; x.I22 = 06; x.I23 = 07;
            x.I31 = 08; x.I32 = 09; x.I33 = 11;
            det = Matrix3D.GetDet(x);

            Assert.AreEqual(-3, det);
        }

        [TestMethod]
        public void InvertibleTest()
        {
            var x = new Matrix3D();
            x.I11 = 01; x.I12 = 02; x.I13 = 03;
            x.I21 = 05; x.I22 = 06; x.I23 = 07;
            x.I31 = 09; x.I32 = 10; x.I33 = 12;

            var z = new Matrix3D();
            z.I11 = -0.50f; z.I12 = -1.50f; z.I13 = +1.00f;
            z.I21 = -0.75f; z.I22 = +3.75f; z.I23 = -2.00f;
            z.I31 = +1.00f; z.I32 = -2.00f; z.I33 = +1.00f;

            bool exists;
            Matrix3D r = Matrix3D.GetInvertible(x, out exists);

            Assert.AreEqual(z.I11, r.I11);
            Assert.AreEqual(z.I12, r.I12);
            Assert.AreEqual(z.I13, r.I13);
            Assert.AreEqual(z.I21, r.I21);
            Assert.AreEqual(z.I22, r.I22);
            Assert.AreEqual(z.I23, r.I23);
            Assert.AreEqual(z.I31, r.I31);
            Assert.AreEqual(z.I32, r.I32);
            Assert.AreEqual(z.I33, r.I33);
        }
    }
}