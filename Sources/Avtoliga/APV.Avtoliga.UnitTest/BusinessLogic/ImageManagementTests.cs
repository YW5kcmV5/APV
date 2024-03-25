using System;
using System.Drawing;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.UnitTest.BusinessLogic.Base;
using APV.Avtoliga.UnitTest.Resources;
using APV.GraphicsLibrary.Images;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Avtoliga.UnitTest.BusinessLogic
{
    [TestClass]
    public sealed class ImageManagementTests : BaseManagementTests
    {
        private static void SaveImage(Image image, string name, int? size = null)
        {
            ImageEntity imageEntity = ImageManagement.Instance.FindByName(name);
            if (imageEntity == null)
            {
                imageEntity = ImageManagement.Instance.Create(image, name);
            }
            else
            {
                using (var container = new ImageContainer(image))
                {
                    imageEntity.ImageFormat = container.ImageFormat;
                    imageEntity.Data = container.ToByteArray();
                    imageEntity.Save();
                }
            }

            Assert.IsNotNull(imageEntity);
            Assert.IsTrue(imageEntity.ImageId > 0);
            Assert.AreEqual(name, imageEntity.Name);
            Assert.AreNotEqual(Guid.Empty, imageEntity.Tag);

            imageEntity = ImageManagement.Instance.FindByName(name);

            Assert.IsNotNull(imageEntity);
        }

        [TestMethod]
        public void SaveNoPhotoImage()
        {
            const string name = @"NoPhoto";
            Image noPhotoImage = ResourceManager.NoPhoto;
            SaveImage(noPhotoImage, name);
        }

        [TestMethod]
        public void SaveNewsLogos()
        {
            SaveImage(ResourceManager.NewsLogo, "NewsLogo");
            SaveImage(ResourceManager.NewsHolidayLogo, "NewsHolidayLogo");
            SaveImage(ResourceManager.NewsSchedulerLogo, "NewsSchedulerLogo");
            SaveImage(ResourceManager.NewsWorkHoursLogo, "NewsWorkHoursLogo");
        }

        [TestMethod]
        public void SaveEagleEyesLogo()
        {
            SaveImage(ResourceManager.EagleEyesLogo, "EagleEyesLogo");
        }
    }
}