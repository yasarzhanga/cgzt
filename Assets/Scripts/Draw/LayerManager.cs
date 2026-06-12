using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Specialized;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace jxzt
{
    /// <summary>
    /// LayerManager - 图层管理器
    /// 负责管理单个绘图图层，支持指针事件处理、绘图操作、图线类型识别等
    /// 实现 IPointerDownHandler、IPointerMoveHandler、IPointerUpHandler 接口以支持触控/鼠标绘图
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class LayerManager : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
    {
        // ========== 静态成员 ==========
        /// <summary>所有图层管理器的列表（静态集合）</summary>
        static List<LayerManager> layerManagers = new List<LayerManager>();
        /// <summary>获取所有图层管理器</summary>
        public static List<LayerManager> LayerManagers { get => layerManagers; }

        // ========== 图层基础数据 ==========
        /// <summary>图层尺寸（宽度和高度）</summary>
        private Size layerSize;
        /// <summary>图层关联的 RawImage 组件</summary>
        RawImage Layer_image;
        /// <summary>图层纹理（Texture2D）</summary>
        private Texture2D texture2D;
        /// <summary>局部纹理刷新复用缓冲，避免批量判分时重复分配临时数组</summary>
        private Color32[] textureRegionBuffer;
        /// <summary>图层像素颜色数组（一维，按行优先存储）</summary>
        private Color32[] image_colors;
        /// <summary>点击状态（是否正在绘制）</summary>
        public bool click_state;

        // ========== 颜色常量 ==========
        /// <summary>透明色常量</summary>
        Color32 None = new(0, 0, 0, 0);
        /// <summary>激活状态颜色（蓝色）</summary>
        public Color32 ActiveColor = new(255, 0, 0, 255 / 2);  // 激活图层显示的颜色
        /// <summary>非激活状态颜色（黑色）</summary>
        Color32 DisActiveColor = new(255, 0, 0, 255 / 2);

        // ========== 绘图状态 ==========
        /// <summary>上一个指针位置（用于计算绘制轨迹）</summary>
        private Vector2 last_position;
        /// <summary>图层是否处于激活状态</summary>
        public bool active;

        // ========== 图层元数据 ==========
        /// <summary>图层错误信息（评分时记录）</summary>
        public string layerError;
        /// <summary>OCR 原始识别结果</summary>
        public string ocr;
        /// <summary>OCR 处理后的字符串</summary>
        public string OCRString;
        /// <summary>图层编号（唯一标识）</summary>
        public int layerNum;
        /// <summary>图层得分（自动评分时使用）</summary>
        public int score;
        /// <summary>图线类型（实线、虚线、点画线等）</summary>
        public linetype lineType;
        /// <summary>序号标签 X 坐标（UI 显示用）</summary>
        public float xuhaoPosX;
        /// <summary>序号标签 Y 坐标（UI 显示用）</summary>
        public float xuhaoPosY;
        /// <summary>错误显示位置</summary>
        public Vector2 displayError_position;
        /// <summary>是否正确（评分结果）</summary>
        public bool isRight = false;
        /// <summary>图线形状（直线、圆、圆弧、椭圆等）</summary>
        public lineshape lineshape;

        // ========== 拟合数据 ==========
        /// <summary>圆的拟合数据（圆心、半径）</summary>
        public Circle circleData = new Circle();
        /// <summary>圆弧的拟合数据</summary>
        public Arc arcData = new Arc();
        /// <summary>椭圆的拟合数据</summary>
        public Ellipse ellipseData;

        // ========== 绘图数据 ==========
        /// <summary>当前使用的笔刷</summary>
        public Brush brush;
        /// <summary>图层像素坐标数组（所有绘制点的坐标）</summary>
        public PositionInt[] data;
        /// <summary>框选区域的数据</summary>
        public PositionInt[] frameSelectData;
        /// <summary>判分区域数据</summary>
        public List<PositionInt> JudgmentZoneData;
        /// <summary>标记数据列表（多个标记区域）</summary>
        public List<List<PositionInt>> markData = new List<List<PositionInt>>();
        /// <summary>二维码数据列表</summary>
        public List<List<PositionInt>> qrCodeData = new List<List<PositionInt>>();
        /// <summary>OCR 识别区域数据列表</summary>
        public List<List<PositionInt>> OCRData = new List<List<PositionInt>>();
        /// <summary>标识类型（尺寸、粗糙度等）</summary>
        public 标识 biaoshi;
        /// <summary>标识字符串（备用字段）</summary>
        public string biaoshis;
        public Size LayerSize
        {
            get => layerSize;
            set
            {
                if (texture2D != null)
                {
                    Destroy(texture2D);
                    texture2D = null;
                }

                layerSize = value;
                image_colors = new Color32[value.width * value.height];
                Array.Fill(image_colors, None);
                texture2D = new Texture2D(value.width, value.height, TextureFormat.RGBA32, false);
                Layer_image = GetComponent<RawImage>();
                Layer_image.texture = texture2D;
                UpdateTex();
            }
        }

        public bool Active
        {
            get => active;
        }
        public Color32[] Image_colors { get => image_colors; }

        private void Start()
        {
            SetActive();
        }
        public void ClearTextureMemory()
        {
            if (Layer_image != null)
            {
                Layer_image.texture = null;
            }

            if (texture2D != null)
            {
                Destroy(texture2D);
                texture2D = null;
            }

            textureRegionBuffer = null;
            image_colors = null;

        }
        /// <summary>
        /// 将纹理（texture）的像素更新到 image_colors 数组
        /// 用于从渲染的纹理中提取像素数据以进行后续处理
        /// </summary>
        public void ColorArrayUpdate()
        {
            if (Layer_image == null || Layer_image.texture == null)
            {
                Debug.LogWarning("[LayerManager] ColorArrayUpdate skipped: Layer_image or texture is null");
                return;
            }
            var srcTex = Layer_image.texture as Texture2D;
            if (srcTex == null)
            {
                Debug.LogWarning("[LayerManager] ColorArrayUpdate skipped: Layer_image.texture is not Texture2D");
                return;
            }
            Texture2D newTexture = ScaleTexture(srcTex, MainManager.instance.width, MainManager.instance.height);
            if (newTexture == null)
            {
                Debug.LogWarning("[LayerManager] ColorArrayUpdate skipped: ScaleTexture returned null");
                return;
            }
            image_colors = newTexture.GetPixels32();
            Destroy(newTexture);
        }
        /// <summary>
        /// 缩放纹理到指定分辨率
        /// 使用双线性插值进行高质量缩放
        /// </summary>
        /// <param name="source">源纹理</param>
        /// <param name="targetWidth">目标宽度</param>
        /// <param name="targetHeight">目标高度</param>
        /// <returns>缩放后的新纹理</returns>
        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            if (source == null)
            {
                Debug.LogWarning("[LayerManager] ScaleTexture source null");
                return null;
            }

            Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
            Color[] rpixels = result.GetPixels(0);
            float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
            float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);

            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }

            result.SetPixels(rpixels, 0);
            result.Apply();
            return result;
        }

        /// <summary>
        /// 指针按下事件处理
        /// </summary>
        /// <param name="eventData">指针事件数据</param>
        public void OnPointerDown(PointerEventData eventData)
        {
        }

        /// <summary>
        /// 指针移动事件处理
        /// 当指针按下且图层处于激活状态时，执行绘图操作
        /// 根据移动距离插值计算绘制点，确保绘制轨迹平滑
        /// </summary>
        /// <param name="eventData">指针事件数据</param>
        public void OnPointerMove(PointerEventData eventData)
        {
            if (click_state && active)
            {
                // 计算两点间的距离，将轨迹分成若干段进行插值
                int slicenum = (int)(Vector2.Distance(last_position, eventData.position) / 5 + 1);
                for (int i = 1; i <= slicenum; i++)
                {
                    if (MainManager.instance.currentBrush != null)
                    {
                        // 线性插值计算中间点
                        Draw(Vector2.Lerp(last_position, eventData.position, i / (float)slicenum), MainManager.instance.currentBrush);
                    }
                }

                UpdateTex();  // 更新纹理显示
                last_position = eventData.position;  // 更新上一位置
                CursorManager.instance.OnPointerMove(eventData.position);  // 更新光标
            }
        }

        /// <summary>
        /// 指针释放事件处理
        /// </summary>
        /// <param name="eventData">指针事件数据</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            click_state = false;
            CursorManager.instance.gameObject.SetActive(click_state);
        }

        /// <summary>
        /// 在指定位置绘制笔刷
        /// 将笔刷的形状数据（brush_data）叠加到图层像素数组上
        /// </summary>
        /// <param name="position">绘制位置（屏幕坐标）</param>
        /// <param name="brush">使用的笔刷</param>
        public void Draw(Vector2 position, Brush brush)
        {
            position = new Vector2(position.x - 571, position.y - 126);

            foreach (var item in brush.brush_data)
            {
                int xpos = (int)(position + item).x;
                int ypos = (int)(position + item).y;

                if (xpos >= 0 && xpos < layerSize.width && ypos >= 0 && ypos < layerSize.height)
                {
                    image_colors[xpos + ypos * layerSize.width] = brush.color;
                }
            }
        }
        public void SetActive()
        {
            active = true;
            transform.SetAsLastSibling();
            ApplyLayerColor(ActiveColor);
        }
        public void SetActive1()
        {
            active = true;
            ApplyLayerColor(ActiveColor);
        }
        public void SetDisactive()
        {
            active = false;
            ApplyLayerColor(DisActiveColor);
        }

        public void SetDisactiveWithoutNotify()
        {
            active = false;
            ApplyLayerColor(DisActiveColor);
        }

        public void SetActiveWithoutNotify()
        {
            active = true;
            ApplyLayerColor(ActiveColor);
        }

        private void ApplyLayerColor(Color32 color)
        {
            if (image_colors == null || layerSize == null)
            {
                return;
            }

            bool changed = false;
            int minX = layerSize.width;
            int minY = layerSize.height;
            int maxX = -1;
            int maxY = -1;
            if (data != null && data.Length > 0)
            {
                int width = layerSize.width;
                int height = layerSize.height;
                foreach (var point in data)
                {
                    if (point == null || point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
                    {
                        continue;
                    }

                    int index = point.x + point.y * width;
                    if (index >= 0 && index < image_colors.Length)
                    {
                        image_colors[index] = color;
                        changed = true;
                        if (point.x < minX) minX = point.x;
                        if (point.y < minY) minY = point.y;
                        if (point.x > maxX) maxX = point.x;
                        if (point.y > maxY) maxY = point.y;
                    }
                }
            }
            else
            {
                int width = layerSize.width;
                for (int i = 0; i < image_colors.Length; i++)
                {
                    if (image_colors[i].a != 0)
                    {
                        image_colors[i] = color;
                        changed = true;
                        int x = i % width;
                        int y = i / width;
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            if (changed)
            {
                UpdateTexRegion(minX, minY, maxX, maxY);
            }
        }

        /// <summary>
        /// 更新纹理显示
        /// 将 image_colors 数组的数据应用到 texture2D，并提交更改
        /// 调用后图层画面会更新显示
        /// </summary>
        public void UpdateTex()
        {
            if (texture2D == null)
            {
                Debug.LogWarning("[LayerManager] UpdateTex skipped: texture2D is null");
                return;
            }
            if (image_colors == null || image_colors.Length != (layerSize.width * layerSize.height))
            {
                Debug.LogWarning("[LayerManager] UpdateTex skipped: image_colors invalid");
                return;
            }
            texture2D.SetPixels32(image_colors);
            texture2D.Apply(false);
        }

        private void UpdateTexRegion(int minX, int minY, int maxX, int maxY)
        {
            if (texture2D == null)
            {
                Debug.LogWarning("[LayerManager] UpdateTexRegion skipped: texture2D is null");
                return;
            }
            if (image_colors == null || layerSize == null || image_colors.Length != (layerSize.width * layerSize.height))
            {
                Debug.LogWarning("[LayerManager] UpdateTexRegion skipped: image_colors invalid");
                return;
            }

            minX = Mathf.Clamp(minX, 0, layerSize.width - 1);
            minY = Mathf.Clamp(minY, 0, layerSize.height - 1);
            maxX = Mathf.Clamp(maxX, 0, layerSize.width - 1);
            maxY = Mathf.Clamp(maxY, 0, layerSize.height - 1);
            if (maxX < minX || maxY < minY)
            {
                return;
            }

            int blockWidth = maxX - minX + 1;
            int blockHeight = maxY - minY + 1;
            int blockLength = blockWidth * blockHeight;
            if (blockLength <= 0)
            {
                return;
            }

            if (blockLength > image_colors.Length / 4)
            {
                UpdateTex();
                return;
            }

            if (textureRegionBuffer == null || textureRegionBuffer.Length != blockLength)
            {
                textureRegionBuffer = new Color32[blockLength];
            }

            for (int y = 0; y < blockHeight; y++)
            {
                Array.Copy(image_colors, (minY + y) * layerSize.width + minX, textureRegionBuffer, y * blockWidth, blockWidth);
            }

            texture2D.SetPixels32(minX, minY, blockWidth, blockHeight, textureRegionBuffer);
            texture2D.Apply(false);
        }

        public PositionInt[] GetData()
        {
            ConcurrentBag<PositionInt> data = new ConcurrentBag<PositionInt>();

            Parallel.For(0, layerSize.width, (x) =>
            {
                Parallel.For(0, layerSize.height, (y) =>
                {
                    if (image_colors[x + y * layerSize.width].a != 0)
                    {
                        data.Add(new PositionInt(x, y));
                    }
                });
            });
            return data.ToArray();
        }
        public void LoadData(PositionInt[] array_PosData)
        {
            Parallel.ForEach(array_PosData, item =>
            {
                int xpos = (int)item.x;
                int ypos = (int)item.y;

                if (xpos >= 0 && xpos < layerSize.width && ypos >= 0 && ypos < layerSize.height)
                {
                    image_colors[xpos + ypos * layerSize.width] = new Color32(0, 85, 255, 255);// new Color32(255, 0, 0, 255); 
                }
            });
            UpdateTex();
        }
        /// <summary>
        /// 获取图层的连通切片
        /// 将图层中所有连接的像素点分组，每组为一个独立的连通区域
        /// 使用 BFS（广度优先搜索）算法进行连通性分析
        /// </summary>
        /// <returns>连通切片列表，每个切片包含一组相连的像素坐标</returns>
        List<List<PositionInt>> GetSlices()
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
        public List<List<PositionInt>> GetSlices(Size studentLayerSize, Color32[] studenteffectcolor)
        {
            List<List<PositionInt>> slices = new List<List<PositionInt>>();

            ColorsWithIndex src = new ColorsWithIndex(studentLayerSize.width, studentLayerSize.height, studenteffectcolor);

            bool CheckPointExist(List<PositionInt> dst, int width, int height)
            {
                Stack<PositionInt> stack = new Stack<PositionInt>();
                stack.Push(new PositionInt(width, height));
                while (stack.Count > 0)
                {
                    PositionInt current = stack.Pop();
                    if (current.x >= 0 && current.x < studentLayerSize.width && current.y >= 0 && current.y < studentLayerSize.height)
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

            for (int i = 0; i < studentLayerSize.width; i++)
            {
                for (int j = 0; j < studentLayerSize.height; j++)
                {
                    if (src[i, j].a > 0)
                    {
                        List<PositionInt> connectpool = new List<PositionInt>();
                        if (CheckPointExist(connectpool, i, j))
                        {
                            connectpool = Tools.ListRemoveRepeatElement(connectpool);
                            slices.Add(connectpool);
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

        /// <summary>
        /// 检查线类型（静态方法）
        /// 根据连通切片的数量和大小分布，判断图线是实线、虚线还是点画线
        /// 使用 DBSCAN 聚类算法对切片进行分组分析
        /// </summary>
        /// <param name="slices">连通切片列表</param>
        /// <returns>线类型枚举（实线、虚线、点画线、未知）</returns>
        public static linetype CheckLineType(List<List<PositionInt>> slices)
        {
            // 找出最长切片的长度
            int maxlength = 0;
            int count = 0;
            foreach (var item in slices)
            {
                if (item.Count > maxlength)
                {
                    maxlength = item.Count;
                }
            }

            // 统计长度超过最大值一半的切片数量
            foreach (var item in slices)
            {
                if (item.Count >= maxlength / 2f)
                {
                    count++;
                }
            }

            Debug.Log("线段切分片数：" + slices.Count);

            // 只有一个切片 → 实线
            if (count == 1)
            {
                return linetype.实线;
            }

            // 收集每个切片的长度作为特征
            List<int> data = new List<int>();
            foreach (var slice in slices)
            {
                data.Add(slice.Count);
            }

            // 使用 DBSCAN 聚类算法对切片长度进行聚类
            double epsilon = 300;  // 聚类半径
            int minPts = 3;        // 最小点数
            Dbscan dbscan = new Dbscan(epsilon, minPts);
            int[] clusters = dbscan.Cluster(data);

            // 按聚类标签分组
            Dictionary<int, List<int>> linedata = new Dictionary<int, List<int>>();
            for (int i = 0; i < clusters.Length; i++)
            {
                if (linedata.ContainsKey(clusters[i]))
                {
                    linedata[clusters[i]].Add(data[i]);
                }
                else
                {
                    List<int> ints = new List<int> { data[i] };
                    linedata.Add(clusters[i], ints);
                }
            }

            // 一个聚类 → 虚线
            if (linedata.Count == 1)
            {
                return linetype.虚线;
            }

            // 多个聚类 → 判断是虚线还是点画线
            if (linedata.Count > 1)
            {
                // 按平均值排序
                var values = linedata.Values.ToArray();
                Array.Sort(values, (List<int> x, List<int> y) =>
                {
                    return (int)(x.Average() - y.Average());
                });
                Array.Reverse(values);

                // 如果第一类切片数量是第二类的2倍以上 → 虚线
                if (values[0].Count / (float)values[1].Count > 2)
                {
                    return linetype.虚线;
                }

                // 否则 → 点画线
                return linetype.点画线;
            }

            return linetype.unknown;  // 无法判断
        }
        public linetype GetLineType()
        {
            return CheckLineType(Algorithm.GetSlices(layerSize, image_colors));
        }
        public linetype GetLineType(Size layerSize, Color32[] image_colors)
        {
            return CheckLineType(Algorithm.GetSlices(layerSize, image_colors));
        }

        public Arc GetArcData(Size layerSize, Color32[] image_colors)
        {
            List<PositionInt> positionInts = Algorithm.Zhang_Suen(layerSize, image_colors);
            Circle circle = Algorithm.LeastSquaresCircleFitting(positionInts);
            return Algorithm.GetArcData(circle, positionInts);
        }
        public Circle GetCircleData(Size layerSize, Color32[] image_colors)
        {
            return Algorithm.LeastSquaresCircleFitting(Algorithm.Zhang_Suen(layerSize, image_colors));
        }

        public Ellipse GetEllipseData(Size layerSize, Color32[] image_colors)
        {
            return Algorithm.LeastSquaresEllipseFitting(Algorithm.Zhang_Suen(layerSize, image_colors));
        }
        public List<PositionInt> GetMarkData(LayerManager layer)
        {
            List<PositionInt> frameSelectAllPos = layer.frameSelectData.ToList();
            List<PositionInt> frameSelectPos = Algorithm.GetOBB(frameSelectAllPos);
            return frameSelectPos;
        }
        public int GetLineWidthData(LayerManager layer)
        {
            List<PositionInt> frameSelectAllPos = layer.frameSelectData.ToList();
            List<PositionInt> frameSelectPos = Algorithm.GetOBB(frameSelectAllPos);
            if (frameSelectPos.Count > 0 && frameSelectPos.Count == 4)
            {
                float width1 = Vector2.Distance(new Vector2(frameSelectPos[0].x, frameSelectPos[0].y), new Vector2(frameSelectPos[1].x, frameSelectPos[1].y));
                foreach (var item in frameSelectPos)
                {


                    for (int i = 0; i < layerSize.width - 1; i++)
                    {
                        for (int j = 0; j < layerSize.height - 1; j++)
                        {
                            float dis = 2 - (float)Math.Sqrt(Math.Pow(i - item.x, 2) + Math.Pow(j - item.y, 2)) * 2;
                            if (dis < 1f && dis >= 0f)
                            {
                                image_colors[i + j * layerSize.width] = new Color32(255, 0, 0, 255);

                            }
                        }
                    }
                }

                UpdateTex();
                Debug.Log("width1" + width1);
            }
            return 1;
        }
        public bool IsSameCircle(LayerManager layer)
        {
            bool isSame = false;
            Circle standardCircle = layer.circleData;
            HashSet<PositionInt> connectpool = new HashSet<PositionInt>();
            for (int i = 0; i < layer.layerSize.width - 1; i++)
            {
                for (int j = 0; j < layer.layerSize.height - 1; j++)
                {
                    float dis = (float)Math.Sqrt(Math.Pow(i - standardCircle.X, 2) + Math.Pow(j - standardCircle.Y, 2)) - standardCircle.Radius;
                    if (dis < 1f && dis >= 0f)
                    {
                        connectpool.Add(new PositionInt(i, j));
                        Debug.Log(i.ToString() + "|" + j.ToString() + "count" + connectpool.Count);

                    }
                }
            }
            return isSame;
        }
        public void CalCameraPicPixels(Size layerSize, Color32[] image_colors)
        {


            int a = 0;
            for (int i = 0; i < layerSize.width - 1; i++)
            {
                for (int j = 0; j < layerSize.height - 1; j++)
                {

                    if (image_colors[i + j * layerSize.width].a > 0)
                    {
                        a++;
                    }
                }
            }
            Debug.Log("照片像素" + a);

        }
        List<PositionInt> GetHollow()
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

        List<PositionInt> GetConvexHull(List<PositionInt> points)
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
                if (i == n - 1 || CrossProduct(p1, points[i], p2) > 0)
                {
                    while (up.Count >= 2 && CrossProduct(up[up.Count - 2], up[up.Count - 1], points[i]) <= 0)
                        up.RemoveAt(up.Count - 1);
                    up.Add(points[i]);
                }
                if (i == n - 1 || CrossProduct(p1, points[i], p2) < 0)
                {
                    while (down.Count >= 2 && CrossProduct(down[down.Count - 2], down[down.Count - 1], points[i]) >= 0)
                        down.RemoveAt(down.Count - 1);
                    down.Add(points[i]);
                }
            }

            down.Reverse();
            up.AddRange(down);
            return up;
        }
        /// <summary>
        /// 叉积判断三点旋转方向（>0逆时针, <0顺时针, =0共线）
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private float CrossProduct(PositionInt a, PositionInt b, PositionInt c)
        {
            return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        }

        public List<PositionInt> GetOBB(List<PositionInt> CH)
        {

            // xy 空间中，十个数据点
            double[,] coord_xy = new double[2, CH.Count];
            for (int i = 0; i < CH.Count; i++)
            {
                coord_xy[0, i] = CH[i].x;
                coord_xy[1, i] = CH[i].y;
            }
            Matrix<double> coord_xy_m = Matrix<double>.Build.DenseOfArray(coord_xy);
            // 求协方差矩阵
            var covMatrix = CalculateCovarianceMatrix(coord_xy_m.Transpose());
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
            List<PositionInt> res = new()
            {
                new PositionInt((int)OBB_xy[0, 0], (int)OBB_xy[1, 0]),
                new PositionInt((int)OBB_xy[0, 1], (int)OBB_xy[1, 1]),
                new PositionInt((int)OBB_xy[0, 2], (int)OBB_xy[1, 2]),
                new PositionInt((int)OBB_xy[0, 3], (int)OBB_xy[1, 3])
            };


            return res;
        }


        bool IsPointInsideRectangle(PositionInt p1, PositionInt p2, PositionInt p3, PositionInt p4, PositionInt p)
        {
            double cross1 = CrossProduct(p1, p2, p);
            double cross2 = CrossProduct(p2, p3, p);
            double cross3 = CrossProduct(p3, p4, p);
            double cross4 = CrossProduct(p4, p1, p);

            return (cross1 >= 0 && cross2 >= 0 && cross3 >= 0 && cross4 >= 0) || (cross1 <= 0 && cross2 <= 0 && cross3 <= 0 && cross4 <= 0);
        }


        public List<PositionInt> GetOBB()
        {
            var arclist = Algorithm.GetOBB(Algorithm.GetConvexHull(Algorithm.GetHollow(layerSize, image_colors)));

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

            return arclist;
        }


        Matrix<double> CalculateCovarianceMatrix(Matrix<double> data)
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
        Vector2 GetMidpoint(Vector2 a, Vector2 b)
        {
            return new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2);
        }

    }

}
