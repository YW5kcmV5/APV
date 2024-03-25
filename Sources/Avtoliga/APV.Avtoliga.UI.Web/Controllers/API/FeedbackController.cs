using System;
using System.Web;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.UI.Web.Models.Entities;
using APV.Common.Extensions;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class FeedbackController
    {
        private static string GetCookieId(long feedbackId)
        {
            return string.Format("Feedback#{0}", feedbackId);
        }

        public static bool GetLike(long feedbackId)
        {
            if (HttpContext.Current != null)
            {
                string cookieId = GetCookieId(feedbackId);
                string likedValue = CookiesManager.GetString(cookieId);
                return (likedValue == "1");
            }
            return false;
        }

        public static int SetLike(long feedbackId, bool liked)
        {
            int likes = -1;
            if (HttpContext.Current != null)
            {
                likes = FeedbackManagement.Instance.Set(feedbackId, liked);
                string likedValue = (liked) ? "1" : "0";
                string cookieId = GetCookieId(feedbackId);
                CookiesManager.SetString(cookieId, likedValue);
            }
            return likes;
        }

        public static ApiResult SaveFeedback(FeedbackInfo feedback)
        {
            string error = null;

            try
            {
                if (feedback == null)
                {
                    error = "Данные не указаны.";
                }
                else if (!feedback.Email.IsValidEmail())
                {
                    error = string.Format("Неверный формат электронной почты (\"{0}\").", feedback.Email);
                }
                else if (!feedback.IsValid())
                {
                    error = "Обязательные поля не заполнены.";
                }
                else
                {
                    FeedbackEntity feedbackEntity = feedback.Transform();
                    FeedbackManagement.Instance.Save(feedbackEntity);
                }
            }
            catch (Exception)
            {
                error = "Неизвестная ошибка. Обратитесь, пожалуйста, в службу поддержки.";
            }

            ApiResult result = (!string.IsNullOrWhiteSpace(error))
                                   ? new ApiResult(false, "Ошибка при сохранении отзыва.\r\n" + error + "\r\nПопробуйте, пожалуйста, ещё раз.")
                                   : new ApiResult(true, "Ваш отзыв успешно сохранён.");

            return result;
        }
    }
}