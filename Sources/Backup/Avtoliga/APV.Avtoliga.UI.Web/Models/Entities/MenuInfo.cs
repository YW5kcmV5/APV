namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class MenuInfo
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string AbsoluteUrl
        {
            get { return HtmlHelpers.FormatUrl(Url); }
        }

        public MenuInfo[] Items { get; set; }

        public static readonly MenuInfo[] MainMenu = new[]
            {
                new MenuInfo
                    {
                        Title = "Главная", Url = "~/", Items = null
                    },
                new MenuInfo
                {
                    Title = "О магазине", Url = "~/about/",
                    Items = new[]
                        {
                            new MenuInfo { Title = "Общая информация", Url = "~/about/" },
                            new MenuInfo { Title = "Отзывы покупателей", Url = "~/responses/" },
                            new MenuInfo { Title = "Новости", Url = "~/news/" },
                            new MenuInfo { Title = "Реквизиты", Url = "~/requisites/" },
                            new MenuInfo { Title = "Контакты", Url = "~/contacts/" },
                            new MenuInfo { Title = "Обратная связь", Url = "~/feedbacks/" }
                        }
                },
                new MenuInfo
                {
                    Title = "Запчасти", Url = "~/catalog",
                    Items = new[]
                        {
                            new MenuInfo { Title = "Каталог", Url = "~/catalog/" },
                            new MenuInfo { Title = "Поиск", Url = "~/search/" },
                            new MenuInfo { Title = "Производители", Url = "~/producers/" },
                            new MenuInfo { Title = "Спецпредложения", Url = "~/offers/" },
                            new MenuInfo { Title = "Статьи", Url = "~/articles/" },
                            new MenuInfo { Title = "Скачать прайс (.xls)", Url = "~/price/" },
                        }
                },
                new MenuInfo
                    {
                        Title = "Контакты", Url = "~/contacts", Items = null
                    },
            };
    }
}