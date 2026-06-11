using System;
using System.Collections.Generic;
using UnityEngine;

namespace jxzt
{
    internal struct GpuLineEvidenceCounters
    {
        public int RawOpaqueInRoiCount;
        public int NearLineCandidateCount;
        public int IgnoredOpaqueCount;
        public bool Overflow;
        public int RoiPixelCount;
    }

    internal sealed class GpuLineEvidencePrefilter : IDisposable
    {
        private const string KernelName = "CSPrefilterLinePixels";
        private const int ThreadGroupSize = 8;
        private const int CounterCount = 4;

        private readonly ComputeShader _shader;
        private int _kernel = -1;
        private Texture2D _studentTexture;
        private LayerManager _textureLayer;
        private Color32[] _textureSource;
        private int _textureWidth;
        private int _textureHeight;
        private ComputeBuffer _outputBuffer;
        private ComputeBuffer _counterBuffer;
        private int _outputCapacity;
        private GpuPosition[] _readbackBuffer;
        private readonly uint[] _counterReadback = new uint[CounterCount];
        private readonly uint[] _zeroCounters = new uint[CounterCount];

        private struct GpuPosition
        {
            public int x;
            public int y;
        }

        internal GpuLineEvidencePrefilter(ComputeShader shader)
        {
            _shader = shader;
        }

        internal bool IsAvailable
        {
            get
            {
                return SystemInfo.supportsComputeShaders
                       && _shader != null
                       && EnsureKernel(out _);
            }
        }

        internal bool IsForShader(ComputeShader shader)
        {
            return ReferenceEquals(_shader, shader);
        }

        internal void InvalidateStudentTexture()
        {
            _textureLayer = null;
            _textureSource = null;
        }

