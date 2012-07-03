using System.Collections.Generic;

namespace Documentos
{
    public class MatrixSimilitud
    {
        private readonly double[][] _similitudes;

        public MatrixSimilitud(IList<Documento> losDocumentos)
        {
            var totalDoc = losDocumentos.Count;
            _similitudes = new double[totalDoc][];
            for(var i=1; i< totalDoc; i++)
            {
                _similitudes [i] = new double[i];
                for (var j = 0; j < i - 1; j++)
                    _similitudes[i][j] = 1 - Documento.SimilitudCosenos(losDocumentos[i], losDocumentos[j]);
                    //_similitudes[i][j] = Documento.DistanciaEuclidiana(losDocumentos[i], losDocumentos[j]);
            }
        }

        public double Get(int i , int j)
        {
            if (i == j) return 0;
            if (i > j) return _similitudes[i][j];
            return _similitudes[j][i];
        }

        public override string ToString()
        {
            var result = "";
            for(var i=1; i < _similitudes.GetUpperBound(0)+1; i++)
            {
                for (var j = 0; j < _similitudes[i].GetUpperBound(0); j++) 
                    result +=_similitudes[i][j].ToString("0.00") +"\t";
                result += "\r\n";
            }
            return result;
        }
    }
}
