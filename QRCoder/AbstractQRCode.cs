namespace QRCoder
{
    public abstract class AbstractQRCode<T>
    {
        protected QRCodeData qrCodeData;

        protected bool qrdataPath = true;
        
        protected AbstractQRCode(QRCodeData data, bool Path = false) {
            qrCodeData = data;
            qrdataPath = Path;
        }
        
        public abstract T GetGraphic(int pixelsPerModule);
    }
}