using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Avtoliga.UI.Web.Controllers.API;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public sealed class NewsModel : BaseModel
    {
        public NewsInfo[] News { get; private set; }

        public bool Archive { get; private set; }

        public NewsModel(bool archive)
        {
            Archive = archive;

            NewsCollection news = (archive)
                                      ? NewsManagement.Instance.ListArchive()
                                      : NewsManagement.Instance.ListLatest();

            News = news.Transform();
        }
    }
}