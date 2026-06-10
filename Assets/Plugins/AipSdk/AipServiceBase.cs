using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip
{
	// Token: 0x02000004 RID: 4
	public abstract class AipServiceBase
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000023E4 File Offset: 0x000005E4
		protected AipServiceBase(string apiKey, string secretKey) : this("", apiKey, secretKey)
		{
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000023F4 File Offset: 0x000005F4
		protected AipServiceBase(string appId, string apiKey, string secretKey)
		{
			this.AppId = appId;
			this.ApiKey = apiKey;
			this.SecretKey = secretKey;
			this.ExpireAt = DateTime.Now;
			this.DebugLog = false;
			this.Timeout = 60000;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002444 File Offset: 0x00000644
		// (set) Token: 0x06000013 RID: 19 RVA: 0x0000244C File Offset: 0x0000064C
		protected string Token { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002455 File Offset: 0x00000655
		// (set) Token: 0x06000015 RID: 21 RVA: 0x0000245D File Offset: 0x0000065D
		protected DateTime ExpireAt { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002466 File Offset: 0x00000666
		// (set) Token: 0x06000017 RID: 23 RVA: 0x0000246E File Offset: 0x0000066E
		public string AppId { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00002477 File Offset: 0x00000677
		// (set) Token: 0x06000019 RID: 25 RVA: 0x0000247F File Offset: 0x0000067F
		public string ApiKey { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002488 File Offset: 0x00000688
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002490 File Offset: 0x00000690
		public string SecretKey { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002499 File Offset: 0x00000699
		// (set) Token: 0x0600001D RID: 29 RVA: 0x000024A1 File Offset: 0x000006A1
		public bool DebugLog { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000024AA File Offset: 0x000006AA
		// (set) Token: 0x0600001F RID: 31 RVA: 0x000024B2 File Offset: 0x000006B2
		public int Timeout { get; set; }

		// Token: 0x06000020 RID: 32 RVA: 0x000024BC File Offset: 0x000006BC
		protected virtual void DoAuthorization()
		{
			object authLock = this.AuthLock;
			lock (authLock)
			{
				if (this.NeetAuth())
				{
					JObject jobject = Auth.OpenApiFetchToken(this.ApiKey, this.SecretKey, false, false);
					if (jobject != null)
					{
						this.ExpireAt = DateTime.Now.AddSeconds((double)((int)jobject["expires_in"] - 1));
						if (jobject["scope"].ToString().Split(new char[]
						{
							' '
						}).ToList<string>().Exists((string v) => Consts.AipScopes.Contains(v)))
						{
							this.IsDev = true;
							this.Token = (string)jobject["access_token"];
						}
					}
					this.HasDoneAuthoried = true;
				}
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000025B8 File Offset: 0x000007B8
		protected virtual bool NeetAuth()
		{
			return !this.HasDoneAuthoried || (this.IsDev && DateTime.Now >= this.ExpireAt);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000025E2 File Offset: 0x000007E2
		protected void PreAction()
		{
			this.DoAuthorization();
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000025EC File Offset: 0x000007EC
		protected virtual JObject PostAction(AipHttpRequest aipReq)
		{
			string text = this.SendRequet(aipReq);
			JObject jobject;
			try
			{
				jobject = (JsonConvert.DeserializeObject(text) as JObject);
			}
			catch (Exception ex)
			{
				throw new AipException(ex.Message + ": " + text);
			}
			if (jobject == null)
			{
				throw new AipException("Empty response, please check input");
			}
			return jobject;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002644 File Offset: 0x00000844
		protected virtual Task<HttpResponseMessage> GenerateWebRequest(AipHttpRequest aipRequest)
		{
			if (!this.IsDev)
			{
				return aipRequest.GenerateCloudRequest(this.ApiKey, this.SecretKey, this.Timeout);
			}
			return aipRequest.GenerateDevWebRequest(this.Token, this.Timeout);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000267B File Offset: 0x0000087B
		protected string SendRequet(AipHttpRequest aipRequest)
		{
			return Utils.StreamToString(this.SendRequetRaw(aipRequest).ReadAsStreamAsync().Result, aipRequest.ContentEncoding);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000269C File Offset: 0x0000089C
		protected HttpContent SendRequetRaw(AipHttpRequest aipRequest)
		{
			Task<HttpResponseMessage> task = this.GenerateWebRequest(aipRequest);
			HttpContent content;
			try
			{
				content = task.Result.Content;
			}
			catch (WebException ex)
			{
				throw new AipException((int)ex.Status, ex.Message);
			}
			if (!task.Result.IsSuccessStatusCode)
			{
				throw new AipException((int)task.Result.StatusCode, "Server response code：" + ((int)task.Result.StatusCode).ToString());
			}
			return content;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002720 File Offset: 0x00000920
		protected void CheckNotNull(object obj, string name)
		{
			if (obj is string && string.IsNullOrWhiteSpace(obj.ToString()))
			{
				throw new AipException(name + " cannot be empty.");
			}
			if (obj == null)
			{
				throw new AipException(name + " cannot be null.");
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000275C File Offset: 0x0000095C
		protected string ImagesToParams(IEnumerable<byte[]> images)
		{
			return images.Select(new Func<byte[], string>(Convert.ToBase64String)).Aggregate((string a, string b) => a + "," + b);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002794 File Offset: 0x00000994
		protected string StrJoin(IEnumerable<string> data, string sep = ",")
		{
			return data.Aggregate((string a, string b) => a + sep + b);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000027C0 File Offset: 0x000009C0
		protected virtual void Log(string msg)
		{
			if (this.DebugLog)
			{
				string arg = DateTime.Now.ToString("[yyyyMMdd HH:mm:ss]");
				Console.WriteLine("{0} [{1}] {2}", arg, base.GetType().FullName, msg);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002800 File Offset: 0x00000A00
		public JObject Report(IEnumerable<Dictionary<string, object>> data)
		{
			AipHttpRequest aipHttpRequest = new AipHttpRequest("https://aip.baidubce.com/rpc/2.0/feedback/v1/report")
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Json
			};
			this.CheckNotNull(data, "data");
			aipHttpRequest.Bodys["feedback"] = data;
			this.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0400000B RID: 11
		protected readonly object AuthLock = new object();

		// Token: 0x0400000C RID: 12
		protected volatile bool HasDoneAuthoried;

		// Token: 0x0400000D RID: 13
		protected volatile bool IsDev;

		// Token: 0x02000023 RID: 35
		public class Type
		{
			// Token: 0x0600020F RID: 527 RVA: 0x00012ABB File Offset: 0x00010CBB
			public Type(string url)
			{
				this.Url = url;
			}

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x06000210 RID: 528 RVA: 0x00012ACA File Offset: 0x00010CCA
			// (set) Token: 0x06000211 RID: 529 RVA: 0x00012AD2 File Offset: 0x00010CD2
			public string Url { get; set; }
		}
	}
}
