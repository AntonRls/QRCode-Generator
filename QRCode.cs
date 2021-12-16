using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCode_generator
{
    class QRCode
    {
        private Bitmap qrCode;
        private Graphics gr;
        /// <summary>
        /// Создание класса QRCode
        /// </summary>
        /// <param name="size">Размер QRCode</param>
        public QRCode(int size)
        {
            qrCode = new Bitmap(size, size);
            initQRCode(false, null);
        }
        public QRCode(int size, bool fillBaground, Brush brush)
        {
            qrCode = new Bitmap(size, size);
            initQRCode(fillBaground, brush);
        }
        /// <summary>
        /// Алфавит. По умолчанию Русский. Нулевой элемент должен быть пустым
        /// </summary>
        public List<string> aphafit = new List<string>()
        {
            "","а","б","в","г","д","е","ё","ж","з","и","й","к","л","м","н","о","п","р","с","т","у","ф","х","ц","ч","ш","щ","ъ","ы","ь","э","ю","я"
        };
        private List<int> number = new List<int>()
        {
            1,2,4,8,16,32
        };

   
        /// <summary>
        /// Переводит текст в QR код
        /// </summary>
        /// <param name="text">Текст, не больше 7 символов</param>
        public void GenerateCode(string text)
        {

            int currentGroup = 1;
            if (text.Length <= 7) //максимально на один фрагмент
            {
                foreach (char c in text.ToCharArray())
                {
                    List<int> sum = new List<int>(0);
                    //Разбитие на числа
                    {
                        int Sum(List<int> l)
                        {
                            int result = 0;
                            foreach (int i in l)
                            {
                                result += i;
                            }
                            return result;
                        }

                        int index = 0;
                        for (int i = 0; i < aphafit.Count; i++)
                        {
                            if (aphafit[i] == c.ToString())
                            {
                                index = i;
                                
                                break;
                            }
                        }


                        int Grop = 1;


                        while (Sum(sum) != index)
                        {
                            for (int i = 0; i < Grop; i++)
                            {

                                for (int i2 = 0; i2 < number.Count; i2++)
                                {
                                    int lastI3 = -1;

                                    if (sum.Count < Grop)
                                    {
                                        sum.Add(number[i2]);
                                    }
                                    if (Sum(sum) == index)
                                    {
                                        break;
                                    }

                                    else if (Sum(sum) > index)
                                    {
                                        if (sum.Count >= Grop)
                                        {
                                            sum.Clear();
                                        }
                                        else if (sum.Count < Grop && lastI3 != -1)
                                        {
                                            sum.Remove(number[lastI3]);
                                        }

                                    }

                                    if (Grop > 1)
                                    {
                                        for (int i3 = 0; i3 < number.Count; i3++)
                                        {
                                            if (number[i2] == number[i3]) continue;

                                            if (sum.Count < Grop)
                                            {
                                                if (Sum(sum) != index)
                                                {
                                                    sum.Add(number[i3]);
                                                    lastI3 = i3;
                                                    if (Sum(sum) == index)
                                                    {
                                                        break;
                                                    }
                                                    else if (Sum(sum) > index)
                                                    {
                                                        sum.Remove(number[i3]);
                                                        lastI3 = -1;
                                                    }
                                                    else if (sum.Count >= Grop)
                                                    {
                                                        sum.Clear();
                                                        lastI3 = -1;
                                                    }

                                                }

                                            }



                                        }
                                    }
                                    if (Sum(sum) == index)
                                    {
                                        break;
                                    }
                                    else if (Sum(sum) > index)
                                    {
                                        if (sum.Count >= Grop)
                                        {
                                            sum.Clear();
                                        }
                                        else if (sum.Count < Grop && lastI3 != -1)
                                        {
                                            sum.Remove(number[lastI3]);
                                        }

                                    }

                                }
                                if (Sum(sum) == index)
                                {
                                    break;
                                }
                                else if (sum.Count >= Grop)
                                {
                                    sum.Clear();
                                }
                            }
                            Grop++;
                            if (Sum(sum) == index)
                            {
                                break;
                            }
                            else if (sum.Count >= Grop)
                            {
                                sum.Clear();

                            }
                        }
                        sum.Sort();

                        int count = 0;
                        for (; count < sum.Count; count++)
                        {
                            if (count != 0)
                            {
                                if (sum[count] == sum[count - 1])
                                {

                                    int temp = sum[count];
                                    int c22 = 0;
                                    while (sum.Contains(temp))
                                    {
                                        sum.Remove(temp);
                                        c22++;
                                    }

                                    if (c22 % 2 != 0)
                                    {
                                        c22 -= 1;
                                        sum.Add(temp);
                                    }
                                    sum.Add(temp * c22);
                                    sum.Sort();
                                    count -= 1;
                                }
                            }

                        }
                        sum.Sort();
                    }
                    //отрисовка qr кода
                    {
                        foreach (PointQr p in qrPoints)
                        {
                            if (p.group == currentGroup)
                            {
                                foreach (int s in sum)
                                {
                                    int index = 0;
                                    foreach (int num in number)
                                    {
                                        if (s == num) break;
                                        index++;
                                    }
                                    draPixel(gr, p.points[index]);

                                }
                                break;
                            }
                        }
                        currentGroup++;
                    }

                }
            }
        }
        /// <summary>
        /// Сохраняет QRCode по указаному пути
        /// </summary>
        /// <param name="path">Путь</param>
        public void SaveCode(string path)
        {
            qrCode.Save(path);
        }


        List<PointQr> qrPoints = new List<PointQr>(0);
        struct PointQr
        {
            public List<Point> points;
            public int group;
        }
        /// <summary>
        /// Должен быть вызван сразу после создания класса
        /// </summary>
        /// <param name="gr"></param>
        private void initQRCode(bool fill, Brush brush)
        {
            gr = Graphics.FromImage(qrCode);
            pixelSize = qrCode.Height / 10;
            if (fill)
            {
                gr.FillRectangle(brush, new Rectangle(new Point(0, 0), new Size(qrCode.Height, qrCode.Height)));
            }
            {
                for (int i = 0; i < 6; i++)
                {
                    draPixel(gr, new Point(i, 0));
                }
                for (int i = 0; i < 6; i++)
                {
                    draPixel(gr, new Point(0, i));
                }
                for (int i = 0; i < 6; i++)
                {
                    draPixel(gr, new Point(i, 5));
                }
                for (int i = 0; i < 6; i++)
                {
                    draPixel(gr, new Point(5, i));
                }
                draPixel(gr, new Point(7, 0));
                draPixel(gr, new Point(9, 0));
                draPixel(gr, new Point(2, 2));
                draPixel(gr, new Point(3, 2));
                draPixel(gr, new Point(3, 3));
                draPixel(gr, new Point(2, 3));
                draPixel(gr, new Point(0, 9));
                draPixel(gr, new Point(0, 7));
                draPixel(gr, new Point(9, 9));
                draPixel(gr, new Point(7, 9));
            }
            for (int i = 1; i < 5; i++)
            {
                List<Point> p = new List<Point>(0);
                for (int iY = 0; iY < 2; iY++)
                {
                    for (int iX = 0; iX < 3; iX++)
                    {
                        p.Add(getCord(iX, iY, i));
                    }

                }
                qrPoints.Add(new PointQr
                {
                    points = p,
                    group = i
                });

            }

            for (int i = 5; i < 8; i++)
            {
                List<Point> p = new List<Point>(0);
                for (int iY = 0; iY < 3; iY++)
                {
                    for (int iX = 0; iX < 2; iX++)
                    {
                        p.Add(getCord(iX, iY, i));

                    }

                }

                qrPoints.Add(new PointQr
                {
                    points = p,
                    group = i
                });

            }

        }
        private int pixelSize;
        Point getCord(int X, int Y, int group)
        {
            Point res;
            if (group == 1)
            {
                res = new Point(X + 7, (Y + 1) * group);
            }
            else if (group == 2)
            {
                res = new Point(X + 7, (Y + 3));
            }
            else if (group == 3)
            {
                res = new Point(X + 7, (Y + 5));
            }
            else if (group == 4)
            {
                res = new Point(X + 7, (Y + 7));
            }
            else if (group == 5)
            {
                res = new Point(X + 5, Y + 7);
            }
            else if (group == 6)
            {
                res = new Point(X + 3, Y + 7);
            }
            else if (group == 7)
            {
                res = new Point(X + 1, Y + 7);
            }
            else
            {
                res = new Point(0, 0);
            }
            return res;
        }
        void draPixel(Graphics gr, Point p)
        {
            gr.FillRectangle(Brushes.Black, new Rectangle(new Point(p.X * pixelSize, p.Y * pixelSize), new Size(pixelSize, pixelSize)));
        }
    }
}
