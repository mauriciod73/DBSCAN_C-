using System;
using System.Collections.Generic;
using System.Linq;
using Documentos;

namespace Clustering
{
    public class KMeans
    {
        private List<Documento> _lstDocumentos;

        public Dictionary<Documento, List<Documento>> Cluster(List<Documento> lstDocumentos, int k)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            _lstDocumentos = lstDocumentos;
            var randomCenters = PickRandomCenters(random, k);

            return ProcessGroups(randomCenters);
        }

        private Dictionary<Documento,List<Documento>> ProcessGroups(List<Documento> randomCenters)
        {
            var centerAssignments = GetCenterAssignments(randomCenters);

            List<Documento> oldCenters = null;
            while (true)
            {
                //calculate average center
                var newCenters = GetNewCenters(centerAssignments);

                if (CentersEqual(newCenters, oldCenters))
                {
                    break;
                }

                centerAssignments = GetCenterAssignments(newCenters);

                oldCenters = newCenters;
            }
            return centerAssignments;
        }

        private List<Documento> PickRandomCenters(Random random, int k)
        {
            //pick random _lstDocumentos
            var randomCenters = new List<Documento>();
            var pickedPointCount = 0;
            while (pickedPointCount < k)
            {
                var point = _lstDocumentos[random.Next(0, _lstDocumentos.Count - 1)];
                if (randomCenters.Contains(point)) continue;
                randomCenters.Add(point);
                pickedPointCount++;
            }
            return randomCenters;
        }

        private static bool CentersEqual(IEnumerable<Documento> newCenters, IEnumerable<Documento> oldCenters)
        {
            if (newCenters == null || oldCenters == null)
            {
                return false;
            }

            return newCenters.Select(newCenter => oldCenters.Any(newCenter.Equals)).All(found => found);
        }


        private List<Documento> GetNewCenters(Dictionary<Documento, List<Documento>> centerAssignments)
        {
            var newCenters = new List<Documento>();

            foreach (var center in centerAssignments.Keys)
            {
                var total = new double[_lstDocumentos[0].Terminos.Length];
                var average = new double[_lstDocumentos[0].Terminos.Length];

                for(var i=0; i<_lstDocumentos[0].Terminos.Length; i++)
                {
                    foreach (var doc in centerAssignments[center])
                    {
                        total[i] += doc.Terminos[i];
                    }

                    average[i] = total[i]/total.Length;
                }

                var newCenter = new Documento {Terminos = average};
                newCenters.Add(newCenter);
            }
            return newCenters;
        }

        private Dictionary<Documento, List<Documento>> GetCenterAssignments(List<Documento> centers)
        {
            var centerAssignments = new Dictionary<Documento, List<Documento>>();

            foreach (var point in centers)  
            {
                centerAssignments.Add(point, new List<Documento>());
            }

            foreach (var point in _lstDocumentos)
            {
                if(point.ClusterID==Documento.Ruido) continue;

                Documento closestCenter = null;
                var closestCenterDistance = double.MaxValue;

                foreach (var centerPoint in centers)
                {
                    var centerDistance = 1 - Documento.SimilitudCosenos(point, centerPoint);
                    
                    if (centerDistance <= closestCenterDistance)
                    {
                        closestCenterDistance = centerDistance;
                        closestCenter = centerPoint;
                    }
                }

                lock (centerAssignments)
                {
                    if (closestCenter != null) centerAssignments[closestCenter].Add(point);
                }
            }

            return centerAssignments;
        }
    }
}
