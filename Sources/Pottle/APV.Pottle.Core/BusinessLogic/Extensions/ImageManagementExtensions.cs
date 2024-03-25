using System.Drawing;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Core.BusinessLogic.Extensions
{
    public static class ImageManagementExtensions
    {
        public static Image GetImage(this DataImageEntity entity)
        {
            return ImageManagement.Instance.GetImage(entity);
        }
    }
}