using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;

namespace APV.Pottle.UnitTest.System
{
    [TestClass]
    public class ComparatorTests
    {
        private enum ByteEnum : byte
        {
            X,

            Y
        }

        private enum IntEnum
        {
            X,

            Y
        }

        private enum LongEnum : long
        {
            X,

            Y
        }

        [TestMethod]
        public void TwoDimensionalEqualsArrayTest()
        {
            var x = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var y = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };

            bool equals = Comparator.Equals(x, y);
            Assert.IsTrue(equals);

            equals = Comparator.Equals((object)x, y);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void TwoDimensionalNotEqualsArrayTest()
        {
            var x = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var y = new[,] { { 1, 2 }, { 3, 5 }, { 5, 6 } };

            bool equals = Comparator.Equals(x, y);
            Assert.IsFalse(equals);

            equals = Comparator.Equals((object)x, y);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void ThreeDimensionalEqualsArrayTest()
        {
            var x = new[, ,] { { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } }, { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } }, { { 17, 18, 19, 20 }, { 21, 22, 23, 24 } } };
            var y = new[, ,] { { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } }, { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } }, { { 17, 18, 19, 20 }, { 21, 22, 23, 24 } } };

            bool equals = Comparator.Equals(x, y);
            Assert.IsTrue(equals);

            equals = Comparator.Equals((object)x, y);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void ThreeDimensionalNotEqualsArrayTest()
        {
            var x = new[, ,] { { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } }, { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } }, { { 17, 18, 19, 20 }, { 21, 22, 23, 24 } } };
            var y = new[, ,] { { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } }, { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } }, { { -1, 18, 19, 20 }, { 21, 22, 23, 24 } } };

            bool equals = Comparator.Equals(x, y);
            Assert.IsFalse(equals);

            equals = Comparator.Equals((object)x, y);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotEqualsDimensionalArrayTest()
        {
            var x = new[, ,] { { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } }, { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } }, { { 17, 18, 19, 20 }, { 21, 22, 23, 24 } } };
            var y = new[,] { { 1, 2 }, { 3, 5 }, { 5, 6 } };

            bool equals = Comparator.Equals(x, y);
            Assert.IsFalse(equals);

            equals = Comparator.Equals((object)x, y);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotEqualsLengthArrayTest()
        {
            var x = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var y = new[,] { { 1, 2, 3 }, { 4, 5, 6} };

            bool equals = Comparator.Equals(x, y);
            Assert.IsFalse(equals);

            equals = Comparator.Equals((object)x, y);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void DateTimeTest()
        {
            DateTime @now = DateTime.UtcNow;
            DateTime x = @now;
            DateTime y = @now;

            bool equals = Comparator.Equals(x, y);
            Assert.IsTrue(equals);

            equals = Comparator.Equals((object)x, y);
            Assert.IsTrue(equals);

            x = @now;
            y = @now.ToLocalTime();

            equals = Comparator.Equals(x, y);
            Assert.IsTrue(equals);

            equals = Comparator.Equals((object)x, y);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void ByteEnumTest()
        {
            var x = ByteEnum.X;
            var y = ByteEnum.X;

            bool equals = Comparator.Equals(x, y);
            Assert.IsTrue(equals);

            x = ByteEnum.X;
            y = ByteEnum.Y;

            equals = Comparator.Equals(x, y);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void IntEnumTest()
        {
            var x = IntEnum.X;
            var y = IntEnum.X;

            bool equals = Comparator.Equals(x, y);
            Assert.IsTrue(equals);

            x = IntEnum.X;
            y = IntEnum.Y;

            equals = Comparator.Equals(x, y);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void LongEnumTest()
        {
            var x = LongEnum.X;
            var y = LongEnum.X;

            bool equals = Comparator.Equals(x, y);
            Assert.IsTrue(equals);

            x = LongEnum.X;
            y = LongEnum.Y;

            equals = Comparator.Equals(x, y);
            Assert.IsFalse(equals);
        }
    }
}