using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public sealed class HeaderModel : BaseModel
    {
        public MenuInfo[] MainMenu
        {
            get { return MenuInfo.MainMenu; }
        }
    }
}