        internal bool TryPrefilterLinePixels(
            LayerManager studentLayer,
            AnswerCheck.PixelBounds bounds,
            Vector2 axis,
            Vector2 center,
            float standardMin,
            float standardMax,
            float bandWidth,
            float endpointSlack,
            float extensionSearchSlack,
            int maxOutputPixels,
            out List<PositionInt> nearLineCandidatePixels,
            out GpuLineEvidenceCounters counters,
            out string failReason)
        {
            nearLineCandidatePixels = new List<PositionInt>();
            counters = new GpuLineEvidenceCounters();
            failReason = string.Empty;

            if (!SystemInfo.supportsComputeShaders)
            {
                failReason = "当前平台不支持 ComputeShader";
                return false;
            }

            if (_shader == null)
            {
                failReason = "LineEvidencePrefilter ComputeShader 未挂载";
                return false;
            }

            if (!EnsureKernel(out failReason))
            {
                return false;
            }

            if (studentLayer == null || studentLayer.LayerSize == null || studentLayer.Image_colors == null)
            {
                failReason = "学生图层或 Image_colors 为空";
                return false;
            }

            int width = studentLayer.LayerSize.width;
            int height = studentLayer.LayerSize.height;
            if (width <= 0 || height <= 0 || studentLayer.Image_colors.Length == 0)
            {
                failReason = "学生图层尺寸无效";
                return false;
            }

            long expectedPixelCount = (long)width * height;
            if (expectedPixelCount > int.MaxValue || studentLayer.Image_colors.Length != expectedPixelCount)
            {
                failReason = "学生图层 Image_colors 长度与图层尺寸不一致";
                return false;
            }

            AnswerCheck.PixelBounds clamped = ClampBounds(bounds, width, height);
            int roiWidth = clamped.maxX - clamped.minX + 1;
            int roiHeight = clamped.maxY - clamped.minY + 1;
            if (roiWidth <= 0 || roiHeight <= 0)
            {
                failReason = "ROI 为空";
                return false;
            }

            if (axis.sqrMagnitude <= 0.0001f)
            {
                failReason = "标准线主轴无效";
                return false;
            }

            maxOutputPixels = Mathf.Max(1, maxOutputPixels);
            long roiPixelCount = (long)roiWidth * roiHeight;
            counters.RoiPixelCount = roiPixelCount > int.MaxValue ? int.MaxValue : (int)roiPixelCount;

            try
            {
                EnsureStudentTexture(studentLayer, width, height);
                EnsureBuffers(maxOutputPixels);

                _counterBuffer.SetData(_zeroCounters);

                Vector2 normalizedAxis = axis.normalized;
                _shader.SetTexture(_kernel, "_StudentTex", _studentTexture);
                _shader.SetBuffer(_kernel, "_OutputPixels", _outputBuffer);
                _shader.SetBuffer(_kernel, "_Counters", _counterBuffer);
                _shader.SetInt("_TextureWidth", width);
                _shader.SetInt("_TextureHeight", height);
                _shader.SetInt("_RoiMinX", clamped.minX);
                _shader.SetInt("_RoiMinY", clamped.minY);
                _shader.SetInt("_RoiMaxX", clamped.maxX);
                _shader.SetInt("_RoiMaxY", clamped.maxY);
                _shader.SetInt("_MaxOutputPixels", maxOutputPixels);
                _shader.SetVector("_Axis", new Vector4(normalizedAxis.x, normalizedAxis.y, 0f, 0f));
                _shader.SetVector("_Center", new Vector4(center.x, center.y, 0f, 0f));
                _shader.SetFloat("_StandardMin", standardMin);
                _shader.SetFloat("_StandardMax", standardMax);
                _shader.SetFloat("_BandWidth", Mathf.Max(1f, bandWidth));
                _shader.SetFloat("_EndpointSlack", Mathf.Max(0f, endpointSlack));
                _shader.SetFloat("_ExtensionSearchSlack", Mathf.Max(0f, extensionSearchSlack));

                int dispatchX = Mathf.CeilToInt(roiWidth / (float)ThreadGroupSize);
                int dispatchY = Mathf.CeilToInt(roiHeight / (float)ThreadGroupSize);
                _shader.Dispatch(_kernel, dispatchX, dispatchY, 1);

                _counterBuffer.GetData(_counterReadback);
                counters.RawOpaqueInRoiCount = SafeUIntToInt(_counterReadback[0]);
                counters.NearLineCandidateCount = SafeUIntToInt(_counterReadback[1]);
                counters.IgnoredOpaqueCount = SafeUIntToInt(_counterReadback[2]);
                counters.Overflow = _counterReadback[3] != 0 || counters.NearLineCandidateCount > maxOutputPixels;

                if (counters.Overflow)
                {
                    failReason = "GPU 近线候选输出超过上限";
                    return false;
                }

                int readCount = Mathf.Min(counters.NearLineCandidateCount, maxOutputPixels);
                EnsureReadbackBuffer(readCount);
                if (readCount > 0)
                {
                    _outputBuffer.GetData(_readbackBuffer, 0, 0, readCount);
                    nearLineCandidatePixels = new List<PositionInt>(readCount);
                    for (int i = 0; i < readCount; i++)
                    {
                        nearLineCandidatePixels.Add(new PositionInt(_readbackBuffer[i].x, _readbackBuffer[i].y));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                failReason = "GPU 预筛异常: " + ex.Message;
                return false;
            }
        }

        public void Dispose()
        {
            ReleaseBuffer(ref _outputBuffer);
            ReleaseBuffer(ref _counterBuffer);
            _outputCapacity = 0;
            _readbackBuffer = null;
            _textureLayer = null;
            _textureSource = null;

            if (_studentTexture != null)
            {
                DestroyTexture(_studentTexture);
                _studentTexture = null;
            }
        }

        private bool EnsureKernel(out string failReason)
        {
            failReason = string.Empty;
            if (_kernel >= 0)
            {
                return true;
            }

            if (_shader == null)
            {
                failReason = "LineEvidencePrefilter ComputeShader 未挂载";
                return false;
            }

            if (!_shader.HasKernel(KernelName))
            {
                failReason = "ComputeShader 缺少 kernel: " + KernelName;
                return false;
            }

            _kernel = _shader.FindKernel(KernelName);
            return _kernel >= 0;
        }

        private void EnsureStudentTexture(LayerManager studentLayer, int width, int height)
        {
            bool recreate = _studentTexture == null
                            || _textureWidth != width
                            || _textureHeight != height;
            if (recreate)
            {
                if (_studentTexture != null)
                {
                    DestroyTexture(_studentTexture);
                }

                _studentTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                _textureWidth = width;
                _textureHeight = height;
                _textureLayer = null;
                _textureSource = null;
            }

            Color32[] source = studentLayer.Image_colors;
            if (!ReferenceEquals(_textureLayer, studentLayer) || !ReferenceEquals(_textureSource, source) || recreate)
            {
                _studentTexture.SetPixels32(source);
                _studentTexture.Apply(false, false);
                _textureLayer = studentLayer;
                _textureSource = source;
            }
        }

        private void EnsureBuffers(int maxOutputPixels)
        {
            if (_outputBuffer == null || _outputCapacity < maxOutputPixels)
            {
                ReleaseBuffer(ref _outputBuffer);
                _outputBuffer = new ComputeBuffer(maxOutputPixels, 8);
                _outputCapacity = maxOutputPixels;
            }

            if (_counterBuffer == null)
            {
                _counterBuffer = new ComputeBuffer(CounterCount, sizeof(uint));
            }
        }

        private void EnsureReadbackBuffer(int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (_readbackBuffer == null || _readbackBuffer.Length < count)
            {
                _readbackBuffer = new GpuPosition[count];
            }
        }

        private static void ReleaseBuffer(ref ComputeBuffer buffer)
        {
            if (buffer == null)
            {
                return;
            }

            buffer.Release();
            buffer = null;
        }

        private static void DestroyTexture(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(texture);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static int SafeUIntToInt(uint value)
        {
            return value > int.MaxValue ? int.MaxValue : (int)value;
        }

        private static AnswerCheck.PixelBounds ClampBounds(AnswerCheck.PixelBounds bounds, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return new AnswerCheck.PixelBounds { minX = 0, maxX = -1, minY = 0, maxY = -1 };
            }

            return new AnswerCheck.PixelBounds
            {
                minX = Mathf.Clamp(bounds.minX, 0, width - 1),
                maxX = Mathf.Clamp(bounds.maxX, 0, width - 1),
                minY = Mathf.Clamp(bounds.minY, 0, height - 1),
                maxY = Mathf.Clamp(bounds.maxY, 0, height - 1)
            };
        }
    }
}
