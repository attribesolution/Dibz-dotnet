using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Common;
using ZXing;
using ZXing.Common;
using DIBZ.Logic;
using DIBZ.Logic.Auth;
using DIBZ.Common.Model;

namespace DIBZ
{
    public static class QRHelper
    {
        public static IHtmlString GenerateQrCode(this HtmlHelper html, string url, string alt = "QR code", int height = 75, int width = 75, int margin = 0)
        {
            var qrWriter = new BarcodeWriter();
            qrWriter.Format = BarcodeFormat.QR_CODE;
            qrWriter.Options = new EncodingOptions() { Height = height, Width = width, Margin = margin };

            using (var q = qrWriter.Write(url))
            {
                using (var ms = new MemoryStream())
                {
                    q.Save(ms, ImageFormat.Png);
                    var img = new TagBuilder("img");
                    img.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
                    img.Attributes.Add("alt", alt);
                    return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
                }
            }
        }
        public static IHtmlString GenerateQrCodeForAdmin(this HtmlHelper html, int offerId, string url, string alt = "QR code", int height = 75, int width = 75, int margin = 0)
        {
            var qrWriter = new BarcodeWriter();
            qrWriter.Format = BarcodeFormat.QR_CODE;
            qrWriter.Options = new EncodingOptions() { Height = height, Width = width, Margin = margin };

            using (var q = qrWriter.Write(url))
            {
                using (var ms = new MemoryStream())
                {
                    q.Save(ms, ImageFormat.Png);
                    var img = new TagBuilder("img");
                    img.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
                    img.Attributes.Add("alt", Convert.ToString(offerId));

                    var serverPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/QRCodes");
                    string fileName = string.Concat(offerId, ".png");
                    string path = Path.Combine(serverPath, fileName);
                    if (!File.Exists(path))
                        FileSaveHelper.SaveBytesToFile(ms.ToArray(), path);

                    return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
                }

            }
        }

        public static void GenerateAndSaveQrCode(string appUserEmail, int swapId, string url, string alt = "QR code", int height = 75, int width = 75, int margin = 0)
        {
            var qrWriter = new BarcodeWriter();
            qrWriter.Format = BarcodeFormat.QR_CODE;
            qrWriter.Options = new EncodingOptions() { Height = height, Width = width, Margin = margin };

            using (var q = qrWriter.Write(url))
            {
                using (var ms = new MemoryStream())
                {
                    q.Save(ms, ImageFormat.Png);
                    //var img = new TagBuilder("img");
                    //img.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
                    //img.Attributes.Add("alt", alt);

                    var serverPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/QRCodes");
                    string fileName = string.Concat(swapId, ".png");
                    string path = Path.Combine(serverPath, fileName);
                    if (!File.Exists(path))
                        FileSaveHelper.SaveBytesToFile(ms.ToArray(), path);
                    EmailHelper.EmailAttachement(appUserEmail, "QR Code", "please find the attached QR Code", path);

                }
            }
        }

        public static string GenerateAndSaveQrCodeForOffer(string appUserEmail, int offerId, string url, string alt = "QR code", int height = 75, int width = 75, int margin = 0)
        {
            var qrWriter = new BarcodeWriter();
            qrWriter.Format = BarcodeFormat.QR_CODE;
            qrWriter.Options = new EncodingOptions() { Height = height, Width = width, Margin = margin };

            using (var q = qrWriter.Write(url))
            {
                using (var ms = new MemoryStream())
                {
                    q.Save(ms, ImageFormat.Png);
                    var serverPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/QRCodes");
                    string fileName = string.Concat(offerId, ".png");
                    string path = Path.Combine(serverPath, fileName);
                    if (!File.Exists(path))
                        FileSaveHelper.SaveBytesToFile(ms.ToArray(), path);
                    return path;
                }
            }
        }
    }
}



    

