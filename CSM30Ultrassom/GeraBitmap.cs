using System.Drawing;

namespace CSM30Ultrassom
{
    public class GeraBitmap
    {
        public void ToBitmap(double[] rawImage, string nomeTxt)
        {
            int width = 60;
            int height = 60;
            string nomeBmp = @".\" + nomeTxt.Replace(".txt",".bmp");
            Bitmap Image = new Bitmap(width, height);

            for (int i = 0; i < width * height; i++)
            {
                if (rawImage[i] < 0)
                    rawImage[i] *= -1;
                Color color = Color.FromArgb((int)rawImage[i]%255, (int)rawImage[i]%255, (int)rawImage[i]%255);
                Image.SetPixel(i / width, i % height, color);
            }
            
            Image.Save(nomeBmp);
        }
    }
}
