using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Egate_Payroll.Classes
{
    public static class ImageHelper
    {
        public static Size Resize(Size actualSize, Size maxSize)
        {
            double sourceWidth = actualSize.Width;
            double sourceHeight = actualSize.Height;
            double maxWidth = maxSize.Width;
            double maxHeight = maxSize.Height;

            double nPercent;
            double nPercentW;
            double nPercentH;

            nPercentW = maxWidth / sourceWidth;
            nPercentH = maxHeight / sourceHeight;
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
            }
            else
            {
                nPercent = nPercentW;
            }
            double destWidth = sourceWidth * nPercent;
            double destHeight = sourceHeight * nPercent;

            return new Size(destWidth, destHeight);
        }
    }
}
