﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace FlexButton
{
    public class ColorHSL
    {
        private short _h;
        public short H
        {
            get { return _h; }
            set
            {
                _h = value;
                if (_h < 0) _h = 0;
            }
        }

        private short _s;
        public short S
        {
            get { return _s; }
            set
            {
                _s = value;
                if (_s < 0) _s = 0;
            }
        }

        private short _l;
        public short L
        {
            get { return _l; }
            set
            {
                _l = value;
                if (_l > 230) _l = 230;
                if (_l < 0) _l = 0;
            }
        }
    }

    public static class ColorTools
    {
        public static ColorHSL RGBtoHSL(Color colorRGB)
        {
            var hsl = new ColorHSL();

            float r, g, b, h, s, l; //this function works with floats between 0 and 1 
            r = colorRGB.R / 256.0f;
            g = colorRGB.G / 256.0f;
            b = colorRGB.B / 256.0f;

            float maxColor = Math.Max(r, Math.Max(g, b));
            float minColor = Math.Min(r, Math.Min(g, b));


            //R == G == B, so it's a shade of gray
            if (r == g && r == b)
            {
                h = 0.0f; //it doesn't matter what value it has       
                s = 0.0f;
                l = r; //doesn't matter if you pick r, g, or b   
            }
            else
            {
                l = (minColor + maxColor) / 2;

                s = l < 0.5 ? (maxColor - minColor) / (maxColor + minColor) : (maxColor - minColor) / (2.0f - maxColor - minColor);

                h = r == maxColor
                    ? (g - b) / (maxColor - minColor)
                    : g == maxColor ? 2.0f + (b - r) / (maxColor - minColor) : 4.0f + (r - g) / (maxColor - minColor);

                h /= 6; //to bring it to a number between 0 and 1
                if (h < 0) h++;
            }

            hsl.H = (short)(h * 255);
            hsl.L = (short)(l * 255);
            hsl.S = (short)(s * 255);

            return hsl;
        }

        public static Color HSLtoRGB(ColorHSL colorHSL)
        {
            double r, g, b; //this function works with floats between 0 and 1
            var h = colorHSL.H / 256.0;
            var s = colorHSL.S / 256.0;
            var l = colorHSL.L / 256.0;

            //If saturation is 0, the color is a shade of gray
            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                //Set the temporary values      
                double temp2 = l < 0.5 ? l * (1 + s) : (l + s) - (l * s);
                double temp1 = 2 * l - temp2;
                double tempr = h + 1.0 / 3.0;

                if (tempr > 1)
                {
                    tempr--;
                }

                double tempg = h;
                double tempb = h - 1.0 / 3.0;
                if (tempb < 0)
                    tempb++;

                //Red     
                r = tempr < 1.0 / 6.0
                    ? temp1 + (temp2 - temp1) * 6.0 * tempr
                    : tempr < 0.5 ? temp2 : tempr < 2.0 / 3.0 ? temp1 + (temp2 - temp1) * ((2.0 / 3.0) - tempr) * 6.0 : temp1;

                //Green       
                g = tempg < 1.0 / 6.0
                    ? temp1 + (temp2 - temp1) * 6.0 * tempg
                    : tempg < 0.5 ? temp2 : tempg < 2.0 / 3.0 ? temp1 + (temp2 - temp1) * ((2.0 / 3.0) - tempg) * 6.0 : temp1;

                //Blue    
                b = tempb < 1.0 / 6.0
                    ? temp1 + (temp2 - temp1) * 6.0 * tempb
                    : tempb < 0.5 ? temp2 : tempb < 2.0 / 3.0 ? temp1 + (temp2 - temp1) * ((2.0 / 3.0) - tempb) * 6.0 : temp1;
            }

            return Color.FromRgb(Convert.ToByte(r * 255), Convert.ToByte(g * 255), Convert.ToByte(b * 255));
        }
    }

}
