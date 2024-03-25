using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using APV.Pottle.UnitTest.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;
using APV.GraphicsLibrary;
using APV.GraphicsLibrary.Images;

namespace APV.Pottle.UnitTest.System
{
    [TestClass]
    public class IconContainerTest
    {
        [TestMethod]
        public void ParseArrayTest()
        {
            const string stringData = ResourceManager.TestIconData;
            byte[] data = Convert.FromBase64String(stringData);

            var iconContainer = new IconContainer(data);
            Bitmap[] bitmaps = iconContainer.GetBitmaps();

            Assert.IsNotNull(bitmaps);
            Assert.AreEqual(2, bitmaps.Length);
            Assert.IsNotNull(bitmaps[0]);
            Assert.IsNotNull(bitmaps[1]);

            Assert.AreEqual(16, bitmaps[0].Width);
            Assert.AreEqual(16, bitmaps[0].Height);
            Assert.AreEqual(32, bitmaps[1].Width);
            Assert.AreEqual(32, bitmaps[1].Height);
        }

        [TestMethod]
        public void ParseStreamTest()
        {
            const string stringData = ResourceManager.TestIconData;
            byte[] data = Convert.FromBase64String(stringData);

            IconContainer iconContainer;
            using (var stream = new MemoryStream(data))
            {
                iconContainer = new IconContainer(stream);
            }

            Bitmap[] bitmaps = iconContainer.GetBitmaps();

            Assert.IsNotNull(bitmaps);
            Assert.AreEqual(2, bitmaps.Length);
            Assert.IsNotNull(bitmaps[0]);
            Assert.IsNotNull(bitmaps[1]);

            Assert.AreEqual(16, bitmaps[0].Width);
            Assert.AreEqual(16, bitmaps[0].Height);
            Assert.AreEqual(32, bitmaps[1].Width);
            Assert.AreEqual(32, bitmaps[1].Height);
        }

        [TestMethod]
        public void SaveTest()
        {
            const string stringData = ResourceManager.TestIconData;
            byte[] data = Convert.FromBase64String(stringData);

            var iconContainer = new IconContainer(data);

            Bitmap[] bitmaps = iconContainer.GetBitmaps();

            var copy = new IconContainer();
            foreach (Bitmap bitmap in bitmaps)
            {
                copy.Add(bitmap);
            }

            byte[] copyData;
            using (var stream = new MemoryStream())
            {
                copy.Save(stream);
                copyData = stream.ToArray();
            }

            iconContainer = new IconContainer(copyData);
            bitmaps = iconContainer.GetBitmaps();

            Assert.IsNotNull(bitmaps);
            Assert.AreEqual(2, bitmaps.Length);
            Assert.IsNotNull(bitmaps[0]);
            Assert.IsNotNull(bitmaps[1]);

            Assert.AreEqual(16, bitmaps[0].Width);
            Assert.AreEqual(16, bitmaps[0].Height);
            Assert.AreEqual(32, bitmaps[1].Width);
            Assert.AreEqual(32, bitmaps[1].Height);
        }

        [TestMethod]
        public void RestoreIconTest()
        {
            var icon = (Bitmap)ResourceManager.TestIcon;
            Bitmap restoredIcon = IconContainer.ToIconFormat(icon);

            Assert.IsNotNull(restoredIcon);

            byte[] iconHashCode = IconContainer.GetHashCode(icon);
            byte[] restoredIconHashCode = IconContainer.GetHashCode(restoredIcon);
            bool equals = Comparator.Equals(iconHashCode, restoredIconHashCode);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void IconSetPixelFormatNewTest()
        {
            var icon = (Bitmap)ResourceManager.TestIcon;

            Bitmap copy = icon.SetPixelFormat(icon.PixelFormat);

            Assert.IsNotNull(copy);

            byte[] iconHashCode = IconContainer.GetHashCode(icon);
            byte[] copyHashCode = IconContainer.GetHashCode(copy);
            bool equals = Comparator.Equals(iconHashCode, copyHashCode);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void IconSetPixelFormatTest()
        {
            var icon = (Bitmap)ResourceManager.TestIcon;

            Bitmap copy = icon.SetPixelFormat(PixelFormat.Format32bppRgb);

            Assert.IsNotNull(copy);

            byte[] iconHashCode = IconContainer.GetHashCode(icon);
            byte[] copyHashCode = IconContainer.GetHashCode(copy);
            bool equals = Comparator.Equals(iconHashCode, copyHashCode);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void ExtractIconTest()
        {
            const string stringData = ResourceManager.TestIconData;
            byte[] data = Convert.FromBase64String(stringData);

            Bitmap[] bitmaps = IconContainer.Extract(data);

            Assert.IsNotNull(bitmaps);
            Assert.AreEqual(2, bitmaps.Length);
            Assert.IsNotNull(bitmaps[0]);
            Assert.IsNotNull(bitmaps[1]);

            Assert.AreEqual(16, bitmaps[0].Width);
            Assert.AreEqual(16, bitmaps[0].Height);
            Assert.AreEqual(32, bitmaps[1].Width);
            Assert.AreEqual(32, bitmaps[1].Height);
        }

        [TestMethod]
        public void ExtractSingleIconTest()
        {
            var icon = (Bitmap)ResourceManager.TestIcon;
            byte[] data = icon.ToByteArray();

            Bitmap[] bitmaps = IconContainer.Extract(data);

            Assert.IsNotNull(bitmaps);
            Assert.AreEqual(1, bitmaps.Length);
            Assert.IsNotNull(bitmaps[0]);

            Assert.AreEqual(icon.Width, bitmaps[0].Width);
            Assert.AreEqual(icon.Height, bitmaps[0].Height);
        }

        [TestMethod]
        public void ExtractBitmapTest()
        {
            var logo = (Bitmap)ResourceManager.TestLogo;
            byte[] data = logo.ToByteArray();

            Bitmap[] bitmaps = IconContainer.Extract(data);

            Assert.IsNotNull(bitmaps);
            Assert.AreEqual(1, bitmaps.Length);
            Assert.IsNotNull(bitmaps[0]);

            Assert.AreEqual(logo.Width, bitmaps[0].Width);
            Assert.AreEqual(logo.Height, bitmaps[0].Height);
        }
    }
}