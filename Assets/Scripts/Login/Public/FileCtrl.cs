using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileCtrl
{
    /// <summary>
    /// 文件操作工具类
    /// 封装文件流的创建和读取操作
    /// 确保目录存在，支持创建或打开文件
    /// </summary>
    private readonly FileStream _fs;

    public static FileStream createFilestream(string directoryPath, string fileName)
    {
        string filePath = Path.Combine(directoryPath, fileName);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        return !File.Exists(filePath) ? File.Create(filePath) : new FileStream(filePath, FileMode.OpenOrCreate);
    }

    public byte[] Read()
    {
        byte[] dataBytes = new byte[_fs.Length];
        _fs.Read(dataBytes, 0, dataBytes.Length);
        _fs.Flush();
        return dataBytes;
    }

    public void Write(byte[] dataBytes)
    {
        _fs.Seek(0, SeekOrigin.Begin);
        _fs.Write(dataBytes, 0, dataBytes.Length);
        _fs.SetLength(dataBytes.Length);
        _fs.Flush();
    }
    public void Close()
    {
        _fs.Close();
    }
}
