using System.Collections.Generic;
using Documentos;

namespace Clustering
{
    /// <summary>
    /// Implementación de DBSCAN adaptada del algoritmo utilizado por Weka para clustering.
    /// </summary>
    public class DBSCAN
    {
        private readonly MatrixSimilitud _laMatrixDeSimilitud;
        public double Eps { get; set; }
        public int MinPts { get; set; }
        protected int ClusterID { get; set; }
        public List<Documento> LstDocumentos;

        public DBSCAN(double eps, int minPts, MatrixSimilitud matrixSimilitud)
        {
            Eps = eps;
            MinPts = minPts;
            ClusterID = 1;
            _laMatrixDeSimilitud = matrixSimilitud;
        }

        public List<Documento> BuildCluster(List<Documento> lstDocumentos)
        {
            LstDocumentos = new List<Documento>();
            foreach (var doc in lstDocumentos)
            {
                var nuevo = (Documento) doc.Clone();
                nuevo.ClusterID = Documento.Unclassified;
                LstDocumentos.Add(nuevo);
            }

            foreach (var doc in LstDocumentos)
            {
                if (doc.ClusterID == Documento.Unclassified)
                    if (ExpandCluster(doc))
                        ClusterID++;
            }
            return LstDocumentos;
        }

        private bool ExpandCluster(Documento doc)
        {
            var seeds = GetRegion(doc);
            if (seeds.Count < MinPts)
            {
                doc.ClusterID = Documento.Ruido;
                return false;
            }

            doc.ClusterID = ClusterID;
            seeds.RemoveAll(x => x.PosicionEnLista == doc.PosicionEnLista);

            while (seeds.Count > 0)
            {
                var seed = seeds[0];
                var seedNeighbourhood = GetRegion(seed);

                if (seedNeighbourhood.Count >= MinPts)
                {
                    foreach (var seedNeighbour in seedNeighbourhood)
                    {
                        if (seedNeighbour.ClusterID == Documento.Unclassified ||
                            seedNeighbour.ClusterID == Documento.Ruido)
                        {
                            if (seedNeighbour.ClusterID == Documento.Unclassified)
                            {
                                seeds.Add(seedNeighbour);
                            }
                            seedNeighbour.ClusterID = ClusterID;
                        }
                    }
                }
                seeds.RemoveAt(0);
            }

            return true;
        }

        private List<Documento> GetRegion(Documento documento)
        {
            var vecinos = new List<Documento>();
            foreach (var doc in LstDocumentos)
            {
                var similitud = _laMatrixDeSimilitud.Get(doc.PosicionEnLista, documento.PosicionEnLista);
                if (similitud <= Eps) vecinos.Add(doc);
            }
            return vecinos;
        }
    }
}
