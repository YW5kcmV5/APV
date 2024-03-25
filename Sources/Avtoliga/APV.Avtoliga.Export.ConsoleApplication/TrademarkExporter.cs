using System;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Common.Periods;
using APV.GraphicsLibrary.Images;
using APV.Pottle.WebParsers.Avtoberg;

namespace APV.Avtoliga.Export.ConsoleApplication
{
    public class TrademarkExporter
    {
        public void Export(TrademarkEntity trademark, AvtobergModelInfo from)
        {
            if (trademark == null)
                throw new ArgumentNullException("trademark");
            if (from == null)
                throw new ArgumentNullException("from");

            string name = from.Name;

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException("from", "Model name is null or white space.");

            //from.Name
            //from.ModelPeriod
            //from.Key
            AnnualPeriodCollection period = from.ModelPeriod;

            ModelEntity model = ModelManagement.Instance.Find(trademark, name, period);
            model = model ?? new ModelEntity();

            model.Trademark = trademark;
            model.Name = name;
            model.Period = (period != null) ? period.ToString() : null;
            
            model.Save();
        }

        public void Export(AvtobergTrademarkInfo from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            string name = from.Name;

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException("from", "Trademark name is null or white space.");

            TrademarkEntity trademark = TrademarkManagement.Instance.FindByName(name);
            trademark = trademark ?? new TrademarkEntity();

            ImageContainer logo = from.Logo;
            ImageEntity logoEntity = (logo != null) ? ImageManagement.Instance.Create(logo.ToBitmap()) : null;
            UrlEntity urlEntity = (from.Url != null) ? UrlManagement.Instance.Create(from.Url.ToString()) : null;

            trademark.Name = name;
            trademark.About = from.Description;
            trademark.LogoImage = logoEntity;
            trademark.Url = urlEntity;

            trademark.Save();

            foreach (AvtobergModelInfo model in from.Models)
            {
                Export(trademark, model);
            }
        }
    }
}