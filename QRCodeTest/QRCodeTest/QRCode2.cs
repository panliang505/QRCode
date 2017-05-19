using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using QRCoder;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using Svg;

namespace CustomQRCode
{
    public enum PositionPatterns
    {
        Rect,
        RoundedRect,
        Circle
    }
    class QRCode2
    {
        public QRCodeGenerator QrcodeGenerator
        {
            get
            {
                if (null == _qrcodeGenerator)
                {
                    _qrcodeGenerator = new QRCodeGenerator();
                }
                return _qrcodeGenerator;
            }
        }

        public Bitmap Imge
        {
            get { return _image; }
        }

        public SvgDocument SVG
        {
            get { return _svg; }
        }


        public bool CreateQRCode(string str, QRCodeGenerator.ECCLevel eccLevel, System.Drawing.Size size)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            _qrdata = QrcodeGenerator.CreateQrCode(str, eccLevel);
            string svg = "";
            if (null != _qrdata)
            {
                SvgQRCode svgqrcode = new SvgQRCode(_qrdata);
                svg = svgqrcode.GetGraphic(size, Color.Black, Color.White);

                using (var stream = new MemoryStream(Encoding.Default.GetBytes(svg)))
                {
                    _svg = SvgDocument.Open<SvgDocument>(stream, null);
                    var g = new SvgGroup();
                    var ls = _svg.Children.ToList();
                    _svg.Children.Clear();
                    foreach (var c in ls)
                        g.Children.Add(c);

                    _svg.Children.Add(g);
                }

                DrawCorner();
            }
            _svg.Write(@"d:\1.svg");
            _image = _svg.Draw();// CustomQRCode.QRCode.Util.Svg2Bitmap(_svg);
            return true;
        }
        public SvgGroup SvgQrcodeGroup => _svg.Children[0] as SvgGroup;

        public void DrawCorner()
        {
            Bitmap Image;
            if (true)
            {
                Image = _svg.Draw();
                LuminanceSource source = new BitmapLuminanceSource(Image);
                var binBitmap = new BinaryBitmap(new HybridBinarizer(source));
                var result = new MultiFormatReader().decode(binBitmap);
                if (result != null)
                {
                    var moduleSize = ((FinderPattern)result.ResultPoints[0]).EstimatedModuleSize;
                    var fill = true;
                    var outterFill = new SvgColourServer(Color.Black);
                    var backFill = new SvgColourServer(Color.White);
                    var innerFill = new SvgColourServer(Color.Black);
                    for (var i = 0; i < 3; ++i)
                    {
                        if (!(result.ResultPoints[i] is FinderPattern))
                            continue;
                        var p = new PointF(result.ResultPoints[i].X, result.ResultPoints[i].Y);
                        var m = moduleSize * 3.5f;
                        var r1 = new RectangleF(p.X - m, p.Y - m, m * 2, m * 2);
                        m = moduleSize * 2.5f;
                        var r2 = new RectangleF(p.X - m, p.Y - m, m * 2, m * 2);
                        m = moduleSize * 1.5f;
                        var r3 = new RectangleF(p.X - m, p.Y - m, m * 2, m * 2);

                        var qry =
                            SvgQrcodeGroup.Children.Where(
                                    r => r is SvgRectangle && r1.IntersectsWith(((SvgRectangle)r).Bounds))
                                .ToList();
                        foreach (var c in qry)
                            SvgQrcodeGroup.Children.Remove(c);
                        SvgVisualElement c1, c2;
                        PositionPatterns POP = PositionPatterns.RoundedRect;
                        PositionPatterns PIP = PositionPatterns.RoundedRect;
                        switch (POP)
                        {
                            case PositionPatterns.RoundedRect:
                                var rect2 = new SvgRectangle
                                {
                                    X = r1.X,
                                    Y = r1.Y,
                                    Width = r1.Width,
                                    Height = r1.Height
                                };

                                SvgQrcodeGroup.Children.Add(rect2);
                                var rect = rect2;
                                rect.CornerRadiusX = r1.Width * 0.2f;
                                rect.CornerRadiusY = r1.Height * 0.2f;
                                c1 = rect;
                                var rect3 = new SvgRectangle
                                {
                                    X = r2.X,
                                    Y = r2.Y,
                                    Width = r2.Width,
                                    Height = r2.Height
                                };

                                SvgQrcodeGroup.Children.Add(rect3);
                                var rect1 = rect3;
                                rect1.CornerRadiusX = r2.Width * 0.2f;
                                rect1.CornerRadiusY = r2.Height * 0.2f;
                                c2 = rect1;
                                break;
                            case PositionPatterns.Circle:
                                var c = new SvgCircle
                                {
                                    CenterX = r1.X + r1.Width / 2,
                                    CenterY = r1.Y + r1.Height / 2,
                                    Radius = r1.Width / 2
                                };
                                SvgQrcodeGroup.Children.Add(c);
                                c1 = c;
                                var c3 = new SvgCircle
                                {
                                    CenterX = r2.X + r2.Width / 2,
                                    CenterY = r2.Y + r2.Height / 2,
                                    Radius = r2.Width / 2
                                };
                                SvgQrcodeGroup.Children.Add(c3);
                                c2 = c3;
                                break;
                            default:
                                var rect4 = new SvgRectangle
                                {
                                    X = r1.X,
                                    Y = r1.Y,
                                    Width = r1.Width,
                                    Height = r1.Height
                                };

                                SvgQrcodeGroup.Children.Add(rect4);
                                c1 = rect4;
                                var rect5 = new SvgRectangle
                                {
                                    X = r2.X,
                                    Y = r2.Y,
                                    Width = r2.Width,
                                    Height = r2.Height
                                };

                                SvgQrcodeGroup.Children.Add(rect5);
                                c2 = rect5;
                                break;
                        }

                        if (fill)
                            c1.Fill = outterFill;

                        c2.Fill = backFill;
                        switch (PIP)
                        {
                            case PositionPatterns.RoundedRect:
                                var rect1 = new SvgRectangle
                                {
                                    X = r3.X,
                                    Y = r3.Y,
                                    Width = r3.Width,
                                    Height = r3.Height
                                };

                                SvgQrcodeGroup.Children.Add(rect1);
                                var rect = rect1;
                                rect.CornerRadiusX = r3.Width * 0.2f;
                                rect.CornerRadiusY = r3.Height * 0.2f;
                                c1 = rect;
                                break;
                            case PositionPatterns.Circle:
                                var c = new SvgCircle
                                {
                                    CenterX = r3.X + r3.Width / 2,
                                    CenterY = r3.Y + r3.Height / 2,
                                    Radius = r3.Width / 2
                                };
                                SvgQrcodeGroup.Children.Add(c);
                                c1 = c;
                                break;
                            default:
                                var rect2 = new SvgRectangle
                                {
                                    X = r3.X,
                                    Y = r3.Y,
                                    Width = r3.Width,
                                    Height = r3.Height
                                };

                                SvgQrcodeGroup.Children.Add(rect2);
                                c1 = rect2;
                                break;
                        }
                        if (fill)
                            c1.Fill = innerFill;
                    }
                }
            }
        }

        protected Bitmap _image;

        protected SvgDocument _svg;

        protected QRCodeData _qrdata;

        protected QRCodeGenerator _qrcodeGenerator;
    }
}
