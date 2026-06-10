using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

namespace jxzt
{
    public static class PendingTitleCodeStorage
    {
        private const string FileName = "pending_titlecodes.json";
        private static readonly object _lock = new object();

        private static string PrimaryFilePath
        {
            get
            {
                try
                {
                    return Path.Combine(Application.streamingAssetsPath, FileName);
                }
                catch
                {
                    return FileName;
                }
            }
        }

        private static string BackupFilePath
        {
            get
            {
                try
                {
                    return Path.Combine(Application.persistentDataPath, FileName);
                }
                catch
                {
                    return FileName;
                }
            }
        }

        public static Dictionary<string, string> Load()
        {
            lock (_lock)
            {
                try
                {
                    string pathToUse = null;
                    if (File.Exists(PrimaryFilePath)) pathToUse = PrimaryFilePath;
                    else if (File.Exists(BackupFilePath)) pathToUse = BackupFilePath;

                    if (pathToUse == null) return new Dictionary<string, string>();

                    var json = File.ReadAllText(pathToUse);
                    if (string.IsNullOrWhiteSpace(json)) return new Dictionary<string, string>();

                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    return dic ?? new Dictionary<string, string>();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[PendingTitleCodeStorage] Load failed: {ex.Message}");
                    return new Dictionary<string, string>();
                }
            }
        }

        public static void Save(Dictionary<string, string> dic)
        {
            lock (_lock)
            {
                var json = JsonConvert.SerializeObject(dic ?? new Dictionary<string, string>(), Formatting.Indented);
                try
                {
                    var dir = Path.GetDirectoryName(PrimaryFilePath) ?? Application.streamingAssetsPath;
                    Directory.CreateDirectory(dir);
                    File.WriteAllText(PrimaryFilePath, json);
#if UNITY_EDITOR
                    Debug.Log($"[PendingTitleCodeStorage] Saved {dic?.Count ?? 0} items to {PrimaryFilePath}");
#endif
                    return;
                }
                catch (Exception exPrimary)
                {
                    Debug.LogWarning($"[PendingTitleCodeStorage] Save to streamingAssetsPath failed: {exPrimary.Message}. Will try persistentDataPath as fallback.");
                }

                try
                {
                    var backupDir = Path.GetDirectoryName(BackupFilePath) ?? Application.persistentDataPath;
                    Directory.CreateDirectory(backupDir);
                    File.WriteAllText(BackupFilePath, json);
#if UNITY_EDITOR
                    Debug.Log($"[PendingTitleCodeStorage] Saved {dic?.Count ?? 0} items to {BackupFilePath}");
#endif
                }
                catch (Exception exBackup)
                {
                    Debug.LogError($"[PendingTitleCodeStorage] Save failed on both paths: {exBackup.Message}");
                }
            }
        }

        public static void DeleteFile()
        {
            lock (_lock)
            {
                try { if (File.Exists(PrimaryFilePath)) File.Delete(PrimaryFilePath); }
                catch (Exception ex) { Debug.LogError($"[PendingTitleCodeStorage] Delete primary file failed: {ex.Message}"); }
                try { if (File.Exists(BackupFilePath)) File.Delete(BackupFilePath); }
                catch (Exception ex) { Debug.LogError($"[PendingTitleCodeStorage] Delete backup file failed: {ex.Message}"); }
            }
        }
    }
}
