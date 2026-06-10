using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

namespace jxzt
{
    public static class PendingXueShengZuoYeStorage
    {
        private const string FileName = "pending_answers.json";
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

        /// <summary>
        /// ���ȴ� streamingAssetsPath ��ȡ�����������ٳ��� persistentDataPath�����ˣ�
        /// </summary>
        public static Dictionary<string, string> Load()
        {
            lock (_lock)
            {
                try
                {
                    string pathToUse = null;
                    if (File.Exists(PrimaryFilePath))
                        pathToUse = PrimaryFilePath;
                    else if (File.Exists(BackupFilePath))
                        pathToUse = BackupFilePath;

                    if (pathToUse == null)
                        return new Dictionary<string, string>();

                    var json = File.ReadAllText(pathToUse);
                    if (string.IsNullOrWhiteSpace(json))
                        return new Dictionary<string, string>();

                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    return dic ?? new Dictionary<string, string>();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[PendingStorage] Load failed: {ex.Message}");
                    return new Dictionary<string, string>();
                }
            }
        }

        /// <summary>
        /// ���ȳ��Ա��浽 streamingAssetsPath����д��ʧ�ܣ�ֻ����ԭ�򣩣����˵� persistentDataPath
        /// </summary>
        public static void Save(Dictionary<string, string> dic)
        {
            lock (_lock)
            {
                var json = JsonConvert.SerializeObject(dic ?? new Dictionary<string, string>(), Formatting.Indented);

                // �ȳ���д����·����streamingAssets��
                try
                {
                    var dir = Path.GetDirectoryName(PrimaryFilePath) ?? Application.streamingAssetsPath;
                    Directory.CreateDirectory(dir);
                    File.WriteAllText(PrimaryFilePath, json);
#if UNITY_EDITOR
                    Debug.Log($"[PendingStorage] Saved {dic?.Count ?? 0} items to {PrimaryFilePath}"); 
#endif
                    return;
                }
                catch (Exception exPrimary)
                {
                    Debug.LogWarning($"[PendingStorage] Save to streamingAssetsPath failed: {exPrimary.Message}. Will try persistentDataPath as fallback.");
                }

                // ���˵� persistentDataPath
                try
                {
                    var backupDir = Path.GetDirectoryName(BackupFilePath) ?? Application.persistentDataPath;
                    Directory.CreateDirectory(backupDir);
                    File.WriteAllText(BackupFilePath, json);
#if UNITY_EDITOR
                    Debug.Log($"[PendingStorage] Saved {dic?.Count ?? 0} items to {BackupFilePath}");
#endif
                }
                catch (Exception exBackup)
                {
                    Debug.LogError($"[PendingStorage] Save failed on both paths: {exBackup.Message}");
                }
            }
        }

        public static void DeleteFile()
        {
            lock (_lock)
            {
                try
                {
                    if (File.Exists(PrimaryFilePath))
                        File.Delete(PrimaryFilePath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[PendingStorage] Delete primary file failed: {ex.Message}");
                }

                try
                {
                    if (File.Exists(BackupFilePath))
                        File.Delete(BackupFilePath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[PendingStorage] Delete backup file failed: {ex.Message}");
                }
            }
        }
    }
}