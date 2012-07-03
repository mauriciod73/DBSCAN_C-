using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Documentos;

namespace Anteproyecto1
{
    internal class Program
    {
        private static List<Documento> _lstDocumentos;

        private static void Main(string[] args)
        {
            StreamReader stream = null;

            try
            {
                var inicio = DateTime.Now;
                _lstDocumentos = new List<Documento>();

                stream = new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read));

                var id = 0;
                while (!stream.EndOfStream)
                {
                    var linea = stream.ReadLine();

                    if (linea.Contains("@") || linea.Contains("%") || String.IsNullOrEmpty(linea))
                        continue;

                    var dim = new List<double>();
                    foreach (var s in linea.Substring(0, linea.Length - 2).Split(','))
                    {
                        dim.Add(Convert.ToDouble(s, new CultureInfo("en-US")));
                    }
                    _lstDocumentos.Add(new Documento {Terminos = dim.ToArray(), PosicionEnLista = id});
                    id++;
                }

                var miMatrizDeSimilitudes = new MatrixSimilitud(_lstDocumentos);
                
                //for (var k = 2; k < 15; k++)
                //{
                    new Clustering(_lstDocumentos, miMatrizDeSimilitudes, 0, 0).Ejecutar(Convert.ToInt32(Math.Ceiling(Math.Sqrt(_lstDocumentos.Count))));
                    Console.WriteLine();
                //}
                var fin = DateTime.Now;
                var tiempo = new TimeSpan(fin.Ticks - inicio.Ticks);
                Console.WriteLine(tiempo.TotalMilliseconds);
            }
            finally
            {
                Console.ReadKey();
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
    }
}
