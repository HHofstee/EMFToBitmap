using TallComponents.PDF;
using TallComponents.PDF.Shapes;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace EMFToBitmap
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://stackoverflow.com/questions/10147293/how-to-display-windows-metafile
            string input = @"D:\github\EMFToBitmap\sample.emf";
            string output = @"D:\github\EMFToBitmap\sample.pdf";

            Bitmap bmp = EMF2Bitmap(input);
            Bitmap2Pdf(bmp, output);
        }

        static Bitmap EMF2Bitmap(string input)
        {
            using (Metafile emf = new Metafile(input))
            { 
                Bitmap bmp = new Bitmap(emf.Width, emf.Height);
           
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(emf, 0, 0);
                    return bmp;
                }
            }
        }
        static void Bitmap2Pdf(System.Drawing.Bitmap bmp, string output)
        {
            ImageShape img = new ImageShape(bmp);
            Document document = new Document();
            Page page = new Page(img.Width, img.Height);

            page.VisualOverlay.Add(img);
            document.Pages.Add(page);
            using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write))
            {
                document.Write(fs);
            }
        }
    }
}