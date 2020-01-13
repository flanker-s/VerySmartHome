﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
//using System.Drawing.Imaging;
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace VerySmartHome.Tools
{
    public sealed class ScreenColorAnalyzer
    {
        DColor ColorBufer = DColor.Empty;
        public MColor GetAvgScreenColor()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            graphics.Dispose();
            Bitmap avgPixel = new Bitmap(printscreen, 1, 1);
            printscreen.Dispose();
            DColor dAvgColor = avgPixel.GetPixel(0, 0);
            avgPixel.Dispose();
            ColorBufer = dAvgColor;
            MColor avgColor = DrowingToMediaColor(dAvgColor);
            return avgColor;
        }

        public MColor GetSceneAvgColorRGB()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            graphics.Dispose();
            var width = (printscreen.Width / 15);
            var hight = (printscreen.Height / 9);
            Bitmap leftUpSquare = printscreen.Clone(new Rectangle(width * 2, hight * 2, width, hight), printscreen.PixelFormat);
            Bitmap rightUpSquare = printscreen.Clone(new Rectangle(width * 13, hight * 2, width, hight), printscreen.PixelFormat);
            Bitmap centerSquare = printscreen.Clone(new Rectangle(width * 7, hight * 4, width, hight), printscreen.PixelFormat);
            Bitmap leftDownSquare = printscreen.Clone(new Rectangle(width * 2, hight * 7, width, hight), printscreen.PixelFormat);
            Bitmap rightDownSquare = printscreen.Clone(new Rectangle(width * 13, hight * 7, width, hight), printscreen.PixelFormat);
            printscreen.Dispose();
            Bitmap leftUpPixel = new Bitmap(leftUpSquare, 1, 1);
            Bitmap rightUpPixel = new Bitmap(rightUpSquare, 1, 1);
            Bitmap centerPixel = new Bitmap(centerSquare, 1, 1);
            Bitmap leftDownPixel = new Bitmap(leftDownSquare, 1, 1);
            Bitmap rightDownPixel = new Bitmap(rightDownSquare, 1, 1);

            DColor leftUp = leftUpPixel.GetPixel(0, 0);
            DColor rightUp = leftUpPixel.GetPixel(0, 0);
            DColor center = leftUpPixel.GetPixel(0, 0);
            DColor leftDown = leftUpPixel.GetPixel(0, 0);
            DColor rightDown = leftUpPixel.GetPixel(0, 0);

            List<DColor> Colors = new List<DColor>();
            Colors.Add(leftUp);
            Colors.Add(rightUp);
            Colors.Add(center);
            Colors.Add(leftDown);
            Colors.Add(rightDown);
            int r = 0, g = 0, b = 0;
            var count = Colors.Count;
            foreach (var color in Colors)
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }
            MColor AvgColor = new MColor();
            AvgColor.R = (byte)(r / count);
            AvgColor.G = (byte)(g / count);
            AvgColor.B = (byte)(b / count);
            return AvgColor;

        }
        public HSLColor GetMostCommonColorHSL()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            graphics.Dispose();
            Bitmap image = new Bitmap(printscreen, 128, 72);
            printscreen.Dispose();
            HSLColor pixel = GetAvgPixelHSL(image);
            return pixel;
        }
        public MColor GetMostCommonColorRGB()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            graphics.Dispose();
            Bitmap image = new Bitmap(printscreen, 320, 240);
            printscreen.Dispose();
            HSLColor hslPixel = GetAvgPixelHSL(image);
            DColor rgbPixel = ColorFromHSL(hslPixel);
            ColorBufer = rgbPixel;
            return DrowingToMediaColor(rgbPixel);
        }
        //HSLColor GetAvgPixelHSL(Bitmap image)
        //{
        //    int[] HueHistogram = new int[360];
        //    float[] HueSatSumHistogram = new float[360];
        //    float[] HueBrightSumHistogram = new float[360];

        //    Color pixel;
        //    for (int i = 0; i < image.Height; i++)
        //    {
        //        for (int j = 0; j < image.Width; j++)
        //        {
        //            pixel = image.GetPixel(j, i);
        //            int hue = (int)pixel.GetHue();
        //            HueHistogram[hue]++;
        //            HueSatSumHistogram[hue] += pixel.GetSaturation();
        //            HueBrightSumHistogram[hue] += GetAccurateBrightness(pixel);
        //        }
        //    }
        //    int temp = 0;
        //    int MostCommonHue = 0;
        //    for (int i = 0; i < HueHistogram.Length; i++)
        //    {
        //        if (HueHistogram[i] > temp)
        //        {
        //            temp = HueHistogram[i];
        //            MostCommonHue = i;
        //        }
        //    }
        //    image.Dispose();
        //    int mostCommonHueSatAvg = 100 * (int)HueSatSumHistogram[MostCommonHue] / (int)HueHistogram[MostCommonHue];
        //    int MostCommonHueBrightAvg = 100 * (int)HueBrightSumHistogram[MostCommonHue] / (int)HueHistogram[MostCommonHue];
        //    HSLColor AvgColor = new HSLColor(MostCommonHue, mostCommonHueSatAvg, MostCommonHueBrightAvg);
        //    return AvgColor;
        //}
        HSLColor GetAvgPixelHSL(Bitmap image)
        {
            int[] HueHistogram = new int[360];
            float[] HueSatSumHistogram = new float[360];
            float[] HueBrightSumHistogram = new float[360];

            DColor pixel;
            //DColor previosPixel = new DColor ();
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    pixel = image.GetPixel(j, i);                   
                    int hue = (int)pixel.GetHue();
                    if(GetAccurateBrightness(pixel) > 0) //>= previosPixel.GetBrightness())
                    {
                        HueHistogram[hue]++;
                        HueSatSumHistogram[hue] += pixel.GetSaturation();
                        HueBrightSumHistogram[hue] += GetAccurateBrightness(pixel);
                    }
                    //previosPixel = pixel;
                }
            }
            int[] HueSmoothHistogram = new int[360];
            for (int i = 0; i < HueHistogram.Length; i++)
            {
                if(i == 0)
                {
                    HueSmoothHistogram[i] = (HueHistogram[i] + HueHistogram[i + 1]) / 2;
                }
                else if (i == HueHistogram.Length - 1)
                {
                    HueSmoothHistogram[i] = (HueHistogram[i -1] + HueHistogram[i]) / 2;
                }
                else
                {
                    HueSmoothHistogram[i] = (HueHistogram[i - 1] + HueHistogram[i] + HueHistogram[i + 1]) / 3;
                }
            }
            int temp = 0;
            int MostCommonHue = 0;
            for (int i = 0; i < HueSmoothHistogram.Length; i++)
            {
                if (HueSmoothHistogram[i] > temp)
                {
                    temp = HueSmoothHistogram[i];
                    MostCommonHue = i;
                }
            }
            image.Dispose();
            int mostCommonHueSatAvg = 100 * (int)HueSatSumHistogram[MostCommonHue] / (int)HueSmoothHistogram[MostCommonHue];
            float MostCommonHueBrightAvg = 100 * HueBrightSumHistogram[MostCommonHue] / HueSmoothHistogram[MostCommonHue];
            HSLColor AvgColor = new HSLColor(MostCommonHue, mostCommonHueSatAvg, MostCommonHueBrightAvg);
            return AvgColor;
        }
        HSLColor GetAvgPixelHSLOptimized(Bitmap image)
        {
            int[] hueCounts = new int[360];
            float[] saturationSums = new float[360];
            float[] brightnessSums = new float[360];
            Color pixel;
            List<int> existingHues = new List<int>();

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    pixel = image.GetPixel(j, i);
                    int hue = (int)pixel.GetHue();
                    existingHues.Add(hue);
                    hueCounts[hue]++;
                    saturationSums[hue] += pixel.GetSaturation();
                    brightnessSums[hue] += GetAccurateBrightness(pixel);
                }
            }
            int count = 0;
            int mostCommonHue = 0;
            foreach (var hue in existingHues)
            {
                if(hueCounts[hue] > count)
                {
                    count = hueCounts[hue];
                    mostCommonHue = hue;
                }
            }
            int hueSaturationAvg = 100 * (int)saturationSums[mostCommonHue] / (int)hueCounts[mostCommonHue];
            int hueBrightAvg = 100 * (int)brightnessSums[mostCommonHue] / (int)hueCounts[mostCommonHue];
            HSLColor avgColor = new HSLColor(mostCommonHue, hueSaturationAvg, hueBrightAvg);
            return avgColor;
        }
        public float GetBrightness()
        {
            if (ColorBufer != DColor.Empty)
            {
                return ColorBufer.GetBrightness();
            }
            else return 0.5F;
        }
        float GetAccurateBrightness(Color c)
        { return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f; }
        MColor DrowingToMediaColor (DColor dcolor)
        {
            MColor mcolor = new MColor();
            mcolor.R = dcolor.R;
            mcolor.G = dcolor.G;
            mcolor.B = dcolor.B;
            mcolor.A = dcolor.A;
            return mcolor;
        }
        public static Color ColorFromHSL(HSLColor hSLColor)
        {
            double h = hSLColor.Hue;
            double s = hSLColor.Saturation/100;
            double l = hSLColor.Lightness/100;
            double r = 0, g = 0, b = 0;
            if (l != 0)
            {
                if (s == 0)
                    r = g = b = l;
                else
                {
                    double temp2;
                    if (l < 0.5)
                        temp2 = l * (1.0 + s);
                    else
                        temp2 = l + s - (l * s);

                    double temp1 = 2.0 * l - temp2;

                    r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, h);
                    b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;

            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }    
    }
}
