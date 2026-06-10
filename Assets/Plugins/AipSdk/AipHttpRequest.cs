using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Baidu.Aip
{
	// Token: 0x02000003 RID: 3
	public class AipHttpRequest
	{
		// Token: 0x06000007 RID: 7 RVA: 0x0000209C File Offset: 0x0000029C
		private AipHttpRequest()
		{
			this.Headers = new Dictionary<string, string>();
			this.Querys = new Dictionary<string, string>
			{
				{
					"aipSdk",
					"CSharp"
				}
			};
			this.Bodys = new Dictionary<string, object>();
			this.Method = "GET";
			this.BodyType = AipHttpRequest.BodyFormat.Formed;
			this.ContentEncoding = Encoding.UTF8;
			ServicePointManager.Expect100Continue = false;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002103 File Offset: 0x00000303
		public AipHttpRequest(string uri) : this()
		{
			this.Uri = new Uri(uri);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002117 File Offset: 0x00000317
		// (set) Token: 0x0600000A RID: 10 RVA: 0x0000211F File Offset: 0x0000031F
		public HttpClient GeneratedRequest { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002128 File Offset: 0x00000328
		public string UriWithQuery
		{
			get
			{
				string str = Utils.ParseQueryString(this.Querys);
				Uri uri = this.Uri;
				return ((uri != null) ? uri.ToString() : null) + "?" + str;
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002160 File Offset: 0x00000360
		public HttpContent ProcessHttpRequest(HttpClient webRequest)
		{
			foreach (KeyValuePair<string, string> keyValuePair in this.Headers)
			{
				webRequest.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
			}
			this.GeneratedRequest = webRequest;
			switch (this.BodyType)
			{
			case AipHttpRequest.BodyFormat.Formed:
			{
				string content = (from pair in this.Bodys
				select pair.Key + "=" + Utils.UriEncode(pair.Value.ToString(), false)).DefaultIfEmpty("").Aggregate((string a, string b) => a + "&" + b);
				webRequest.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
				return new StringContent(content, this.ContentEncoding, "application/x-www-form-urlencoded");
			}
			case AipHttpRequest.BodyFormat.Json:
			{
				string content2 = JsonConvert.SerializeObject(this.Bodys);
				webRequest.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				return new StringContent(content2, this.ContentEncoding, "application/json");
			}
			case AipHttpRequest.BodyFormat.JsonRaw:
			{
				string content3 = JsonConvert.SerializeObject(this.Bodys["RAw"]);
				webRequest.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				return new StringContent(content3, this.ContentEncoding, "application/json");
			}
			default:
				return null;
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022E4 File Offset: 0x000004E4
		public Task<HttpResponseMessage> GenerateDevWebRequest(string token, int timeout)
		{
			this.Querys.Add("access_token", token);
			HttpClient httpClient = new HttpClient
			{
				Timeout = new TimeSpan(0, 0, timeout)
			};
			HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(this.Method), this.UriWithQuery)
			{
				Content = this.ProcessHttpRequest(httpClient)
			};
			return httpClient.SendAsync(request);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002344 File Offset: 0x00000544
		public Task<HttpResponseMessage> GenerateCloudRequest(string ak, string sk, int timeout)
		{
			HttpClient httpClient = new HttpClient
			{
				Timeout = new TimeSpan(0, 0, timeout)
			};
			HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(this.Method), this.UriWithQuery)
			{
				Content = this.ProcessHttpRequest(httpClient)
			};
			Auth.CloudRequest(this, ak, sk);
			return httpClient.SendAsync(request);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002398 File Offset: 0x00000598
		public Task<HttpResponseMessage> GenerateSpeechRequest(int timeout)
		{
			HttpClient httpClient = new HttpClient
			{
				Timeout = new TimeSpan(0, 0, timeout)
			};
			HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(this.Method), this.Uri)
			{
				Content = this.ProcessHttpRequest(httpClient)
			};
			return httpClient.SendAsync(request);
		}

		// Token: 0x04000002 RID: 2
		public const string BodyFormatJsonRawKey = "RAw";

		// Token: 0x04000003 RID: 3
		public Dictionary<string, object> Bodys;

		// Token: 0x04000004 RID: 4
		public AipHttpRequest.BodyFormat BodyType;

		// Token: 0x04000005 RID: 5
		public Encoding ContentEncoding;

		// Token: 0x04000006 RID: 6
		public Dictionary<string, string> Headers;

		// Token: 0x04000007 RID: 7
		public string Method;

		// Token: 0x04000008 RID: 8
		public Dictionary<string, string> Querys;

		// Token: 0x04000009 RID: 9
		public Uri Uri;

		// Token: 0x02000021 RID: 33
		public enum BodyFormat
		{
			// Token: 0x04000128 RID: 296
			Formed,
			// Token: 0x04000129 RID: 297
			Json,
			// Token: 0x0400012A RID: 298
			JsonRaw,
			// Token: 0x0400012B RID: 299
			FileFormed
		}
	}
}
