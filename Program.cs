using TallComponents.PDF;
using TallComponents.PDF.Shapes;
using TallComponents.PDF.Layout.Paragraphs;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace EMFToBitmap
{
    class Program
    {
        private delegate void Bitmap2Pdf(System.Drawing.Bitmap bmp, string file_name);
        static void Main(string[] args)
        {
            // http://stackoverflow.com/questions/10147293/how-to-display-windows-metafile
            string input = @"D:\github\EMFToBitmap\sample.emf";
            string output1 = @"D:\github\EMFToBitmap\sample1.pdf";
            string output2 = @"D:\github\EMFToBitmap\sample2.pdf";
            string output3 = @"D:\github\EMFToBitmap\sample3.pdf";
            string output4 = @"D:\github\EMFToBitmap\sample4.pdf";

            EMF2PDF(input, output1, 1, new Bitmap2Pdf(Bitmap2PdfViaPdfKit));
            EMF2PDF(input, output2, 10, new Bitmap2Pdf(Bitmap2PdfViaPdfKit));
            EMF2PDF(input, output3, 1, new Bitmap2Pdf(Bitmap2PdfViaTallPdf));
            EMF2PDF(input, output4, 10, new Bitmap2Pdf(Bitmap2PdfViaTallPdf));

            Console.WriteLine("Hit return.");
            Console.ReadLine();
        }

        static void EMF2PDF(string input, string output, int scale, Bitmap2Pdf bitmap_to_pdf)
        {
            Metafile emf = new Metafile(input);
            float hr = emf.HorizontalResolution;
            float vr = emf.VerticalResolution;

            Console.WriteLine($"hr:{hr} vr:{vr}");

            bitmap_to_pdf(EMF2Bitmap(emf, hr, vr, scale), output);
        }

        static Bitmap EMF2Bitmap(Metafile emf, float hr, float vr, int scale)
        {
            Bitmap bmp = new Bitmap(emf.Width * scale, emf.Height * scale);

            bmp.SetResolution(hr * scale, vr * scale);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(emf, 0, 0);
                return bmp;
            }
        }
        static void Bitmap2PdfViaPdfKit(System.Drawing.Bitmap bmp, string file_name)
        {
            ImageShape img = new ImageShape(bmp);
            TallComponents.PDF.Document document = new TallComponents.PDF.Document();
            Page page = new Page(img.Width, img.Height);

            page.VisualOverlay.Add(img);
            document.Pages.Add(page);
            using (FileStream fs = new FileStream(file_name, FileMode.Create, FileAccess.Write))
            {
                document.Write(fs);
            }
        }

        static void Bitmap2PdfViaTallPdf(System.Drawing.Bitmap bmp, string file_name)
        {
            using (FileStream file = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), file_name), FileMode.Create, FileAccess.Write))
            {
                //private TallComponents.PDF.Layout.Fonts.Font helveticaFont = new TallComponents.PDF.Layout.Fonts.Font() { Path=Properties.Settings.Default.HelveticaNeueFontPath };
                TallComponents.PDF.Layout.Document pdf = new TallComponents.PDF.Layout.Document();

                var section = pdf.Sections.Add();

                TextParagraph h1 = new TextParagraph();
                section.Paragraphs.Add(h1);
                h1.StartOnNewPage = true;
                //h1.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Center;
                h1.Margin.Left = 0;
                h1.Margin.Bottom = 20;
                h1.Fragments.Add(new TallComponents.PDF.Layout.Paragraphs.Fragment("Heading"));

                var i1 = new TallComponents.PDF.Layout.Paragraphs.Image(bmp);
                //i1.MaskColor = TallComponents.PDF.Layout.Colors.Color.Transparent;
                section.Paragraphs.Add(i1);
                if (i1.Height > 470)
                    i1.Height = 470;
                else
                {
                    i1.Padding.Top = (470 - i1.Height) / 2;
                }
                i1.FitPolicy = FitPolicy.Shrink;
                i1.HorizontalAlignment = TallComponents.PDF.Layout.HorizontalAlignment.Center;
                i1.KeepAspectRatio = true;

                pdf.Write(file, false);
            }
        }
    }
}