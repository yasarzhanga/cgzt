using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace jxzt
{
    /// <summary>
    /// Tools - 绘图工具类
    /// 提供笔刷创建、数据处理、数学计算等辅助功能
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// 创建圆形笔刷
        /// 根据指定半径生成实心圆的笔刷数据
        /// </summary>
        /// <param name="radius">笔刷半径（像素）</param>
        /// <returns>Brush 对象，包含笔刷的形状数据和颜色</returns>
        public static Brush CreateCircleBrush(int radius)
        {
            Brush brush = new Brush();
            List<Vector2> points = new List<Vector2>();
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if (i * i + j * j < radius * radius)
                    {
                        points.Add(new Vector2(i, j));
                    }
                }
            }
            brush.brush_data = points.ToArray();
            return brush;
        }

        /// <summary>
        /// 数组去重
        /// 使用 HashSet 去除数组中的重复元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="ele">输入数组</param>
        /// <returns>去重后的数组</returns>
        public static T[] ArrayRemoveRepeatElement<T>(T[] ele)
        {
            HashSet<T> set = new HashSet<T>();

            foreach (var e in ele)
            {
                try
                {
                    set.Add(e);
                }
                catch (System.Exception)
                {

                    throw;
                }

            }
            return set.ToArray();
        }

        /// <summary>
        /// 列表去重
        /// 使用 HashSet 去除列表中的重复元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="ele">输入列表</param>
        /// <returns>去重后的列表</returns>
        public static List<T> ListRemoveRepeatElement<T>(List<T> ele)
        {
            HashSet<T> set = new HashSet<T>();
            foreach (var e in ele)
            {
                try
                {
                    set.Add(e);
                }
                catch (System.Exception)
                {

                    throw;
                }

            }
            return set.ToList();
        }

        /// <summary>
        /// 计算叉积（向量积）
        /// 用于判断三个点的相对位置关系
        /// </summary>
        /// <param name="a">第一个点（向量起点）</param>
        /// <param name="b">第二个点（向量中间点）</param>
        /// <param name="c">第三个点（向量终点）</param>
        /// <returns>叉积结果（>0：逆时针，<0：顺时针，=0：共线）</returns>
        public static float CrossProduct(PositionInt a, PositionInt b, PositionInt c)
        {
            return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        }

        /// <summary>
        /// 计算协方差矩阵
        /// 用于计算点集的协方差矩阵，用于后续的特征值和特征向量计算
        /// </summary>
        /// <param name="data">输入数据矩阵（每列代表一个维度，每行代表一个样本）</param>
        /// <returns>协方差矩阵</returns>
        public static Matrix<double> CalculateCovarianceMatrix(Matrix<double> data)
        {
            int rows = data.RowCount;
            int cols = data.ColumnCount;

            // 计算每一列的均值
            double[] means = new double[cols];
            for (int i = 0; i < cols; i++)
            {
                double sum = 0;
                for (int j = 0; j < rows; j++)
                {
                    sum += data[j, i];
                }
                means[i] = sum / rows;
            }

            // 计算协方差矩阵
            Matrix<double> covarianceMatrix = Matrix<double>.Build.Dense(cols, cols);
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < rows; k++)
                    {
                        sum += (data[k, i] - means[i]) * (data[k, j] - means[j]);
                    }
                    covarianceMatrix[i, j] = sum / (rows - 1);
                }
            }

            return covarianceMatrix;
        }

        /// <summary>
        /// 图像膨胀操作（单次）
        /// 将点集向外扩展一个像素（上下左右四个方向）
        /// </summary>
        /// <param name="src">输入点集</param>
        /// <returns>膨胀后的点集</returns>
        public static List<PositionInt> dilation(List<PositionInt> src)
        {
            HashSet<PositionInt> res = new HashSet<PositionInt>(src.ToArray());
            foreach (var item in src)
            {
                res.Add(item + PositionInt.left);
                res.Add(item + PositionInt.right);
                res.Add(item + PositionInt.up);
                res.Add(item + PositionInt.down);
            }
            return res.ToList();
        }

        /// <summary>
        /// 图像膨胀操作（多次迭代）
        /// 将点集向外扩展 50 次迭代，用于大面积膨胀
        /// </summary>
        /// <param name="src">输入点集</param>
        /// <returns>膨胀后的点集</returns>
        public static List<PositionInt> dilation(PositionInt[] src)
        {
            HashSet<PositionInt> res = new HashSet<PositionInt>(src);
            for (int i = 0; i < 50; i++)
            {
                foreach (var item in src)
                {
                    res.Add(item + PositionInt.left);
                    res.Add(item + PositionInt.right);
                    res.Add(item + PositionInt.up);
                    res.Add(item + PositionInt.down);
                }
            }

            return res.ToList();
        }
    }


/// <summary>
/// ComputeBuffer 追踪器
/// 用于统一管理和释放所有创建的 ComputeBuffer，避免内存泄漏
/// </summary>
public class ComputeBufferTracker
    {
        /// <summary>存储所有创建的 ComputeBuffer 引用</summary>
        private static List<ComputeBuffer> _buffers = new List<ComputeBuffer>();

        /// <summary>
        /// 创建并跟踪 ComputeBuffer
        /// </summary>
        /// <param name="count">缓冲区元素数量</param>
        /// <param name="stride">每个元素的字节大小</param>
        /// <returns>创建的 ComputeBuffer</returns>
        public static ComputeBuffer Create(int count, int stride)
        {
            ComputeBuffer buffer = new ComputeBuffer(count, stride);
            _buffers.Add(buffer);
            return buffer;
        }

        /// <summary>
        /// 释放所有追踪的 ComputeBuffer
        /// 应在场景切换或程序退出时调用
        /// </summary>
        public static void ReleaseAll()
        {
            foreach (var buffer in _buffers)
            {
                if (buffer != null)
                {
                    buffer.Dispose();
                }
            }
            _buffers.Clear();
        }
    }
   
}

