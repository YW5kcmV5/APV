using System;
using System.Drawing;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.BusinessLogic.Extensions;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using APV.Pottle.UnitTest.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;
using APV.GraphicsLibrary;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class ImageManagementTests : BaseManagementTests
    {
        [TestMethod]
        public void CreateLogoWithScale()
        {
            Image logo = ResourceManager.TestLogo;
            
            ImageManagement management = ImageManagement.Instance;

            DataImageEntity entity = management.CreateLogo(logo);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.ImageId != SystemConstants.UnknownId);
            Assert.AreEqual(Constants.LogoSize, entity.Size);
            Assert.IsNotNull(entity.OriginalImage);
            Assert.AreEqual(logo.Width, entity.OriginalImage.Width);
            Assert.AreEqual(logo.Height, entity.OriginalImage.Height);

            Image loadedOriginalImage = entity.OriginalImage.GetImage();
            bool equals = Comparator.Equals(logo, loadedOriginalImage);
            Assert.IsTrue(equals);

            Image scaledImage = entity.GetImage();

            DataImageEntity existingEntity = management.Create(scaledImage);
            Assert.IsNotNull(existingEntity);
            Assert.AreEqual(entity.ImageId, existingEntity.ImageId);

            equals = Comparator.Equals(logo, loadedOriginalImage);
            Assert.IsTrue(equals);

            DataImageEntity originalEntity = management.GetOriginalImage(scaledImage);
            Assert.IsNotNull(originalEntity);
            Assert.AreEqual(entity.OriginalImage.ImageId, originalEntity.ImageId);
        }

        [TestMethod]
        public void CreateLogoNoScale()
        {
            Image logo = ResourceManager.TestLogo;

            ImageManagement management = ImageManagement.Instance;

            DataImageEntity entity = management.Create(logo);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.ImageId != SystemConstants.UnknownId);
            Assert.IsNull(entity.OriginalImage);

            Image loadedImage = entity.GetImage();
            bool equals = Comparator.Equals(logo, loadedImage);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void CreateIconWithScale()
        {
            Image icon = ResourceManager.TestIcon;

            ImageManagement management = ImageManagement.Instance;

            DataImageEntity entity = management.CreateIcon(icon);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.ImageId != SystemConstants.UnknownId);
            Assert.AreEqual(Constants.IconSize, entity.Size);
            Assert.IsNotNull(entity.OriginalImage);
            Assert.AreEqual(icon.Width, entity.OriginalImage.Width);
            Assert.AreEqual(icon.Height, entity.OriginalImage.Height);

            Image loadedOriginalImage = entity.OriginalImage.GetImage();
            bool equals = Comparator.Equals(icon, loadedOriginalImage);
            Assert.IsTrue(equals);

            Image scaledImage = entity.GetImage();

            DataImageEntity existingEntity = management.Create(scaledImage);
            Assert.IsNotNull(existingEntity);
            Assert.AreEqual(entity.ImageId, existingEntity.ImageId);

            equals = Comparator.Equals(icon, loadedOriginalImage);
            Assert.IsTrue(equals);

            DataImageEntity originalEntity = management.GetOriginalImage(scaledImage);
            Assert.IsNotNull(originalEntity);
            Assert.AreEqual(entity.OriginalImage.ImageId, originalEntity.ImageId);
        }

        [TestMethod]
        public void CreateIconNoScale()
        {
            Image icon = ResourceManager.TestIcon;

            ImageManagement management = ImageManagement.Instance;

            DataImageEntity entity = management.Create(icon);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.ImageId != SystemConstants.UnknownId);
            Assert.IsNull(entity.OriginalImage);

            Image loadedImage = entity.GetImage();
            bool equals = Comparator.Equals(icon, loadedImage);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void DeleteTest()
        {
            Image logo = ResourceManager.TestLogo;

            ImageManagement management = ImageManagement.Instance;

            DataImageEntity entity = management.Create(logo);
            Assert.IsNotNull(entity);

            entity = management.Find(logo);
            Assert.IsNotNull(entity);

            management.Delete(logo);

            entity = management.Find(logo);
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void CreateSelfImageTest()
        {
            Image logo = ResourceManager.TestIcon;

            ImageManagement management = ImageManagement.Instance;

            management.Delete(logo);

            DataImageEntity entity = management.Find(logo);
            Assert.IsNull(entity);

            entity = management.Create(logo, logo);

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.ImageId != SystemConstants.UnknownId);
            Assert.IsNull(entity.OriginalImage);

            Image loadedImage = entity.GetImage();
            bool equals = Comparator.Equals(logo, loadedImage);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void ChildrenTest()
        {
            Image logo = ResourceManager.TestLogo;

            ImageManagement management = ImageManagement.Instance;

            DataImageEntity entity = management.Create(logo);
            DataImageEntity smallImage = management.Resize(entity, 2 * logo.Width, 2 * logo.Height);
            DataImageEntity bigImage = management.Resize(entity, 3 * logo.Width, 3 * logo.Height);

            DataImageCollection children = entity.Children;

            Assert.IsTrue(children.Count >= 2);
            Assert.IsTrue(children.Contains(smallImage));
            Assert.IsTrue(children.Contains(bigImage));
            Assert.IsFalse(children.Contains(entity));
            
            Assert.IsNotNull(children.Owner);
            Assert.AreEqual(entity, children.Owner);
            Assert.IsTrue(children.Readonly);

            Assert.IsNotNull(children[0]);
            Assert.IsNotNull(children[0].Container);
            Assert.AreEqual(children, children[0].Container);
            Assert.IsNotNull(children[0].Owner);
            Assert.AreEqual(entity.Id, children[0].Owner.Id);
        }

        [TestMethod]
        public void ExtractIconTest()
        {
            const string stringData = ResourceManager.TestIconData;
            byte[] data = Convert.FromBase64String(stringData);
            ImageManagement management = ImageManagement.Instance;

            DataImageEntity entity = management.Create(data);

            Assert.IsNotNull(entity);
            Assert.AreEqual(32, entity.Width);
            Assert.AreEqual(32, entity.Height);

            DataImageCollection children = entity.Children;

            Assert.IsTrue(children.Count >= 1);
        }

        [TestMethod]
        public void ExtractBitmapTest()
        {
            var logo = (Bitmap)ResourceManager.TestLogo;

            byte[] data = logo.ToByteArray();
            ImageManagement management = ImageManagement.Instance;

            DataImageEntity entity = management.Create(data);

            Assert.IsNotNull(entity);
        }
    }
}