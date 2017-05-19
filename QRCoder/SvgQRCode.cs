using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace QRCoder
{
    public class SvgQRCode : AbstractQRCode<string>, IDisposable
    {
        public SvgQRCode(QRCodeData data, bool qrpath = false) : base(data, qrpath) { }


        public override string GetGraphic(int pixelsPerModule)
        {
            Size viewBox = new Size(pixelsPerModule*qrCodeData.ModuleMatrix.Count, pixelsPerModule * qrCodeData.ModuleMatrix.Count);
            return GetGraphic(viewBox, Color.Black, Color.White);
        }

        public string GetGraphic(Size viewBox)
        {
            return GetGraphic(viewBox, Color.Black, Color.White);
        }

        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor)
        {
            return GetGraphic(viewBox, ColorTranslator.ToHtml(Color.FromArgb(darkColor.ToArgb())), ColorTranslator.ToHtml(Color.FromArgb(lightColor.ToArgb())));
        }

        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            return GetGraphic(viewBox, ColorTranslator.ToHtml(Color.FromArgb(darkColor.ToArgb())), ColorTranslator.ToHtml(Color.FromArgb(lightColor.ToArgb())), drawQuietZones);
        }

        public string GetGraphic(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true)
        {
            if (false == qrdataPath)
                return GetGraphicEx2(viewBox, darkColorHex, lightColorHex, drawQuietZones);
            
            StringBuilder svgFile = new StringBuilder(@"");
            int unitsPerModule = (int)Math.Floor(Convert.ToDouble(Math.Min(viewBox.Width, viewBox.Height)) / qrCodeData.ModuleMatrix.Count);
            var size = (qrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * unitsPerModule;
            int offset = drawQuietZones ? 0 : 4 * unitsPerModule;
            int drawableSize = size + offset;
            for (int x = 0; x < drawableSize; x = x + unitsPerModule)
            {
                for (int y = 0; y < drawableSize; y = y + unitsPerModule)
                {
                    var module = qrCodeData.ModuleMatrix[(y + unitsPerModule) / unitsPerModule - 1][(x + unitsPerModule) / unitsPerModule - 1];
                    if (module)
                    {
                        string temp = @"M " + (x - offset) + " " + (y - offset);
                        temp += " " + "L " + (x - offset + unitsPerModule) + " " + (y - offset);
                        temp += " " + "L " + (x - offset + unitsPerModule) + " " + (y - offset + unitsPerModule);
                        temp += " " + "L " + (x - offset) + " " + (y - offset + unitsPerModule);
                        temp += " " + "z ";
                        svgFile.AppendLine(temp);
                    }
                }
            }
            
            return svgFile.ToString();
        }

        public string GetGraphicEx(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true)
        {
            StringBuilder svgFile = new StringBuilder(@"<svg version=""1.1"" baseProfile=""full"" width=""" + viewBox.Width + @""" height=""" + viewBox.Height + @""" xmlns=""http://www.w3.org/2000/svg"">");
            int unitsPerModule = (int)Math.Floor(Convert.ToDouble(Math.Min(viewBox.Width, viewBox.Height)) / qrCodeData.ModuleMatrix.Count);
            var size = (qrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * unitsPerModule;
            int offset = drawQuietZones ? 0 : 4 * unitsPerModule;
            int drawableSize = size + offset;
            for (int x = 0; x < drawableSize; x = x + unitsPerModule)
            {
                for (int y = 0; y < drawableSize; y = y + unitsPerModule)
                {
                    var module = qrCodeData.ModuleMatrix[(y + unitsPerModule) / unitsPerModule - 1][(x + unitsPerModule) / unitsPerModule - 1];
                    string temp = @"<rect x=""" + (x - offset) + @""" y=""" + (y - offset) + @""" width=""" + unitsPerModule + @""" height=""" + unitsPerModule + @""" fill=""" + (module ? darkColorHex : lightColorHex) + @""" />";
                    if (module)
                        svgFile.AppendLine(temp);
                }
            }
            svgFile.Append(@"</svg>");
            return svgFile.ToString();
        }

        /// <summary>
        /// 可生成指定大小的二维码SVG文件
        /// </summary>
        /// <param name="viewBox"></param>
        /// <param name="darkColorHex"></param>
        /// <param name="lightColorHex"></param>
        /// <param name="drawQuietZones"></param>
        /// <returns></returns>
        public string GetGraphicEx2(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true)
        {
            StringBuilder svgFile = new StringBuilder(@"<svg version=""1.1"" baseProfile=""full"" width=""" + viewBox.Width + @""" height=""" + viewBox.Height + @""" xmlns=""http://www.w3.org/2000/svg"">");
            double unitsPerModule = Math.Round(Convert.ToDouble(Math.Min(viewBox.Width, viewBox.Height)) / qrCodeData.ModuleMatrix.Count, 2);
            var size = (int)Math.Ceiling((qrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * unitsPerModule);
            int offset = (int)Math.Ceiling(drawQuietZones ? 0 : 4 * unitsPerModule);
            for (int x = 0; x < qrCodeData.ModuleMatrix.Count; x++)
            {
                for (int y = 0; y < qrCodeData.ModuleMatrix.Count; y++)
                {
                    var module = qrCodeData.ModuleMatrix[y][x];
                    string temp = @"<rect x=""" + (x * unitsPerModule - offset) + 
                        @""" y=""" + (y * unitsPerModule - offset) + 
                        @""" width=""" + unitsPerModule + @""" height=""" + unitsPerModule + 
                        @""" fill=""" + (module ? darkColorHex : lightColorHex) + @""" />";
                    if (module)
                        svgFile.AppendLine(temp);
                }
            }
            svgFile.Append(@"</svg>");
            return svgFile.ToString();
        }

        public void Dispose()
        {
            this.qrCodeData = null;
        }
    }
}
