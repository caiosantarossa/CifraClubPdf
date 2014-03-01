using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CifraClubPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Params:");
                Console.WriteLine("[0] Cifras Path");
                Console.WriteLine("[1] Output Path");
                Console.Read();
                return;
            }

            String pathCifras = args[0];
            String pathOutput = args[1];

            MakePdf(pathCifras, pathOutput);
        }

        private static void MakePdf(String pathCifras, String pathOutput)
        {
            // creation of a document-object
            Document myDocument = new Document(PageSize.A4);

            try
            {
                PdfWriter.GetInstance(myDocument, new FileStream(pathOutput + "/CifraClubPdf.pdf", FileMode.Create));

                myDocument.Open();
                myDocument.NewPage();

                BaseFont baseFont = BaseFont.CreateFont();
                Font fontTitulo = new Font(baseFont, 16, Font.BOLD);

                Font fontPaginaInicial = new Font(baseFont, 32, Font.BOLD);
                myDocument.Add(new Paragraph("\n\n\n\n Cifras \n\n\n\n\n\n", fontPaginaInicial));

                myDocument.NewPage();
                myDocument.Add(new Paragraph(" "));

                // carrega os arquivos do diretorio
                DirectoryInfo di = new DirectoryInfo(pathCifras);
                FileInfo[] rgFiles = di.GetFiles().OrderBy(f => f.Name).ToArray();

                Dictionary<string, string> indice = new Dictionary<string, string>();
                foreach (FileInfo fi in rgFiles)
                {
                    // Read the file as one string.
                    System.IO.StreamReader myFile;
                    if (fi.Name.Contains("utf8"))
                        myFile = new System.IO.StreamReader(fi.FullName, Encoding.UTF8);
                    else
                        myFile = new System.IO.StreamReader(fi.FullName, Encoding.UTF7);

                    // le o arquivo
                    string str = myFile.ReadToEnd();
                    string[] linhas = str.Trim().Split('\n');

                    string index = fi.Name.Split('-')[0];
                    string titulo = linhas[0];

                    if (titulo.Contains("-"))
                        titulo = titulo.Split('-')[1];

                    titulo = titulo.Replace("\n", "").Replace("\r", "");

                    if (indice.ContainsKey(titulo.Trim()))
                        throw new Exception("Música duplicada: " + index + "-" + titulo);

                    indice.Add(titulo.Trim(), index);

                    myFile.Close();
                }

                // monta a pagina de indice
                myDocument.NewPage();
                myDocument.Add(new Paragraph("Índice em ordem alfabética \n\n", fontTitulo));

                foreach (var item in indice.OrderBy(n => n.Key))
                    myDocument.Add(new Paragraph(item.Value + " - " + item.Key));

                foreach (FileInfo fi in rgFiles)
                {
                    // Read the file as one string.
                    System.IO.StreamReader myFile;
                    if (fi.Name.Contains("utf8"))
                        myFile = new System.IO.StreamReader(fi.FullName, Encoding.UTF8);
                    else
                        myFile = new System.IO.StreamReader(fi.FullName, Encoding.UTF7);

                    // le o arquivo
                    string str = myFile.ReadToEnd().Trim();

                    // verifica se contem a seção de acordes no final e remove
                    if (str.Contains("----------------- Acordes -----------------"))
                        str = str.Substring(0, str.IndexOf("----------------- Acordes -----------------")).Trim();

                    myDocument.NewPage();

                    string[] linhas = str.Split('\n');
                    string titulo = fi.Name.Split('-')[0] + " - ";

                    titulo += linhas[0];
                    str = str.Substring(str.IndexOf(linhas[1]));

                    myDocument.Add(new Paragraph(titulo, fontTitulo));
                    myDocument.Add(new Paragraph(str));

                    myFile.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            myDocument.Close();
        }
    }
}
