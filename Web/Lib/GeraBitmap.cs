using System;
using System.Drawing;
using System.Web;

namespace Web.Lib
{
    public class GeraBitmap
    {
        public void ToBitmap(double[] rawImage, string nomeTxt, string path)
        {
            double min = 5000000, max = 0, den;
            int width = 60;
            int height = 60;
            //var path = HttpContext.Current.Server.MapPath("~/Content/Images/");
            DateTime dataHora = DateTime.Now;
            String dataHoraString = dataHora.Day.ToString() + "."
                                    + dataHora.Month.ToString() + "."
                                    + dataHora.Year.ToString() + "-"
                                    + dataHora.Hour.ToString() + "."
                                    + dataHora.Minute.ToString() + "."
                                    + dataHora.Second.ToString();
            string nomeBmp = path + nomeTxt.Replace(".txt", "");
            nomeBmp = nomeBmp + "%" + dataHoraString + "%" + ".bmp";
            Bitmap Image = new Bitmap(width, height);

            for (int i = 0; i < width * height; i++)
            {
                if (rawImage[i] < min)
                    min = rawImage[i];
                else if (rawImage[i] > max)
                    max = rawImage[i];
            }
            den = 255/(max - min);
            for (int i = 0; i < width * height; i++)
            {
                rawImage[i] = den * (rawImage[i] - min);
                /*
                if (rawImage[i] < 0)
                    rawImage[i] *= -1;
                */
                Color color = Color.FromArgb((int)rawImage[i], (int)rawImage[i], (int)rawImage[i]);
                Image.SetPixel(i / width, i % height, color);
            }

            Image.Save(nomeBmp);
        }
    }
}