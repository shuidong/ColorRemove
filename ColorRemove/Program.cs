using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ColorRemove
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("请输入参数（待删除红色的文件）");
                return;
            }
            string item = args[0];//@"C:\Users\develop\Desktop\10000.png";
            //string item = @"C:\Users\develop\Desktop\wp_4a.png";
            string tmpFile = item + ".tmp";
            System.IO.File.Copy(item, tmpFile, true);
            Bitmap Color_Palet = ChangeToFormat32bppArgb(tmpFile);
            //保存用ファイル名 読み込んだファイル名に_を付ける
            string savefilename = item;
                //System.IO.Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + Path.GetExtension(item));
            //System.IO.Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item) + ".sin" + Path.GetExtension(item));

            // Color.FromArgb(0,255,0)は赤0 緑255 青0 で緑となります
            //Color.FromArgb(0,255,0)の部分はtry_hartmannさんのコードの仕様に合わせて変更してください
            DeletePixcel(Color_Palet, Color.FromArgb(255, 0, 0));

            Color_Palet.Save(savefilename, System.Drawing.Imaging.ImageFormat.Png);
        }

        // 画素の削除
        private static void DeletePixcel(Bitmap bitmap, Color c)
        {
            BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] pixcel = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(bmpdata.Scan0, pixcel, 0, pixcel.Length);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int idx = (x + y * bitmap.Width) * 4;
                    byte a, r, g, b;
                    b = pixcel[idx];
                    g = pixcel[idx + 1];
                    r = pixcel[idx + 2];
                    a = pixcel[idx + 3];
                    if ((c.B == b && c.G == g && r >= 0)
                        ||
                        (b < 120 && g < 120 && r >= 120)
                        )
                    {
                        // 次の2行のどちらかをご使用下さい
                        pixcel[idx] = pixcel[idx + 1] = pixcel[idx + 2] = pixcel[idx + 3] = 0; // 画素値,透過情報を0にする場合
                        pixcel[idx + 3] = 0; // 透過情報だけ0にする場合
                    }

                }
            }
            Marshal.Copy(pixcel, 0, bmpdata.Scan0, pixcel.Length);
            bitmap.UnlockBits(bmpdata);
        }

        // 画像の読み込み 透過情報なしの画像の場合透過情報付に変更する
        private static Bitmap ChangeToFormat32bppArgb(string filename)
        {
            Bitmap bmp = new Bitmap(filename);
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int width = bmp.Width, height = bmp.Height;
                byte[] pixcel = new byte[width * height * 4];
                Marshal.Copy(bmpdata.Scan0, pixcel, 0, pixcel.Length);
                bmp.UnlockBits(bmpdata);
                bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(pixcel, 0, bmpdata.Scan0, pixcel.Length);
                bmp.UnlockBits(bmpdata);
            }
            return bmp;
        }
    }
}
