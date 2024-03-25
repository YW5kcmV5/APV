using System.Drawing;
using APV.Avtoliga.Core.Entities;

namespace APV.Avtoliga.Core.BusinessLogic.Extensions
{
    public static class ImageManagementExtensions
    {
        public static Image GetImage(this ImageEntity entity)
        {
            return ImageManagement.Instance.GetImage(entity);
        }
    }
}