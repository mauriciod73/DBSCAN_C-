using System;
using System.Collections.Generic;
using System.Linq;
using Clustering;
using Documentos;

namespace Anteproyecto1
{
    public class Clustering
    {
        public List<Documento> LstDocumentos;
        private readonly MatrixSimilitud _laMatrixDeSimilitud;
        private double _eps;
        private int _minPts;

        public Clustering(List<Documento> lstDocumentos, MatrixSimilitud miMatrix, double eps, int minPts)
        {
            if (lstDocumentos == null) return;
            LstDocumentos = new List<Documento>(lstDocumentos);
            foreach (var doc in lstDocumentos)
                doc.ClusterID = 0;
            _laMatrixDeSimilitud = miMatrix;
            _eps = eps;
            _minPts = minPts;
        }

        public void Ejecutar(int k)
        {
            var inicio = DateTime.Now.Ticks;

            var kmeans = new KMeans().Cluster(LstDocumentos, k);
            var fin = DateTime.Now.Ticks;

            var transcurrido = new TimeSpan(fin - inicio).TotalMilliseconds;

            while (transcurrido <= 1000)
            {
                var cluster = MinimoDePuntos(kmeans);

                _eps = EpsilonMaximo(kmeans[cluster]);
                _minPts = kmeans[cluster].Count;

                LstDocumentos = new DBSCAN(_eps, _minPts, _laMatrixDeSimilitud).BuildCluster(LstDocumentos);
                var res = (from c in LstDocumentos
                           group c by c.ClusterID
                           into g
                           select new {clusterID = g.Key, miembros = g.Select(x => x.ClusterID).Count()}).OrderBy(
                               d => d.clusterID).ToList();

                Console.WriteLine("Eps: {0}\tMinPts: {1}\tK: {2}", _eps, _minPts, k);

                foreach (var r in res)
                {
                    Console.WriteLine("#Cluster: {0}\tMiembros: {1}", r.clusterID, r.miembros);
                }
                fin = DateTime.Now.Ticks;
                kmeans = new KMeans().Cluster(LstDocumentos, k);
                transcurrido = new TimeSpan(fin - inicio).TotalMilliseconds;
            }
            Console.WriteLine("Transcurrido: {0}", transcurrido);
        }

        private static Documento MinimoDePuntos(Dictionary<Documento, List<Documento>> kmeans)
        {
            var minimoDePuntos = int.MaxValue;
            Documento cluster = null;
            //Para cada cluster resultado del proceso de kmeans
            for (var i = 0; i < kmeans.Keys.Count; i++)
            {
                var c = kmeans.Keys.ElementAt(i);
                if (kmeans[c].Count >= minimoDePuntos) continue;
                minimoDePuntos = kmeans[c].Count;
                cluster = c;
            }
            return cluster;
        }

        private double EpsilonMaximo(IList<Documento> cluster)
        {
            var epsilonMaximo = double.MinValue;

            //se encuentra la distancia promedio para cada uno de los documentos del cluster
            for (var j = 0; j < cluster.Count; j++)
            {
                var suma = cluster.Sum(t => _laMatrixDeSimilitud.Get(cluster[j].PosicionEnLista, t.PosicionEnLista));
                var promedio = suma/cluster.Count;
                if (promedio > epsilonMaximo)
                    epsilonMaximo = promedio;
            }
            return epsilonMaximo;
        }
    }
}