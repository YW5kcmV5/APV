using APV.Avtoliga.Common;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Common;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class ImageController
    {
        public static readonly ImageEntity NoPhotoImageEntity = ImageManagement.Instance.GetByName(Constants.NoPhotoImageName);

        public static ImageEntity GetImage(string imageIdOrValue)
        {
            ImageEntity entity = null;
            if (!string.IsNullOrWhiteSpace(imageIdOrValue))
            {
                long id;
                if ((long.TryParse(imageIdOrValue, out id)) && (id > SystemConstants.UnknownId))
                {
                    entity = ImageManagement.Instance.Find(id);
                }
                entity = entity ?? ImageManagement.Instance.FindByName(imageIdOrValue);
            }
            entity = entity ?? NoPhotoImageEntity;
            return entity;
        }
    }
}