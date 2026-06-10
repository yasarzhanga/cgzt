using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace jxzt
{
    /// <summary>
    /// 画笔类 - 定义绘图用的笔刷数据
    /// </summary>
    public class Brush
    {
        public Vector2[] brush_data;  // 笔刷形状数据（相对于中心的像素偏移数组）
        public Color32 color;         // 笔刷颜色（包含Alpha通道）
    }

    /// <summary>
    /// 尺寸结构体 - 存储宽度和高度
    /// </summary>
    public class Size
    {
        public int width;   // 宽度（像素）
        public int height;  // 高度（像素）

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    /// <summary>
    /// 画笔类型枚举 - 定义当前使用的工具类型
    /// </summary>
    public enum brushType
    {
        paint,   // 绘画模式（用颜色绘制）
        eraser,  // 擦除模式（用透明色擦除）
        none     // 无操作模式
    }

    /// <summary>
    /// 整数坐标类 - 表示画布上的像素坐标
    /// 类似 Vector2Int，但重载了乘法和加法运算符，方便进行坐标运算
    /// </summary>
    public class PositionInt
    {
        public int x;  // 横坐标（像素）
        public int y;  // 纵坐标（像素）

        // 常用方向常量（只读）
        public static readonly PositionInt one = new PositionInt(1, 1);    // 右下方向
        public static readonly PositionInt left = new PositionInt(-1, 0);  // 左方向
        public static readonly PositionInt right = new PositionInt(1, 0); // 右方向
        public static readonly PositionInt up = new PositionInt(0, 1);    // 上方向
        public static readonly PositionInt down = new PositionInt(0, -1); // 下方向

        /// <summary>
        /// 构造函数 - 初始化坐标
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        public PositionInt(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 运算符重载 - 整数乘以坐标（标量乘法）
        /// 例如：2 * new PositionInt(3, 4) = new PositionInt(6, 8)
        /// </summary>
        /// <param name="a">整数标量</param>
        /// <param name="b">坐标</param>
        /// <returns>新的坐标（各分量乘以标量）</returns>
        public static PositionInt operator *(int a, PositionInt b)
        {
            return new PositionInt(a * b.x, a * b.y);
        }

        /// <summary>
        /// 运算符重载 - 坐标相加（向量加法）
        /// 例如：new PositionInt(1, 2) + new PositionInt(3, 4) = new PositionInt(4, 6)
        /// </summary>
        /// <param name="a">第一个坐标</param>
        /// <param name="b">第二个坐标</param>
        /// <returns>新的坐标（各分量相加）</returns>
        public static PositionInt operator +(PositionInt a, PositionInt b)
        {
            return new PositionInt(a.x + b.x, a.y + b.y);
        }

        /// <summary>
        /// 重写 Equals 方法 - 判断两个坐标是否相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>true：坐标相同；false：不同或类型不匹配</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is PositionInt)) return false;

            PositionInt p = (PositionInt)obj;
            return x == p.x & y == p.y;
        }

        /// <summary>
        /// 重写 GetHashCode 方法 - 生成哈希值（用于 Dictionary 等容器）
        /// </summary>
        /// <returns>哈希值（x 和 y 的异或结果）</returns>
        public override int GetHashCode()
        {
            return x ^ y;
        }
    }
    /// <summary>
    /// 图层数据类 - 存储单个绘图图层的所有信息
    /// 每个图层对应一条图线或一个标注，包含图线类型、识别结果、评分等信息
    /// </summary>
    public class LayerData
    {
        public int layerNum;                    // 图层编号（唯一标识）
        public string layerError;              // 图层错误信息（如有）
        public string lineType;                // 线型（实线、虚线、点画线等）
        public string lineshape;               // 图线形状（直线、圆、圆弧、椭圆等）
        public string biaoshi;                 // 标识类型（尺寸、粗糙度、弧度等）
        public string OCRString;               // OCR识别的字符串结果
        public string ocr;                    // OCR原始结果
        public int score;                     // 该图层得分（用于自动评分）
        public PositionInt[] data;            // 图线像素坐标数组（绘制的所有点）
        public PositionInt[] frameSelectData;  // 框选区域的数据（用于手动选择）
        public Brush brush;                    // 使用的笔刷（颜色和形状）
        public float xuhaoPosX;               // 序号标签的X坐标（用于UI显示）
        public float xuhaoPosY;               // 序号标签的Y坐标（用于UI显示）
        public Circle circleData = new();      // 圆的拟合数据（圆心、半径）
        public Arc arcData = new();           // 圆弧的拟合数据（圆心、半径、起终点）
        public Ellipse ellipseData;            // 椭圆的拟合数据（中心、长轴、短轴、倾角）
        public List<PositionInt> JudgmentZoneData;  // 判分区域数据（用于定义评分范围）
        public List<List<PositionInt>> markData = new List<List<PositionInt>>();      // 标记数据（多个标记区域）
        public List<List<PositionInt>> qrCodeData = new List<List<PositionInt>>();   // 二维码数据（多个二维码区域）
        public List<List<PositionInt>> OCRData = new List<List<PositionInt>>();     // OCR识别区域数据（多个文字区域）

    }
    /// <summary>
    /// 答案文件类 - 存储整个答案的所有图层数据
    /// 用于保存和加载学生答案或教师答案
    /// </summary>
    public class AnswerFile
    {
        public List<LayerData> data;  // 所有图层的数据列表（每个图层对应一条图线或一个标注）
        public int width;              // 画布宽度（像素）
        public int height;             // 画布高度（像素）

        /// <summary>
        /// 构造函数 - 初始化图层数据列表
        /// </summary>
        public AnswerFile()
        {
            data = new List<LayerData>();
        }

        /// <summary>
        /// 获取所有图层的数据（转换为 Color32 数组格式）
        /// 每个图层生成一个二维像素数组，绘制的像素为黑色，其余为透明
        /// </summary>
        /// <returns>Color32 数组列表，每个元素对应一个图层的像素数据</returns>
        public List<Color32[]> GetData()
        {
            Color32 black = new Color32(0, 0, 0, 255);  // 黑色（不透明）
            List<Color32[]> color32s = new List<Color32[]>();
            for (int i = 0; i < data.Count; i++)
            {
                Color32[] tmp = new Color32[width * height];  // 初始化像素数组
                foreach (var item in data[i].data)
                {
                    // 将绘制的像素点设为黑色
                    tmp[item.x + item.y * width] = black;
                }
                color32s.Add(tmp);
            }
            return color32s;
        }
    }

    /// <summary>
    /// 带索引的颜色数据类 - 封装二维颜色数组，提供方便的索引访问
    /// 支持 ROI（感兴趣区域）设置，可只对局部区域进行操作
    /// </summary>
    public class ColorsWithIndex
    {
        public Color32[] data;  // 一维颜色数组（按行优先存储：index = y * width + x）
        public int width;       // 画布宽度（像素）
        public int height;      // 画布高度（像素）

        // ROI（感兴趣区域）相关变量
        int roiWidth = 0;   // ROI 宽度
        int roiHeight = 0;  // ROI 高度
        int roix = 0;       // ROI 左上角 X 坐标
        int roiy = 0;       // ROI 左上角 Y 坐标
        bool hasROI = false; // 是否启用了 ROI

        /// <summary>
        /// 设置 ROI（感兴趣区域）- 后续操作只针对该区域
        /// </summary>
        /// <param name="position">ROI 左上角坐标</param>
        /// <param name="size">ROI 尺寸</param>
        public void SetRoI(Vector2Int position, Vector2Int size)
        {
            roiWidth = size.x;
            roiHeight = size.y;
            roix = position.x;
            roiy = position.y;
            hasROI = true;
        }

        /// <summary>
        /// 清除 ROI - 恢复为操作整个画布
        /// </summary>
        public void ClearRoI()
        {
            roix = 0;
            roiy = 0;
            roiWidth = 0;
            roiHeight = 0;
            hasROI = false;
        }

        /// <summary>
        /// 构造函数 - 创建指定大小的空画布（所有像素透明）
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public ColorsWithIndex(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new Color32[width * height];
            Array.Fill(this.data, new Color32(0, 0, 0, 0));  // 初始化为透明
        }

        /// <summary>
        /// 构造函数 - 从已有数据创建画布（数据量足够时克隆）
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="data">颜色数据数组</param>
        public ColorsWithIndex(int width, int height, Color32[] data)
        {
            this.width = width;
            this.height = height;
            if (data.Length >= width * height)
            {
                this.data = (Color32[])data.Clone();  // 深拷贝数据
            }
        }

        public Color32 this[int i, int j]
        {
            get
            {
                try
                {
                    if (i < 0 || i >= width || j < 0 || j >= height)
                        throw new System.Exception($"w:{i},h:{j}---index invalid");
                    return data[j * width + i];  // 行优先存储：index = y * width + x
               
                }
                catch (System.Exception)
                {
                    throw;  // 重新抛出异常，由调用者处理
                }

            }

            set
            {
                try
                {
                    data[j * width + i] = value;  // 设置指定坐标的颜色值
                }
                catch (System.Exception)
                {
                    throw;  // 重新抛出异常，由调用者处理
                }            
            }
        }

        /// <summary>
        /// 获取所有非透明像素的坐标和颜色（字典格式）
        /// </summary>
        /// <returns>坐标-颜色字典（只包含 Alpha > 0 的像素）</returns>
        public Dictionary<PositionInt,Color32> GetPositionIntWithColor()
        {
            Dictionary<PositionInt,Color32> pairs = new Dictionary<PositionInt,Color32>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (this[i, j].a > 0)  // 只收集非透明像素
                    {
                        pairs.Add(new PositionInt(i, j), this[i,j]);
                    }
                }
            }
            return pairs;
        }

        /// <summary>
        /// 获取所有非透明像素的坐标（列表格式）
        /// </summary>
        /// <returns>坐标列表（只包含 Alpha > 0 的像素）</returns>
        public List<PositionInt> GetPositionInts()
        {
            List<PositionInt> positions = new List<PositionInt>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (this[i, j].a > 0)  // 只收集非透明像素
                    {
                        positions.Add(new PositionInt(i, j));
                    }
                }
            }
            return positions;
        }

        /// <summary>
        /// 从字典数据设置像素颜色（会先清空画布）
        /// </summary>
        /// <param name="data">坐标-颜色字典</param>
        public void SetPositionIntWithColor(Dictionary<PositionInt, Color32> data)
        {
            Array.Fill(this.data, new Color32(0, 0, 0, 0));  // 清空画布
            foreach (var item in data)
            {
                // 边界检查，防止越界
                if (item.Key.x >= 0 && item.Key.x < width && item.Key.y >= 0 && item.Key.y < height)
                {
                    this[item.Key.x, item.Key.y] = item.Value;
                }
            }
        }

        /// <summary>
        /// 移动所有像素（平移变换）
        /// </summary>
        /// <param name="x">X 方向平移量</param>
        /// <param name="y">Y 方向平移量</param>
        public void Move(int x, int y)
        {
            Dictionary<PositionInt,Color32> pairs = GetPositionIntWithColor();  // 获取所有像素
            pairs.All((e) =>
            {
                e.Key.x += x;  // X 方向平移
                e.Key.y += y;  // Y 方向平移
                return true;
            });
            SetPositionIntWithColor(pairs);  // 写回数据
        }
    }

    /// <summary>
    /// 线型枚举 - 工程制图中图线的类型
    /// </summary>
    public enum linetype
    {
        实线,       // 连续实线（如轮廓线）
        虚线,       // 断续虚线（如隐藏线）
        点画线,     // 长划加点（如中心线）
        双点画线,   // 长划加两点（如假想线）
        unknown     // 未知类型
    }

    /// <summary>
    /// 图线形状枚举 - 绘制的几何形状类型
    /// </summary>
    public enum lineshape
    {
        直线,       // 直线段
        圆,         // 圆
        圆弧,       // 圆弧（部分圆）
        椭圆,       // 椭圆
        标识,       // 标识符号（尺寸、粗糙度等）
        二维码,     // 二维码
        文字识别,   // 文字识别区域
        判分区域,   // 判分区域（用于自动评分）
        unknown     // 未知形状
    }

    /// <summary>
    /// 标识类型枚举 - 工程图标注标识的类型
    /// </summary>
    public enum 标识
    {
        unknown,        // 未知类型
        尺寸 = 1,      // 尺寸标注
        粗糙度 = 2,     // 表面粗糙度
        弧度 = 3,       // 弧度标注
        类粗糙度 = 4,    // 类似粗糙度的标注
        直径 = 5,       // 直径标注
        半径 = 6        // 半径标注
    }

    /// <summary>
    /// 文字识别类型枚举 - OCR 识别的文字类型
    /// </summary>
    public enum 文字识别
    {
        姓名,       // 学生姓名
        学号,       // 学生学号
        自定义,     // 自定义文字
        unknown     // 未知类型
    }
    /// <summary>
    /// 错误原因枚举 - 自动评分时返回的错误类型
    /// </summary>
    public enum ErrorReson
    {
        正确,                       // 正确（无错误）
        线型使用错误,               // 线型使用错误（如实线误用虚线）
        图线不在或偏离正确位置,     // 图线位置错误（偏离标准位置）
        图线过长,                   // 图线长度过长
        图线过短,                   // 图线长度过短
        剖面线方向绘制错误,         // 剖面线方向错误
        剖面线不是45度线,           // 剖面线角度错误（应为45度）
        剖面线间距大小不一,         // 剖面线间距不均匀
        在标注位置未发现对应尺寸,   // 漏标尺寸
        尺寸绘制不标准格式错误,     // 尺寸标注格式错误
        尺寸符号错误,               // 尺寸符号错误（如直径符号错误）
        尺寸数值错误,               // 尺寸数值错误
        尺寸标注数值位置不对,       // 尺寸数值位置错误
        标注了多余尺寸,             // 多标尺寸（不应标注的标注了）
        公差符号错用,               // 公差符号错误（如 H7 误用为 h7）
        未标注公差,                 // 漏标公差
        公差数字错误,               // 公差数值错误
        公差格式错误,               // 公差格式错误
        基准要素标识位置错误,       // 基准要素位置错误
        基准要素标识未标识,         // 漏标基准要素
        基准要素符号错误,           // 基准符号错误
        剖面图标识未注写,           // 漏写剖面图标识
        未填写技术要求,             // 漏填技术要求
        二维码或姓名学号,           // 二维码或姓名学号错误
        回答错误                    // 问答题回答错误
    }
    /// <summary>
    /// 圆数据类 - 存储圆的拟合结果
    /// </summary>
    public class Circle
    {
        public float X { get; set; }      // 圆心 X 坐标
        public float Y { get; set; }      // 圆心 Y 坐标
        public float Radius { get; set; }  // 圆半径
    }

    /// <summary>
    /// 圆弧数据类 - 存储圆弧的拟合结果（包含起终点）
    /// </summary>
    public class Arc
    {
        public float X { get; set; }       // 圆心 X 坐标
        public float Y { get; set; }       // 圆心 Y 坐标
        public float Radius { get; set; }   // 圆弧半径
        public float Start_X { get; set; }  // 起点 X 坐标
        public float Start_Y { get; set; }  // 起点 Y 坐标
        public float End_X { get; set; }    // 终点 X 坐标
        public float End_Y { get; set; }    // 终点 Y 坐标
    }

    /// <summary>
    /// 圆弧数据类（扩展版）- 用于绘制和判定圆弧
    /// 包含绘制方向、整圆判定等详细信息
    /// </summary>
    public class Arc1
    {
        public float radis;           // 半径
        public int center_x;          // 圆心 X 坐标（整数）
        public int center_y;          // 圆心 Y 坐标（整数）
        public Vector2 start_edge;    // 起点坐标
        public Vector2 end_edge;      // 终点坐标

        public bool superior_arc;     // 是否为优弧（大于180度的弧）
        public bool mask;             // 遮罩标志（用于绘制）
        public float linewidth;       // 线宽（用于判定点是否在弧上）
        public int dir1;              // 画线方向（0：无法判断，1：顺时针，-1：逆时针）
        public int isCircle;          // 整圆判定（0：准备画圆，1：半圆，2：整圆，4：整圆已完成）

        /// <summary>
        /// 构造函数 - 完整初始化圆弧数据
        /// </summary>
        public Arc1(float radis, int center_x, int center_y, Vector2 start_edge, Vector2 end_edge, bool superior_arc, bool mask, float linewidth, int dir1, int isCircle)
        {
            this.radis = radis;
            this.center_x = center_x;
            this.center_y = center_y;
            this.start_edge = start_edge;
            this.end_edge = end_edge;
            this.superior_arc = superior_arc;
            this.mask = mask;
            this.linewidth = linewidth;
            this.dir1 = dir1;
            this.isCircle = isCircle;
        }

        /// <summary>
        /// 构造函数 - 从起点和终点快速创建圆弧（简化版）
        /// 圆心取起点坐标，半径取两点距离
        /// </summary>
        /// <param name="start">起点坐标</param>
        /// <param name="end">终点坐标</param>
        public Arc1(Vector2 start, Vector2 end)
        {
            this.center_x = (int)start.x;  // 圆心 X 取起点 X（简化计算）
            this.center_y = (int)start.y;  // 圆心 Y 取起点 Y（简化计算）
            this.radis = Mathf.Sqrt(Mathf.Pow(start.x - end.x, 2) + Mathf.Pow(start.y - end.y, 2));  // 半径 = 两点距离
        }

        public bool FindPointCircle(float linewidth, int x, int y, Vector2 orignPoint, Vector2 Apoint, float Angle1, int dir1, float cos1, int isCircle)
        {
            float distan = Mathf.Sqrt(Mathf.Pow(x - center_x, 2) + Mathf.Pow(y - center_y, 2)) - radis;
            if (distan < linewidth && distan > 0)
            {

                Vector2 Cpoint = new Vector2(x, y) - orignPoint;
                float Angle2 = Vector2.Angle(Apoint.normalized, Cpoint.normalized);
                float dot2 = Vector2.Dot(Apoint.normalized, Cpoint.normalized);
                float cos2 = Vector3.Cross(Apoint.normalized, Cpoint.normalized).z < 0 ? -1 : 1;
                if (dir1 == -1)//画线方向逆时针
                {
                    if (isCircle == 4)
                    {
                        if (cos2 > 0)
                        {
                            if (Angle2 < 180)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (cos2 < 0)
                        {
                            Angle2 = 360 - Angle2;
                            if (360 > Angle2 && Angle2 > 180)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else if (cos1 > 0 && cos2 > 0)//0-180
                    {
                        if (Angle1 > Angle2)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (cos1 < 0)  //180-360              
                    {
                        Angle1 = 360 - Angle1;
                        if (cos2 > 0)
                        {
                            if (Angle2 < 180)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (cos2 < 0)
                        {
                            Angle2 = 360 - Angle2;
                            if (Angle1 > Angle2 && Angle2 > 180)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }


                    }
                }
                else if (dir1 == 1)//画线方向顺时针
                {
                    if (isCircle == 4)
                    {
                        if (cos2 < 0)
                        {
                            if (Angle2 < 180)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (cos2 > 0)
                        {
                            Angle2 = 360 - Angle2;
                            if (360 > Angle2 && Angle2 > 180)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else if (cos1 < 0 && cos2 < 0)//0-180
                    {
                        if (Angle1 > Angle2)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    else if (cos1 > 0)//180-360
                    {
                        Angle1 = 360 - Angle1;
                        if (cos2 < 0)
                        {
                            if (Angle2 < 180)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (cos2 > 0)
                        {
                            Angle2 = 360 - Angle2;
                            if (Angle1 > Angle2 && Angle2 > 180)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }

                    }
                }

            }
            return false;
        }
        /// <summary>
        /// 判断学生圆弧是否与标准圆弧重合（用于自动评分）
        /// </summary>
        /// <param name="standardRadis">标准圆弧半径</param>
        /// <param name="radis">学生圆弧半径</param>
        /// <param name="standardCenter_x">标准圆心 X</param>
        /// <param name="standardCenter_y">标准圆心 Y</param>
        /// <param name="center_x">学生圆心 X</param>
        /// <param name="center_y">学生圆心 Y</param>
        /// <param name="standardStart_edge">标准起点</param>
        /// <param name="standardEnd_edge">标准终点</param>
        /// <param name="start_edge">学生起点</param>
        /// <param name="end_edge">学生终点</param>
        /// <param name="standardDir1">标准画线方向</param>
        /// <param name="dir1">学生画线方向</param>
        /// <param name="isStandardCircle">标准是否整圆</param>
        /// <param name="isCircle">学生是否整圆</param>
        /// <returns>true：圆弧重合（在容差范围内）；false：不重合</returns>
        public bool IsCoincideStandardArc(float standardRadis, float radis, int standardCenter_x, int standardCenter_y, int center_x, int center_y, Vector2 standardStart_edge, Vector2 standardEnd_edge, Vector2 start_edge, Vector2 end_edge, int standardDir1, int dir1, int isStandardCircle, int isCircle)
        {
            Vector2 standardCenter = new Vector2(standardCenter_x, standardCenter_y);
            Vector2 center = new Vector2(center_x, center_y);

            // 如果是整圆，只比较圆心和半径
            if (isStandardCircle == 4 && isCircle == 4)
            {
                if (Mathf.Abs(standardRadis - radis) < 11f && Vector2.Distance(standardCenter, center) < 11f) return true;
            }

            // 如果画线方向相同，比较相同起终点
            if (standardDir1 == dir1)
            {
                float dis1 = Vector2.Distance(standardCenter, center);
                float dis2 = Vector2.Distance(standardStart_edge, start_edge);
                float dis3 = Vector2.Distance(standardEnd_edge, end_edge);
                // 容差范围内视为重合
                if (Mathf.Abs(standardRadis - radis) < 11f && Vector2.Distance(standardCenter, center) < 50f 
                    && Vector2.Distance(standardStart_edge, start_edge) < 50f && Vector2.Distance(standardEnd_edge, end_edge) < 50f) 
                    return true;
            }
            else  // 如果画线方向相反，比较相反起终点
            {
                //如果是相反的起终点
                if (Mathf.Abs(standardRadis - radis) < 11f && Vector2.Distance(standardCenter, center) < 50f 
                    && Vector2.Distance(standardStart_edge, end_edge) < 50f && Vector2.Distance(standardEnd_edge, start_edge) < 50f) 
                    return true;
            }
            return false;  // 不重合
        }
    }
    /// <summary>
    /// 椭圆数据类 - 存储椭圆的拟合结果
    /// </summary>
    public class Ellipse
    {
        /// <summary>
        /// 构造函数 - 初始化椭圆数据
        /// </summary>
        /// <param name="x">中心 X 坐标</param>
        /// <param name="y">中心 Y 坐标</param>
        /// <param name="a">长轴长度</param>
        /// <param name="b">短轴长度</param>
        /// <param name="radius">倾角（弧度或角度）</param>
        public Ellipse(float x, float y, float a, float b, float radius)
        {
            X = x;
            Y = y;
            A = a;
            B = b;
            Theta = radius;
        }
        /// <summary>
        /// 中心横坐标
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// 中心纵坐标
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// 长轴长度
        /// </summary>
        public float A { get; set; }
        /// <summary>
        /// 短轴长度
        /// </summary>
        public float B { get; set; }
        /// <summary>
        /// 倾角（椭圆旋转角度）
        /// </summary>
        public float Theta { get; set; }
    }
    
}

