using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace jxzt
{
    /// <summary>
    /// Lightweight scoring performance logger.
    /// Keep this tool side-effect free so timing can be added without changing scoring flow.
    /// </summary>
    public static class ScoringPerf
    {
        public static bool Enabled = true;
        public static bool VerboseLayerLogs = false;
        public static bool DetailedMatchLogs = false;
        public static float SlowLayerThresholdSeconds = 2f;

        private const string Prefix = "[ScoringPerf]";
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        private static readonly Dictionary<string, long> Starts = new Dictionary<string, long>();
        private static string currentImageName = string.Empty;
        private static string currentTitleId = string.Empty;

        public static long NowMs
        {
            get { return Stopwatch.ElapsedMilliseconds; }
        }

        public static string CurrentTitleId
        {
            get { return currentTitleId; }
        }

        public static void BeginImage(string imagePath, int remainingCount, int totalCount)
        {
            currentImageName = string.IsNullOrEmpty(imagePath) ? string.Empty : Path.GetFileName(imagePath);
            Start("Image", $"file={currentImageName};remaining={remainingCount};total={totalCount}");
        }

        public static void EndImage(string detail = null)
        {
            End("Image", MergeDetail($"file={currentImageName}", detail));
            currentImageName = string.Empty;
            currentTitleId = string.Empty;
        }

        public static void BeginTitle(string titleId, string detail = null)
        {
            currentTitleId = titleId ?? string.Empty;
            Start(TitleKey("Title", currentTitleId), MergeDetail($"title={currentTitleId};file={currentImageName}", detail));
        }

        public static void EndTitle(string titleId, string detail = null)
        {
            End(TitleKey("Title", titleId), MergeDetail($"title={titleId};file={currentImageName}", detail));
        }

        public static void Event(string name, string detail = null)
        {
            if (!Enabled) return;
            Debug.Log($"{Prefix} event={name} t={NowMs}ms {Clean(detail)}");
        }

        public static void Start(string name, string detail = null)
        {
            if (!Enabled) return;
            Starts[name] = NowMs;
            Debug.Log($"{Prefix} start={name} t={NowMs}ms {Clean(detail)}");
        }

        public static long End(string name, string detail = null)
        {
            if (!Enabled) return 0L;

            long now = NowMs;
            if (!Starts.TryGetValue(name, out long startMs))
            {
                Debug.Log($"{Prefix} end={name} t={now}ms durationMs=-1 missingStart=true {Clean(detail)}");
                return -1L;
            }

            Starts.Remove(name);
            long duration = now - startMs;
            Debug.Log($"{Prefix} end={name} t={now}ms durationMs={duration} {Clean(detail)}");
            return duration;
        }

        public static IDisposable Scope(string name, string detail = null)
        {
            return new ScopeTimer(name, detail);
        }

        public static IDisposable ScopeDetailed(string name, string detail = null)
        {
            return DetailedMatchLogs ? (IDisposable)new ScopeTimer(name, detail) : NoopScope.Instance;
        }

        public static void LayerEnd(int index, int total, int layerNum, string result, long startMs, string detail = null)
        {
            if (!Enabled) return;
            long duration = NowMs - startMs;
            bool slow = duration >= SlowLayerThresholdSeconds * 1000f;
            Debug.Log($"{Prefix} layer title={currentTitleId} file={currentImageName} index={index}/{total} layerNum={layerNum} durationMs={duration} slow={slow} result={Clean(result)} {Clean(detail)}");
        }

        public static void LayerMatchMetric(int layerNum, string detail)
        {
            if (!Enabled) return;
            Debug.Log($"{Prefix} layerMetric title={currentTitleId} layerNum={layerNum} {Clean(detail)}");
        }

        public static string TitleKey(string prefix, string titleId)
        {
            return $"{prefix}:{titleId ?? string.Empty}";
        }

        private static string MergeDetail(string first, string second)
        {
            if (string.IsNullOrEmpty(first)) return second;
            if (string.IsNullOrEmpty(second)) return first;
            return first + ";" + second;
        }

        private static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
        }

        private sealed class ScopeTimer : IDisposable
        {
            private readonly string name;
            private readonly string detail;
            private bool disposed;

            public ScopeTimer(string name, string detail)
            {
                this.name = name;
                this.detail = detail;
                Start(name, detail);
            }

            public void Dispose()
            {
                if (disposed) return;
                disposed = true;
                End(name, detail);
            }
        }

        private sealed class NoopScope : IDisposable
        {
            public static readonly NoopScope Instance = new NoopScope();

            public void Dispose()
            {
            }
        }
    }
}
