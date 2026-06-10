using System;
using System.Collections.Generic;
using System.Reflection;
namespace jxzt
{
    /// <summary>
    /// DBSCAN 密度聚类算法实现
    /// 用于识别学生绘图中的线型（直线、圆、椭圆等），通过密度聚类区分前景点和噪声点
    /// 参数 eps：邻域半径；minPts：核心点所需的最小邻居数
    /// </summary>
    public class Dbscan
    {
        private double eps;
        private int minPts;

        public Dbscan(double epsilon, int minPts)
        {
            eps = epsilon;
            this.minPts = minPts;
        }

        public int[] Cluster(List<int> points)
        {

            int n = points.Count;
            int[] labels = new int[n];
            int clusterId = 0;

            // 初始化所有点的标签为-1，表示未分类
            for (int i = 0; i < n; i++)
            {

                labels[i] = -1;
            }

            // 遍历所有点
            for (int i = 0; i < n; i++)
            {

                int p = points[i];

                // 如果点已经分类，则跳过
                if (labels[i] != -1)
                {

                    continue;
                }

                // 找到p的邻居点
                List<int> neighbors = GetNeighbors(points, i);

                // 如果邻居点数量小于minPts，则将p标记为噪声点
                if (neighbors.Count < minPts)
                {

                    labels[i] = 0;
                    continue;
                }

                // 新建一个簇
                clusterId++;
                labels[i] = clusterId;

                // 扩展簇
                ExpandCluster(points, ref labels, p, neighbors, clusterId, eps, minPts);
            }

            return labels;
        }


        public void ExpandCluster(List<int> points, ref int[] labels, int p, List<int> neighbors, int clusterId, double eps, int minPts)
        {

            // 遍历邻居点
            for (int i = 0; i < neighbors.Count; i++)
            {

                int q = neighbors[i];
                int index = points.IndexOf(q);

                // 如果邻居点未分类，则将其加入簇中
                if (labels[index] == -1)
                {

                    labels[index] = clusterId;

                    // 找到q的邻居点
                    List<int> qNeighbors = GetNeighbors(points, index);

                    // 如果邻居点数量大于等于minPts，则将其加入扩展簇的邻居点列表中
                    if (qNeighbors.Count >= minPts)
                    {

                        neighbors.AddRange(qNeighbors);
                    }
                }
                // 如果邻居点已经被分类为噪声点，则将其重新分类到当前簇中
                else if (labels[index] == 0)
                {

                    labels[index] = clusterId;
                }
            }
        }

        private List<int> GetNeighbors(List<int> points, int q)
        {
            List<int> neighbors = new List<int>();

            for (int i = 0; i < points.Count; i++)
            {
                if (Math.Abs(points[i] - points[q]) <= eps && i != q)
                {
                    neighbors.Add(points[i]);
                }
            }

            return neighbors;
        }
    }
}
