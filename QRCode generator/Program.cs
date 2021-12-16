using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCode_generator
{
    class Program
    {
        static void Main(string[] args)
        {
            QRCode qr = new QRCode(1000, true, Brushes.White);
            qr.GenerateCode("привет");
            qr.SaveCode("res.png");
        }
     }
}
