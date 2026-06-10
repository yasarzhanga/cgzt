using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace jxzt
{
    /// <summary>
    /// Algorithm - 图像处理算法静态类
    /// 包含所有图像处理相关的核心算法，如圆拟合、椭圆拟合、细化、分割等
    /// </summary>
    public static class Algorithm
    {
        /// <summary>
        /// 最小二乘法圆拟合
        /// 根据给定的像素点坐标，拟合出圆的圆心和半径
        /// 使用最小二乘法求解圆的方程 (x-a)^2 + (y-b)^2 = r^2
        /// </summary>
        /// <param name="points">输入像素点列表</param>
        /// <returns>拟合的圆数据（包含圆心坐标和半径）</returns>
        public static Circle LeastSquaresCircleFitting(List<PositionInt> points)
        {
            int N = points.Count;
            if (N < 3)
            {
                return null;
            }

            double sumX = 0.0;
            double sumY = 0.0;
            double sumX2 = 0.0;
            double sumY2 = 0.0;
            double sumX3 = 0.0;
            double sumY3 = 0.0;
            double sumXY = 0.0;
            double sumXY2 = 0.0;
            double sumX2Y = 0.0;

            for (int pId = 0; pId < N; ++pId)
            {
                sumX += points[pId].x;
                sumY += points[pId].y;

                double x2 = points[pId].x * points[pId].x;
                double y2 = points[pId].y * points[pId].y;
                sumX2 += x2;
                sumY2 += y2;

                sumX3 += x2 * points[pId].x;
                sumY3 += y2 * points[pId].y;
                sumXY += points[pId].x * points[pId].y;
                sumXY2 += points[pId].x * y2;
                sumX2Y += x2 * points[pId].y;
            }

            double C, D, E, G, H;
            double a, b, c;

            C = N * sumX2 - sumX * sumX;
            D = N * sumXY - sumX * sumY;
            E = N * sumX3 + N * sumXY2 - (sumX2 + sumY2) * sumX;
            G = N * sumY2 - sumY * sumY;
            H = N * sumX2Y + N * sumY3 - (sumX2 + sumY2) * sumY;

            a = (H * D - E * G) / (C * G - D * D);
            b = (H * C - E * D) / (D * D - G * C);
            c = -(a * sumX + b * sumY + sumX2 + sumY2) / N;
            Circle circle = new Circle();
            circle.X = (float)(-a / 2.0);
            circle.Y = (float)(-b / 2.0);
            circle.Radius = (float)(Math.Sqrt(a * a + b * b - 4 * c) / 2.0);

            return circle;
        }

        /// <summary>
        /// 最小二乘法椭圆拟合
        /// 根据给定的像素点坐标，拟合出椭圆的几何参数
        /// 使用一般式 Ax^2 + Bxy + Cy^2 + Dx + Ey + F = 0 求解椭圆参数
        /// </summary>
        /// <param name="points">输入像素点列表</param>
        /// <returns>拟合的椭圆数据（包含中心、长轴、短轴、倾角）</returns>
        public static Ellipse LeastSquaresEllipseFitting(List<PositionInt> points)
        {
            double A = 0.00, B = 0.00, C = 0.00, D = 0.00, E = 0.00;
            double x2y2 = 0.0, x1y3 = 0.0, x2y1 = 0.0, x1y2 = 0.0, x1y1 = 0.0, yyy4 = 0.0, yyy3 = 0.0, yyy2 = 0.0, xxx2 = 0.0, xxx1 = 0.0, yyy1 = 0.0, x3y1 = 0.0, xxx3 = 0.0;
            int N = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                double xi = points[i].x;
                double yi = points[i].y;

                x2y2 += xi * xi * yi * yi;
                x1y3 += xi * yi * yi * yi;
                x2y1 += xi * xi * yi;
                x1y2 += xi * yi * yi;
                x1y1 += xi * yi;
                yyy4 += yi * yi * yi * yi;
                yyy3 += yi * yi * yi;
                yyy2 += yi * yi;
                xxx2 += xi * xi;
                xxx1 += xi;
                yyy1 += yi;
                x3y1 += xi * xi * xi * yi;
                xxx3 += xi * xi * xi;
            }
            Matrix<double> matrix = Matrix<double>.Build.DenseOfArray(new double[5, 5]{
            {x2y2, x1y3, x2y1, x1y2, x1y1 },
            { x1y3, yyy4, x1y2, yyy3, yyy2 },
            { x2y1, x1y2, xxx2, x1y1, xxx1 },
            { x1y2, yyy3, x1y1, yyy2, yyy1 },
            { x1y1, yyy2, xxx1, yyy1, N }
        });

            Matrix<double> matrix2 = Matrix<double>.Build.DenseOfArray(new double[5, 1] { { x3y1 }, { x2y2 }, { xxx3 }, { x2y1 }, { xxx2 } });
            Matrix<double> matrix3 = Matrix<double>.Build.DenseOfArray(new double[5, 1] { { A }, { B }, { C }, { D }, { E } });

            //求矩阵matrix的逆，结果为InverseMatrix

            Matrix<double> inverseMatrix = matrix.Inverse();

            ///求参数A,B,C,D,E
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    matrix3[i, 0] += inverseMatrix[i, j] * (-matrix2[j, 0]);
                }
            }
            A = matrix3[0, 0];
            B = matrix3[1, 0];
            C = matrix3[2, 0];
            D = matrix3[3, 0];
            E = matrix3[4, 0];

            ///求拟合结果重要参数
            double Xc = (2 * B * C - A * D) / (A * A - 4 * B);
            double Yc = (2 * D - A * C) / (A * A - 4 * B);
            double a = Math.Sqrt(Math.Abs(2 * (A * C * D - B * C * C - D * D + 4 * B * E - A * A * E) / ((A * A - 4 * B) * (B - Math.Sqrt(A * A + (1 - B) * (1 - B)) + 1))));
            double b = Math.Sqrt(Math.Abs(2 * (A * C * D - B * C * C - D * D + 4 * B * E - A * A * E) / ((A * A - 4 * B) * (B + Math.Sqrt(A * A + (1 - B) * (1 - B)) + 1))));
            double theta = -Math.Atan2(a * a - b * b * B, a * a * B - b * b);

            return new Ellipse((float)Xc, (float)Yc, (float)a, (float)b, (float)theta);

        }

        /// <summary>
        /// Zhang-Suen 细化算法
        /// 将二值图像中的线条细化到单像素宽度，便于后续分析和识别
        /// 该算法通过迭代删除边界像素，保留连通骨架
        /// </summary>
        /// <param name="layerSize">图层尺寸</param>
        /// <param name="image_colors">像素颜色数组</param>
        /// <returns>细化后的像素坐标列表</returns>
        public static List<PositionInt> Zhang_Suen(Size layerSize, Color32[] image_colors)
        {
            ColorsWithIndex src = new ColorsWithIndex(layerSize.width, layerSize.height, image_colors);
            int[] Zhangmude = new int[9];
            List<PositionInt> deletelist = new List<PositionInt>();
            while (true)
            {
                for (int x = 1; x < layerSize.width - 1; x++)
                {
                    for (int y = 1; y < layerSize.height - 1; y++)
                    {
                        if (src[x, y].a > 0)
                        {
                            Zhangmude[0] = 1;
                            if (src[x, y - 1].a > 0) Zhangmude[1] = 1;
                            else Zhangmude[1] = 0;
                            if (src[x + 1, y - 1].a > 0) Zhangmude[2] = 1;
                            else Zhangmude[2] = 0;
                            if (src[x + 1, y].a > 0) Zhangmude[3] = 1;
                            else Zhangmude[3] = 0;
                            if (src[x + 1, y + 1].a > 0) Zhangmude[4] = 1;
                            else Zhangmude[4] = 0;
                            if (src[x, y + 1].a > 0) Zhangmude[5] = 1;
                            else Zhangmude[5] = 0;
                            if (src[x - 1, y + 1].a > 0) Zhangmude[6] = 1;
                            else Zhangmude[6] = 0;
                            if (src[x - 1, y].a > 0) Zhangmude[7] = 1;
                            else Zhangmude[7] = 0;
                            if (src[x - 1, y - 1].a > 0) Zhangmude[8] = 1;
                            else Zhangmude[8] = 0;
                            int whitepointtotal = 0;
                            for (int k = 1; k < 9; k++)
                            {
                                //得到1的个数
                                whitepointtotal = whitepointtotal + Zhangmude[k];
                            }
                            if ((whitepointtotal >= 2) && (whitepointtotal <= 6))
                            {
                                //得到01的个数
                                int ap = 0;
                                if ((Zhangmude[1] == 0) && (Zhangmude[2] == 1)) ap++;
                                if ((Zhangmude[2] == 0) && (Zhangmude[3] == 1)) ap++;
                                if ((Zhangmude[3] == 0) && (Zhangmude[4] == 1)) ap++;
                                if ((Zhangmude[4] == 0) && (Zhangmude[5] == 1)) ap++;
                                if ((Zhangmude[5] == 0) && (Zhangmude[6] == 1)) ap++;
                                if ((Zhangmude[6] == 0) && (Zhangmude[7] == 1)) ap++;
                                if ((Zhangmude[7] == 0) && (Zhangmude[8] == 1)) ap++;
                                if ((Zhangmude[8] == 0) && (Zhangmude[1] == 1)) ap++;
                                //计算bp
                                int bp = 0;
                                bp += Zhangmude[1];
                                bp += Zhangmude[2] << 1;
                                bp += Zhangmude[3] << 2;
                                bp += Zhangmude[4] << 3;
                                bp += Zhangmude[5] << 4;
                                bp += Zhangmude[6] << 5;
                                bp += Zhangmude[7] << 6;
                                bp += Zhangmude[8] << 7;
                                if (ap == 1 || bp == 65 || bp == 5 || bp == 20 || bp == 80 || bp == 13 || bp == 22 || bp == 52 || bp == 133 || bp == 141 || bp == 54)
                                {
                                    if ((Zhangmude[1] * Zhangmude[3] * Zhangmude[5] == 0) && (Zhangmude[3] * Zhangmude[5] * Zhangmude[7] == 0))
                                    {
                                        deletelist.Add(new PositionInt(x, y));
                                    }
                                }
                            }
                        }
                    }
                }

                if (deletelist.Count() == 0) break;
                foreach (var deleteItem in deletelist)
                {
                    src[deleteItem.x, deleteItem.y] = new Color32(0, 0, 0, 0);
                }
                deletelist.Clear();
                for (int x = 1; x < layerSize.width - 1; x++)
                {
                    for (int y = 1; y < layerSize.height - 1; y++)
                    {
                        if (src[x, y].a > 0)
                        {
                            Zhangmude[0] = 1;
                            if (src[x, y - 1].a > 0) Zhangmude[1] = 1;
                            else Zhangmude[1] = 0;
                            if (src[x + 1, y - 1].a > 0) Zhangmude[2] = 1;
                            else Zhangmude[2] = 0;
                            if (src[x + 1, y].a > 0) Zhangmude[3] = 1;
                            else Zhangmude[3] = 0;
                            if (src[x + 1, y + 1].a > 0) Zhangmude[4] = 1;
                            else Zhangmude[4] = 0;
                            if (src[x, y + 1].a > 0) Zhangmude[5] = 1;
                            else Zhangmude[5] = 0;
                            if (src[x - 1, y + 1].a > 0) Zhangmude[6] = 1;
                            else Zhangmude[6] = 0;
                            if (src[x - 1, y].a > 0) Zhangmude[7] = 1;
                            else Zhangmude[7] = 0;
                            if (src[x - 1, y - 1].a > 0) Zhangmude[8] = 1;
                            else Zhangmude[8] = 0;
                            int whitepointtotal = 0;
                            for (int k = 1; k < 9; k++)
                            {
                                //得到1的个数
                                whitepointtotal = whitepointtotal + Zhangmude[k];
                            }
                            if ((whitepointtotal >= 2) && (whitepointtotal <= 6))
                            {
                                //得到01的个数
                                int ap = 0;
                                if ((Zhangmude[1] == 0) && (Zhangmude[2] == 1)) ap++;
                                if ((Zhangmude[2] == 0) && (Zhangmude[3] == 1)) ap++;
                                if ((Zhangmude[3] == 0) && (Zhangmude[4] == 1)) ap++;
                                if ((Zhangmude[4] == 0) && (Zhangmude[5] == 1)) ap++;
                                if ((Zhangmude[5] == 0) && (Zhangmude[6] == 1)) ap++;
                                if ((Zhangmude[6] == 0) && (Zhangmude[7] == 1)) ap++;
                                if ((Zhangmude[7] == 0) && (Zhangmude[8] == 1)) ap++;
                                if ((Zhangmude[8] == 0) && (Zhangmude[1] == 1)) ap++;
                                //计算bp
                                int bp = 0;
                                bp += Zhangmude[1];
                                bp += Zhangmude[2] << 1;
                                bp += Zhangmude[3] << 2;
                                bp += Zhangmude[4] << 3;
                                bp += Zhangmude[5] << 4;
                                bp += Zhangmude[6] << 5;
                                bp += Zhangmude[7] << 6;
                                bp += Zhangmude[8] << 7;
                                if (ap == 1 || bp == 65 || bp == 5 || bp == 20 || bp == 80 || bp == 13 || bp == 22 || bp == 52 || bp == 133 || bp == 141 || bp == 54)
                                {
                                    if ((Zhangmude[1] * Zhangmude[3] * Zhangmude[7] == 0) && (Zhangmude[1] * Zhangmude[5] * Zhangmude[7] == 0))
                                    {
                                        deletelist.Add(new PositionInt(x, y));
                                    }
                                }
                            }
                        }
                    }
                }
                Debug.Log(deletelist.Count);
                if (deletelist.Count == 0) break;
                foreach (var deleteItem in deletelist)
                {
                    src[deleteItem.x, deleteItem.y] = new Color32(0, 0, 0, 0);
                }
                deletelist.Clear();
            }

            List<PositionInt> res = src.GetPositionInts();
            return res;

        }

        /// <summary>
        /// 图像连通区域分割算法
        /// 将图像中所有相连的像素点分组，每组为一个独立的连通区域
        /// 使用 BFS（广度优先搜索）进行连通性分析
        /// </summary>
        /// <param name="layerSize">图层尺寸</param>
        /// <param name="Image_colors">像素颜色数组</param>
        /// <returns>连通区域列表，每个区域包含一组相连的像素坐标</returns>
        public static List<List<PositionInt>> GetSlices(Size layerSize, Color32[] Image_colors)
        {
            List<List<PositionInt>> slices = new List<List<PositionInt>>();

            ColorsWithIndex src = new ColorsWithIndex(layerSize.width, layerSize.height, Image_colors);

            bool CheckPointExist(HashSet<PositionInt> dst, int width, int height)
            {
                Stack<PositionInt> stack = new Stack<PositionInt>();
                stack.Push(new PositionInt(width, height));
                while (stack.Count > 0)
                {
                    PositionInt current = stack.Pop();
                    if (current.x >= 0 && current.x < layerSize.width && current.y >= 0 && current.y < layerSize.height)
                    {
                        if (src[current.x, current.y].a > 0)
                        {
                            dst.Add(current);
                            if (!dst.Contains(new PositionInt(current.x - 1, current.y)))
                            {
                                stack.Push(new PositionInt(current.x - 1, current.y));
                            }
                            if (!dst.Contains(new PositionInt(current.x + 1, current.y)))
                            {
                                stack.Push(new PositionInt(current.x + 1, current.y));
                            }
                            if (!dst.Contains(new PositionInt(current.x, current.y - 1)))
                            {
                                stack.Push(new PositionInt(current.x, current.y - 1));
                            }
                            if (!dst.Contains(new PositionInt(current.x, current.y + 1)))
                            {
                                stack.Push(new PositionInt(current.x, current.y + 1));
                            }
                        }
                    }
                }
                return dst.Count > 0;
            }

            for (int i = 0; i < layerSize.width; i++)
            {
                for (int j = 0; j < layerSize.height; j++)
                {
                    if (src[i, j].a > 0)
                    {
                        HashSet<PositionInt> connectpool = new HashSet<PositionInt>();
                        if (CheckPointExist(connectpool, i, j))
                        {

                            slices.Add(new List<PositionInt>(connectpool));

                            foreach (var item in connectpool)
                            {
                                src[item.x, item.y] = new Color32(0, 0, 0, 0);
                            }
                        }

                    }
                }
            }

            return slices;
        }

        public static List<List<PositionInt>> GetSlices(ColorsWithIndex src)
    {
        List<List<PositionInt>> slices = new List<List<PositionInt>>();
        ColorsWithIndex srcClone = new ColorsWithIndex(src.width,src.height,src.data);
        bool CheckPointExist(HashSet<PositionInt> dst, int width, int height)
        {
            Stack<PositionInt> stack = new Stack<PositionInt>();
            stack.Push(new PositionInt(width, height));
            while (stack.Count > 0)
            {
                PositionInt current = stack.Pop();
                if (current.x >= 0 && current.x < srcClone.width && current.y >= 0 && current.y < srcClone.height)
                {
                    if (srcClone[current.x, current.y].a > 0)
                    {
                        dst.Add(current);
                        if (!dst.Contains(new PositionInt(current.x - 1, current.y)))
                        {
                            stack.Push(new PositionInt(current.x - 1, current.y));
                        }
                        if (!dst.Contains(new PositionInt(current.x + 1, current.y)))
                        {
                            stack.Push(new PositionInt(current.x + 1, current.y));
                        }
                        if (!dst.Contains(new PositionInt(current.x, current.y - 1)))
                        {
                            stack.Push(new PositionInt(current.x, current.y - 1));
                        }
                        if (!dst.Contains(new PositionInt(current.x, current.y + 1)))
                        {
                            stack.Push(new PositionInt(current.x, current.y + 1));
                        }
                    }
                }
            }
            return dst.Count > 0;
        }

        for (int i = 0; i < srcClone.width; i++)
        {
            for (int j = 0; j < srcClone.height; j++)
            {
                if (srcClone[i, j].a > 0)
                {
                    HashSet<PositionInt> connectpool = new HashSet<PositionInt>();
                    if (CheckPointExist(connectpool, i, j))
                    {

                        slices.Add(new List<PositionInt>(connectpool));

                        foreach (var item in connectpool)
                        {
                            srcClone[item.x, item.y] = new Color32(0, 0, 0, 0);
                        }
                    }
                }
            }
        }
        return slices;
    }

        /// <summary>
        /// 获取凸包（使用 Graham Scan 算法）
        /// 找出包围所有点的最小凸多边形
        /// 用于计算有向包围盒等后续处理
        /// </summary>
        /// <param name="points">输入点列表</param>
        /// <returns>凸包上的点列表（按逆时针顺序）</returns>
        public static List<PositionInt> GetConvexHull(List<PositionInt> points)
    {
        int n = points.Count;
        if (n < 3) return points;

            points.Sort((a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

            PositionInt p1 = points[0], p2 = points[n - 1];
            List<PositionInt> up = new List<PositionInt>
        {
            p1
        };
            List<PositionInt> down = new List<PositionInt>
        {
            p1
        };

            for (int i = 1; i < n; i++)
            {
                if (i == n - 1 || Tools.CrossProduct(p1, points[i], p2) > 0)
                {
                    while (up.Count >= 2 && Tools.CrossProduct(up[up.Count - 2], up[up.Count - 1], points[i]) <= 0)
                        up.RemoveAt(up.Count - 1);
                    up.Add(points[i]);
                }
                if (i == n - 1 || Tools.CrossProduct(p1, points[i], p2) < 0)
                {
                    while (down.Count >= 2 && Tools.CrossProduct(down[down.Count - 2], down[down.Count - 1], points[i]) >= 0)
                        down.RemoveAt(down.Count - 1);
                    down.Add(points[i]);
                }
            }

            down.Reverse();
            up.AddRange(down);
            return up;
        }
        //空心算法
        public static List<PositionInt> GetHollow(Size layerSize, Color32[] Image_colors)
        {
            List<PositionInt> hollow = new List<PositionInt>();
            ColorsWithIndex src = new ColorsWithIndex(layerSize.width, layerSize.height, Image_colors);
            for (int i = 1; i < layerSize.width - 1; i++)
            {
                for (int j = 1; j < layerSize.height - 1; j++)
                {
                    if (src[i, j].a > 0)
                    {
                        if ((src[i + 1, j].a == 0) || (src[i - 1, j].a == 0) || (src[i, j + 1].a == 0) || (src[i, j - 1].a == 0))
                        {
                            hollow.Add(new PositionInt(i, j));
                        }
                    }
                }
            }


            return hollow;
        }

        /// <summary>
        /// 判断点是否在四边形内
        /// 使用叉积法判断点相对于四边形各边的位置
        /// </summary>
        /// <param name="p1">四边形顶点1</param>
        /// <param name="p2">四边形顶点2</param>
        /// <param name="p3">四边形顶点3</param>
        /// <param name="p4">四边形顶点4</param>
        /// <param name="p">待测点</param>
        /// <returns>true：点在四边形内；false：点在四边形外</returns>
        public static bool IsPointInsideRectangle(PositionInt p1, PositionInt p2, PositionInt p3, PositionInt p4, PositionInt p)
        {
            double cross1 = Tools.CrossProduct(p1, p2, p);
            double cross2 = Tools.CrossProduct(p2, p3, p);
            double cross3 = Tools.CrossProduct(p3, p4, p);
            double cross4 = Tools.CrossProduct(p4, p1, p);

            return (cross1 >= 0 && cross2 >= 0 && cross3 >= 0 && cross4 >= 0) || (cross1 <= 0 && cross2 <= 0 && cross3 <= 0 && cross4 <= 0);
        }

        /// <summary>
        /// 计算有向最小包围盒（OBB）
        /// 使用协方差矩阵计算点集的主方向
        /// 然后在主方向坐标系中计算 AABB，最后转换回原坐标系
        /// </summary>
        /// <param name="CH">凸包上的点列表</param>
        /// <returns>OBB 的四个角点列表</returns>
        public static List<PositionInt> GetOBB(List<PositionInt> CH)
        {
            if (CH.Count == 0)
            {
                return CH;
            }
            // xy 空间中，十个数据点
            double[,] coord_xy = new double[2, CH.Count];
            for (int i = 0; i < CH.Count; i++)
            {
                coord_xy[0, i] = CH[i].x;
                coord_xy[1, i] = CH[i].y;
            }
            Matrix<double> coord_xy_m = Matrix<double>.Build.DenseOfArray(coord_xy);
            // 求协方差矩阵
            var covMatrix = Tools.CalculateCovarianceMatrix(coord_xy_m.Transpose());
            // 求协方差矩阵的特征向量
            var eigVector = covMatrix.Evd().EigenVectors.Multiply(Matrix<double>.Build.DenseOfArray(new double[2, 2] { { 0, -1 }, { 1, 0 } }));
            // 将数据点从 xy 空间转到 uv 空间
            var coord_uv = eigVector.Transpose().Multiply(coord_xy_m);
            // 求 uv 空间的 AABB，依次保存：左下角、左上角、右下角、右上角
            double uMin = coord_uv.Row(0).Min();
            double uMax = coord_uv.Row(0).Max();
            double vMin = coord_uv.Row(1).Min();
            double vMax = coord_uv.Row(1).Max();
            Matrix<double> AABB_uv = Matrix<double>.Build.DenseOfArray(new double[2, 4] {
            {uMin,uMin,uMax,uMax},{vMin,vMax,vMin,vMax}
        });
            // uv 空间的 AABB 转回 xy 空间，即得到 OBB
            var OBB_xy = eigVector.Multiply(AABB_uv);
            List<PositionInt> res = new List<PositionInt>
        {
            new PositionInt((int)OBB_xy[0, 0], (int)OBB_xy[1, 0]),
            new PositionInt((int)OBB_xy[0, 1], (int)OBB_xy[1, 1]),
            new PositionInt((int)OBB_xy[0, 2], (int)OBB_xy[1, 2]),
            new PositionInt((int)OBB_xy[0, 3], (int)OBB_xy[1, 3])
        };


            return res;
        }

        /// <summary>
        /// 计算有向包围盒内的所有像素点
        /// 先计算 OBB，然后在 OBB 内遍历所有像素
        /// </summary>
        /// <param name="layerSize">图层尺寸</param>
        /// <param name="Image_colors">像素颜色数组</param>
        /// <returns>OBB 内的所有像素坐标列表</returns>
        public static List<PositionInt> GetOBBPixels(Size layerSize, Color32[] Image_colors)
        {
            var arclist = GetOBB(GetConvexHull(GetHollow(layerSize, Image_colors)));
            if (arclist.Count != 0)
            {
                for (int i = 0; i < layerSize.width; i++)
                {
                    for (int j = 0; j < layerSize.height; j++)
                    {
                        if (IsPointInsideRectangle(arclist[0], arclist[1], arclist[3], arclist[2], new PositionInt(i, j)))
                        {
                            arclist.Add(new PositionInt(i, j));
                        }
                    }
                }
            }

            return arclist;
        }

        /// <summary>
        /// 获取圆弧数据
        /// 根据拟合的圆和细化后的像素点，计算圆弧的起终点
        /// 通过比较各点到圆心的角度，确定弧的起始和结束位置
        /// </summary>
        /// <param name="circle">拟合的圆数据</param>
        /// <param name="positionInts">细化后的像素点列表</param>
        /// <returns>圆弧数据（包含圆心、半径、起点、终点）</returns>
        public static Arc GetArcData(Circle circle, List<PositionInt> positionInts)
        {
            Vector2 point = new Vector2(circle.X, circle.Y);
            List<float> angleList = new List<float>();
            Vector2 Apoint = new Vector2(positionInts[0].x, positionInts[0].y) - point;
            for (int i = 0; i < positionInts.Count; i++)
            {
                Vector2 Cpoint = new Vector2(positionInts[i].x, positionInts[i].y) - point;
                int dir = Vector3.Cross(Apoint, Cpoint).z > 0 ? 1 : -1;
                float Angle = Vector2.Angle(Apoint.normalized, Cpoint.normalized);
                Angle = dir == -1 ? -Angle : Angle;
                angleList.Add(Angle);
            }
            float maxAngle = angleList[0];
            float minAngle = angleList[0];
            int maxNum = 0;
            int minNum = 0;
            for (int i = 0; i < angleList.Count; i++)
            {
                if (angleList[i] > maxAngle && maxNum >= 0)
                {
                    maxAngle = angleList[i];
                    maxNum = i;
                }
                if (angleList[i] < minAngle && minNum >= 0)
                {
                    minAngle = angleList[i];
                    minNum = i;
                }
            }
            Arc arc = new Arc();
            arc.X = circle.X;
            arc.Y = circle.Y;
            arc.Radius = circle.Radius;
            arc.Start_X = positionInts[minNum].x;
            arc.Start_Y = positionInts[minNum].y;
            arc.End_X = positionInts[maxNum].x;
            arc.End_Y = positionInts[maxNum].y;
            return arc;
        }

    }
}
