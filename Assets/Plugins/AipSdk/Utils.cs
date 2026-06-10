using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Baidu.Aip
{
	// Token: 0x02000007 RID: 7
	public class Utils
	{
		// Token: 0x06000033 RID: 51 RVA: 0x00002CB8 File Offset: 0x00000EB8
		public static string StreamToString(Stream ss, Encoding enc)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(ss, enc))
			{
				result = streamReader.ReadToEnd();
			}
			ss.Close();
			return result;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002CF8 File Offset: 0x00000EF8
		public static string ParseQueryString(Dictionary<string, string> querys)
		{
			if (querys.Count == 0)
			{
				return "";
			}
			return (from pair in querys
			select pair.Key + "=" + pair.Value).Aggregate((string a, string b) => a + "&" + b);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002D5C File Offset: 0x00000F5C
		public static string UriEncode(string input, bool encodeSlash = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in Encoding.UTF8.GetBytes(input))
			{
				if ((b >= 97 && b <= 122) || (b >= 65 && b <= 90) || (b >= 48 && b <= 57) || b == 95 || b == 45 || b == 126 || b == 46)
				{
					stringBuilder.Append((char)b);
				}
				else if (b == 47)
				{
					if (encodeSlash)
					{
						stringBuilder.Append("%2F");
					}
					else
					{
						stringBuilder.Append((char)b);
					}
				}
				else
				{
					stringBuilder.Append('%').Append(b.ToString("X2"));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002E0C File Offset: 0x0000100C
		public static string Md5(string text)
		{
			byte[] bytes = Encoding.Default.GetBytes(text);
			return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).ToUpper();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002E3C File Offset: 0x0000103C
		public static byte[] StreamToBytes(Stream input)
		{
			byte[] array = new byte[16384];
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int count;
				while ((count = input.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002E9C File Offset: 0x0000109C
		public static long UnixTimestamp()
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
		}
	}
}
