using System;
using System.Collections.Generic;
using System.Linq;

namespace Documentos
{
    public class Documento : IEquatable<Documento>
    {
        public int PosicionEnLista { get; set; }
        public const int Unclassified = 0;
        public const int Ruido = -1;
        public bool EsVisitado { get; set; }
        public double[] Terminos { get; set; }
        public int ClusterID { get; set; }
        public bool Clasificado { get; set; }
        public bool EsRuido { get; set; }

        public Documento()
        {
            ClusterID = Unclassified;
            EsVisitado = false;
        }

        public static double DistanciaEuclidiana(Documento doc1, Documento doc2)
        {
            var suma = 0.0;
            for (var i = 0; i < doc1.Terminos.Length; i++)
            {
                suma += (doc1.Terminos[i] - doc2.Terminos[i])*(doc1.Terminos[i] - doc2.Terminos[i]);
            }
            return Math.Sqrt(suma);
        }

        /// <summary>
        /// Retorna la distancia entre dos documentos utilizando la similitud de cosenos
        /// </summary>
        /// <param name="doc1"></param>
        /// <param name="doc2"></param>
        /// <returns>distancia (por similitud de cosenos) entre este documento y el argumento</returns>
        public static double SimilitudCosenos(Documento doc1, Documento doc2)
        {
            var productoPunto = ProductoPunto(doc1.Terminos, doc2.Terminos);
            var magnitudDoc1 = Magnitud(doc1.Terminos);
            var magnitudDoc2 = Magnitud(doc2.Terminos);

            return productoPunto/(magnitudDoc1*magnitudDoc2);
        }

        private static double ProductoPunto(double[] vec1, double[] vec2)
        {
            var productoPunto = 0.0;
            for (var i = 0; i < vec1.Length; i++)
            {
                productoPunto += (vec1[i]*vec2[i]);
            }
            return productoPunto;
        }

        private static double Magnitud(double[] vector)
        {
            return Math.Sqrt(ProductoPunto(vector, vector));
        }

        public object Clone()
        {
            return new Documento
                       {
                           ClusterID = ClusterID,
                           Clasificado = Clasificado,
                           EsRuido = EsRuido,
                           EsVisitado = EsVisitado,
                           PosicionEnLista = PosicionEnLista,
                           Terminos = (double[]) Terminos.Clone()
                       };
        }

        public bool Equals(Documento other)
        {
            return PosicionEnLista == other.PosicionEnLista;
            //return Terminos.SequenceEqual(other.Terminos);
        }
    }
}
