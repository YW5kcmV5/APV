using System;
using System.Drawing;
using System.Linq;
using System.Xml;
using APV.Common;
using APV.GraphicsLibrary;
using APV.GraphicsLibrary.Images;
using APV.Pottle.UnitTest.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.System
{
    [TestClass]
    public class ImageContainerTests
    {
        [TestMethod]
        public void IconConstructor()
        {
            Image icon = ResourceManager.TestIcon;
            Assert.IsTrue(icon.RawFormat.Convert() == ImageFormat.Icon);

            var container = new ImageContainer(icon);

            Assert.AreEqual(ImageFormat.Icon, container.ImageFormat);
            Assert.AreEqual(icon.Width, container.Width);
            Assert.AreEqual(icon.Height, container.Height);
            Assert.AreEqual(icon.HorizontalResolution, container.HorizontalResolution);
            Assert.AreEqual(icon.VerticalResolution, container.VerticalResolution);
            Assert.AreEqual(icon.PixelFormat, container.PixelFormat);

            byte[] restoredIconBitmapData = container.ToBitmapArray();
            Assert.IsNotNull(restoredIconBitmapData);
            Assert.IsFalse(restoredIconBitmapData.All(b => b == 0));

            byte[] restoredIconData = container.ToByteArray();
            Assert.IsNotNull(restoredIconData);
            Assert.IsFalse(restoredIconData.All(b => b == 0));

            Image restoredIcon = container.ToBitmap();
            Assert.IsNotNull(restoredIcon);

            byte[] containerHash = container.GetHashCode();
            byte[] iconHash = ImageContainer.CalcHash(icon);
            byte[] restoredIconHash = ImageContainer.CalcHash(restoredIcon);

            bool equals = Comparator.Equals(containerHash, iconHash);
            Assert.IsTrue(equals);

            equals = Comparator.Equals(containerHash, restoredIconHash);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void LogoConstructor()
        {
            Image logo = ResourceManager.TestLogo;

            Assert.IsTrue(logo.RawFormat.Convert() == ImageFormat.Png);

            var container = new ImageContainer(logo);

            Assert.AreEqual(ImageFormat.Png, container.ImageFormat);
            Assert.AreEqual(logo.Width, container.Width);
            Assert.AreEqual(logo.Height, container.Height);
            Assert.AreEqual(logo.HorizontalResolution, container.HorizontalResolution);
            Assert.AreEqual(logo.VerticalResolution, container.VerticalResolution);
            Assert.AreEqual(logo.PixelFormat, container.PixelFormat);

            Image restoredLogo = container.ToBitmap();
            Assert.IsNotNull(restoredLogo);

            byte[] containerHash = container.GetHashCode();
            byte[] logoHash = ImageContainer.CalcHash(logo);
            byte[] restoredLogoHash = ImageContainer.CalcHash(restoredLogo);

            bool equals = Comparator.Equals(containerHash, logoHash);
            Assert.IsTrue(equals);

            equals = Comparator.Equals(containerHash, restoredLogoHash);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void IncreaseIcon()
        {
            Image icon = ResourceManager.TestIcon;
            Assert.IsTrue(icon.RawFormat.Convert() == ImageFormat.Icon);

            int newSize = 2*icon.Width;
            Image resizedIcon = icon.Resize(newSize);

            Assert.IsNotNull(resizedIcon);
            Assert.AreEqual(newSize, resizedIcon.Width);
            Assert.AreEqual(newSize, resizedIcon.Height);
            Assert.IsTrue(resizedIcon.RawFormat.Convert() == ImageFormat.Icon);
        }

        [TestMethod]
        public void DescreaseIcon()
        {
            Image icon = ResourceManager.TestIcon;
            Assert.IsTrue(icon.RawFormat.Convert() == ImageFormat.Icon);

            int newSize = icon.Width/2;
            Image resizedIcon = icon.Resize(newSize);

            Assert.IsNotNull(resizedIcon);
            Assert.AreEqual(newSize, resizedIcon.Width);
            Assert.AreEqual(newSize, resizedIcon.Height);
            Assert.IsTrue(resizedIcon.RawFormat.Convert() == ImageFormat.Icon);
        }

        [TestMethod]
        public void IncreaseIconHeight()
        {
            Image icon = ResourceManager.TestIcon;
            Assert.IsTrue(icon.RawFormat.Convert() == ImageFormat.Icon);

            int newSize = 2*icon.Height;
            Image resizedIcon = icon.ResizeHeight(newSize);

            Assert.IsNotNull(resizedIcon);
            Assert.AreEqual(newSize, resizedIcon.Width);
            Assert.AreEqual(newSize, resizedIcon.Height);
            Assert.IsTrue(resizedIcon.RawFormat.Convert() == ImageFormat.Icon);
        }

        [TestMethod]
        public void DescreaseIconHeight()
        {
            Image icon = ResourceManager.TestIcon;
            Assert.IsTrue(icon.RawFormat.Convert() == ImageFormat.Icon);

            int newSize = icon.Width / 2;
            Image resizedIcon = icon.ResizeHeight(newSize);

            Assert.IsNotNull(resizedIcon);
            Assert.AreEqual(newSize, resizedIcon.Width);
            Assert.AreEqual(newSize, resizedIcon.Height);
            Assert.IsTrue(resizedIcon.RawFormat.Convert() == ImageFormat.Icon);
        }

        [TestMethod]
        public void DataContractSerializationText()
        {
            Image logo = ResourceManager.TestLogo;
            var container = new ImageContainer(logo);

            XmlDocument doc = Serializer.Serialize(container, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(doc);

            var restoredContainer = Serializer.Deserialize<ImageContainer>(doc, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(restoredContainer);
            Assert.AreEqual(container.Width, restoredContainer.Width);
            Assert.AreEqual(container.Height, restoredContainer.Height);
            Assert.AreEqual(container.PixelFormat, restoredContainer.PixelFormat);
            Assert.AreEqual(container.ImageFormat, restoredContainer.ImageFormat);
            Assert.AreEqual(container.FileFormat, restoredContainer.FileFormat);
            //Assert.AreEqual(container.HorizontalResolution, restoredContainer.HorizontalResolution);
            //Assert.AreEqual(container.VerticalResolution, restoredContainer.VerticalResolution);
            Assert.AreEqual(Convert.ToBase64String(container.GetHashCode()), Convert.ToBase64String(restoredContainer.GetHashCode()));
        }

        [TestMethod]
        public void XmlSerializationText()
        {
            Image logo = ResourceManager.TestLogo;
            var container = new ImageContainer(logo);

            XmlDocument doc = Serializer.Serialize(container);

            Assert.IsNotNull(doc);

            var restoredContainer = Serializer.Deserialize<ImageContainer>(doc);

            Assert.IsNotNull(restoredContainer);
            Assert.AreEqual(container.Width, restoredContainer.Width);
            Assert.AreEqual(container.Height, restoredContainer.Height);
            Assert.AreEqual(container.PixelFormat, restoredContainer.PixelFormat);
            Assert.AreEqual(container.ImageFormat, restoredContainer.ImageFormat);
            Assert.AreEqual(container.FileFormat, restoredContainer.FileFormat);
            //Assert.AreEqual(container.HorizontalResolution, restoredContainer.HorizontalResolution);
            //Assert.AreEqual(container.VerticalResolution, restoredContainer.VerticalResolution);
            Assert.AreEqual(Convert.ToBase64String(container.GetHashCode()), Convert.ToBase64String(restoredContainer.GetHashCode()));
        }
    }
}