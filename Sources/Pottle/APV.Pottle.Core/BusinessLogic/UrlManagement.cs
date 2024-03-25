using System;
using APV.EntityFramework;
using APV.Common;
using APV.Common.Extensions;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class UrlManagement : BaseManagement<UrlEntity, UrlCollection, UrlDataLayerManager>
    {
        [AnonymousAccess]
        public string FormatUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (!AbsoluteUri.IsWellFormedAbsoluteUriString(url))
                throw new ArgumentOutOfRangeException("url", string.Format("Url \"{0}\" is not weel formed absolute url.", url));

            var absoluteUri = new AbsoluteUri(url);
            return absoluteUri.Url;
        }

        [AnonymousAccess]
        public string GetHost(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (!AbsoluteUri.IsWellFormedAbsoluteUriString(url))
                throw new ArgumentOutOfRangeException("url", string.Format("Url \"{0}\" is not weel formed absolute url.", url));

            var uri = new AbsoluteUri(url);
            string host = uri.Host;
            return FormatUrl(host);
        }

        [AnonymousAccess]
        public byte[] GetHashCode(string url)
        {
            string formattedUrl = FormatUrl(url);
            return formattedUrl.Hash256();
        }

        [AnonymousAccess]
        public string GetUrl(UrlEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return FormatUrl(entity.Url);
        }

        [AnonymousAccess]
        public Uri GetUri(UrlEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return new Uri(FormatUrl(entity.Url));
        }

        [AnonymousAccess]
        public byte[] GetHashCode(UrlEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return GetHashCode(entity.Url);
        }

        [AnonymousAccess]
        public UrlEntity Find(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (!AbsoluteUri.IsWellFormedAbsoluteUriString(url))
                throw new ArgumentOutOfRangeException("url", string.Format("Url \"{0}\" is not weel formed absolute url.", url));

            byte[] hashCode = GetHashCode(url);
            return DatabaseManager.Find(hashCode);
        }

        [AnonymousAccess]
        public UrlEntity Create(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (!AbsoluteUri.IsWellFormedAbsoluteUriString(url))
                throw new ArgumentOutOfRangeException("url", string.Format("Url \"{0}\" is not weel formed absolute url.", url));

            byte[] hashCode = GetHashCode(url);
            UrlEntity entity = DatabaseManager.Find(hashCode);
            if (entity != null)
            {
                return entity;
            }

            string formattedUrl = FormatUrl(url);
            string hostUrl = GetHost(url);
            byte[] hostHashCode = GetHashCode(hostUrl);

            UrlEntity hostEntity = DatabaseManager.Find(hostHashCode);
            if (hostEntity == null)
            {
                hostEntity = new UrlEntity
                    {
                        Url = hostUrl,
                        Alive = null,
                        HashCode = hostHashCode,
                    };
                hostEntity.Save();

                if (formattedUrl == hostUrl)
                {
                    return hostEntity;
                }
            }

            entity = new UrlEntity
                {
                    Url = formattedUrl,
                    Alive = null,
                    HashCode = hashCode,
                    HostUrl = hostEntity,
                };
            entity.Save();

            return entity;
        }

        [AdminAccess]
        public void Verify(UrlEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.IsNew)
                throw new ArgumentOutOfRangeException("entity", "Specified entity is new (is not stored in database).");

            bool alive = WebUtility.IsUrlAvailable(entity.Url);
            entity.Alive = alive;
            entity.VerifiedAt = DatabaseManager.GetUtcNow();
            entity.Save();
        }

        [AdminAccess]
        public void Verify(DateTime olderThen)
        {
            UrlCollection items = DatabaseManager.GetToVerify(olderThen);
            foreach (UrlEntity url in items)
            {
                Verify(url);
            }
        }

        public static readonly UrlManagement Instance = (UrlManagement)EntityFrameworkManager.GetManagement<UrlEntity>();
    }
}