using System.Drawing;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.UnitTest.BusinessLogic.Base;
using APV.Avtoliga.UnitTest.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Avtoliga.UnitTest.BusinessLogic
{
    [TestClass]
    public sealed class ProducerManagementTests : BaseManagementTests
    {
        [TestMethod]
        public void CreateDepoTest()
        {
            //Data:
            Image image = ResourceManager.DepoLogo;
            const string url = @"http://www.depoautolamp.com/";
            const string name = @"Depo";
            const string title = @"Каталог автомобильной оптики Depo: фары Depo, противотуманные фары Depo, фонари Depo";
            const string about = 
@"Компания Depo основана в 1977 году. Философия компании — производство доступных и высококачественных автозапчастей. Качество и инновации стали ключевыми аспектами для компании, благодаря чему оптика Depo сейчас является одной из самых популярных в мире.
На сегодняшний день компания Depo — крупнейший производитель автомобильной оптики в Азии. В ассортименте каталога Depo - фары, противотуманные фары, задние фонари, поворотники. Компания Depo производит настолько качественную продукцию, что разница между оригинальной и альтернативной оптикой заключается лишь в логотипе производителя.
В производственные технологии фар, противотуманных фар и фонарей Depo постоянно внедряются новые технологические методики, которые направлены на повышение качества выпускаемой продукции, а также на увеличение ассортимента.
Depo обладает лицензиями ведущих автопроизводителей на изготовление альтернативной оптики. Фары Depo, противотуманные фары Depo и задние фонари Depo, выпускаемые для новых моделей иномарок, проходят обязательное тестирование. В настоящее время оптика Депо представлена в 100 странах мира. В США, одном из крупнейших автомобильных рынков мира, фары Depo имеют прочную позицию в том числе за счет высокого качества, соответствующего стандартам SAE & DOT и FMVSS-108 (Федеральный стандарт безопасности автотранспорта). Кроме этого, само производство фар сертифицировано в соответствии с ISO 9002 и QS 9000. В России вся оптика Depo сертифицирована по системе ГОСТ-Р.
Благодаря Depo эра безопасной и качественной оптики для тюнинга авто наступила! Тюнинговая оптика Depo выпускается под брэндом DEPO Performance и представлена в нашем каталоге. Все модели этой линейки разработаны, чтобы увеличить яркость света фар авто, не влияя на функциональность и не ставя под угрозу безопасность пассажиров. Яркость и видимость - главные элементы безопасности, требуемые законом при разработке альтернативной оптики. Depo действительно сделали привлекательную альтернативную оптику, не забыв об ее основном назначении - ярко и безопасно светить.
Вся информация о производителе взята с официального сайта Depo.
В магазине Avtoberg вы можете воспользоваться быстрым поиском по онлайн каталогу оптики Depo, заказать и купить фары Depo на все популярные модели иномарок BMW, Nissan, Honda, Mazda и др.";

            //Logo
            ImageEntity logoEntity = ImageManagement.Instance.Create(image);
            UrlEntity urlEntity = UrlManagement.Instance.Create(url);

            ProducerEntity producer = ProducerManagement.Instance.FindByName(name);
            producer = producer ?? new ProducerEntity();

            producer.Name = name;
            producer.Url = urlEntity;
            producer.Logo = logoEntity;
            producer.Title = title;
            producer.About = about;

            producer.Save();
        }

        [TestMethod]
        public void CreateSonarTest()
        {
            //Data:
            Image image = ResourceManager.SonarLogo;
            const string name = @"Sonar";
            const string title = @"Каталог тюнинговой оптики Sonar: тюнинговые фары и фонари Sonar";
            const string about =
@"Sonar - крупнейший производитель оптики в Тайване. Компания начала свою работу в 1964 году и в настоящий момент имеет три крупных современных производственных предприятия и штат сотрудников около 400 человек.
Sonar производит качественные альтернативные фары для автомобилей, основная цель производства — создание дизайнерской оптики, отличной от оригинальной. Компанию Sonar по праву считают мировым лидером в сфере производства тюнинговой оптики для авто. В ассортименте каталога Sonar - светодиодные фары и светодиодные фонари.
Дизайн оптики, разработанный Sonar для многих марок автомобилей, — уникален, поэтому он привлекает внимание к авто и подчеркивает его стиль. При этом важно, что тюнинговые фары Sonar конструктивно идентичны оригиналу, поэтому для их установки не требуются доработки (предусматривают OEM установку). Кроме того, тюнинговая оптика Sonar DOT/SAE сертифицирована.
Оптика Sonar производится на автоматизированом ультрасовременном оборудовании. Компания Sonar постоянно уделяется внимание расширению модельного ряда выпускаемой ею продукции и слежению за качеством производимой оптики.
Только в магазине Avtoberg вы можете заказать и купить фары Sonar на многие иномарки. Доставка комплектов тюнинговых фар и фонарей Sonar на наши пункты выдачи занимает 7 дней. Всего неделя ожидания и Вы сможете преобразить свое любимое авто!";

            //Logo
            ImageEntity logoEntity = ImageManagement.Instance.Create(image);

            ProducerEntity producer = ProducerManagement.Instance.FindByName(name);
            producer = producer ?? new ProducerEntity();

            producer.Name = name;
            producer.Url = null;
            producer.Logo = logoEntity;
            producer.Title = title;
            producer.About = about;

            producer.Save();
        }

        [TestMethod]
        public void CreateTycTest()
        {
            //Data:
            Image image = ResourceManager.TycLogo;
            const string name = @"TYC";

            //Logo
            ImageEntity logoEntity = ImageManagement.Instance.Create(image);

            ProducerEntity producer = ProducerManagement.Instance.FindByName(name);
            producer = producer ?? new ProducerEntity();

            producer.Name = name;
            producer.Logo = logoEntity;

            producer.Save();
        }

        [TestMethod]
        public void CreateTygTest()
        {
            //Data:
            Image image = ResourceManager.TygLogo;
            const string name = @"Tong Yang Group";

            //Logo
            ImageEntity logoEntity = ImageManagement.Instance.Create(image);

            ProducerEntity producer = ProducerManagement.Instance.FindByName(name);
            producer = producer ?? new ProducerEntity();

            producer.Name = name;
            producer.Logo = logoEntity;

            producer.Save();
        }
    }
}