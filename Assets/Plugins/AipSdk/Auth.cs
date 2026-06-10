using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip
{
	// Token: 0x02000005 RID: 5
	public class Auth
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00002854 File Offset: 0x00000A54
		private Auth()
		{
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000285C File Offset: 0x00000A5C
		public static JObject OpenApiFetchToken(string ak, string sk, bool throws = false, bool debugLog = false)
		{
			Dictionary<string, string> querys = new Dictionary<string, string>
			{
				{
					"grant_type",
					"client_credentials"
				},
				{
					"client_id",
					ak
				},
				{
					"client_secret",
					sk
				}
			};
			string text = string.Format("{0}?{1}", "https://aip.baidubce.com/oauth/2.0/token", Utils.ParseQueryString(querys));
			if (debugLog)
			{
				Console.WriteLine(text);
			}
			try
			{
				Task<HttpResponseMessage> task = new HttpClient().PostAsync(text, null);
				if (task.Result.IsSuccessStatusCode)
				{
					JObject jobject = JsonConvert.DeserializeObject(task.Result.Content.ReadAsStringAsync().Result) as JObject;
					if (jobject["access_token"] != null && jobject["expires_in"] != null)
					{
						return jobject;
					}
					if (throws)
					{
						throw new AipException("Failed to request token. " + (string)jobject["error_description"]);
					}
					return null;
				}
				else if (throws)
				{
					string str = "Failed to request token. ";
					string str2 = task.Result.StatusCode.ToString();
					HttpContent content = task.Result.Content;
					throw new AipException(str + str2 + ((content != null) ? content.ToString() : null));
				}
			}
			catch (Exception ex)
			{
				if (throws)
				{
					throw new AipException("Failed to request token. " + ex.Message);
				}
				return null;
			}
			return null;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000029B8 File Offset: 0x00000BB8
		private static string CanonicalRequest(AipHttpRequest aipHttpRequest)
		{
			Uri uri = aipHttpRequest.Uri;
			string text = Utils.UriEncode(uri.AbsolutePath, false);
			string text2 = (from pair in (from pair in aipHttpRequest.Querys
			where !pair.Key.Equals("authorization")
			select new KeyValuePair<string, string>(Utils.UriEncode(pair.Key, false), Utils.UriEncode(pair.Value, false))).ToList<KeyValuePair<string, string>>()
			orderby pair.Key
			select string.Format("{0}={1}", pair.Key, Utils.UriEncode(pair.Value, true))).DefaultIfEmpty("").Aggregate((string a, string b) => a + "&" + b);
			string text3 = uri.Host;
			if ((!(uri.Scheme == "https") || uri.Port != 443) && (!(uri.Scheme == "http") || uri.Port != 80))
			{
				text3 = text3 + ":" + uri.Port.ToString();
			}
			string text4 = "content-type:" + Utils.UriEncode(aipHttpRequest.GeneratedRequest.DefaultRequestHeaders.Accept.ToString(), true) + "\nhost:" + Utils.UriEncode(text3, false);
			return string.Format("{0}\n{1}\n{2}\n{3}", new object[]
			{
				aipHttpRequest.Method,
				text,
				text2,
				text4
			});
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002B5C File Offset: 0x00000D5C
		public static void CloudRequest(AipHttpRequest aipReq, string ak, string sk)
		{
			DateTime now = DateTime.Now;
			int num = 1200;
			string text = now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK");
			string text2 = string.Concat(new string[]
			{
				"bce-auth-v1/",
				ak,
				"/",
				text,
				"/",
				num.ToString()
			});
			string s = Auth.Hex(new HMACSHA256(Encoding.UTF8.GetBytes(sk)).ComputeHash(Encoding.UTF8.GetBytes(text2)));
			string s2 = Auth.CanonicalRequest(aipReq);
			string str = Auth.Hex(new HMACSHA256(Encoding.UTF8.GetBytes(s)).ComputeHash(Encoding.UTF8.GetBytes(s2)));
			string value = text2 + "/content-type;host/" + str;
			aipReq.GeneratedRequest.DefaultRequestHeaders.TryAddWithoutValidation("x-bce-date", text);
			aipReq.GeneratedRequest.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", value);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002C58 File Offset: 0x00000E58
		private static string Hex(byte[] data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in data)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000015 RID: 21
		private const string OAUTH_URL = "https://aip.baidubce.com/oauth/2.0/token";
	}
}
