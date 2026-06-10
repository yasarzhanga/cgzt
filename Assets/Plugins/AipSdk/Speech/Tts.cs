using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.Speech
{
	// Token: 0x0200000D RID: 13
	public class Tts : Base
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00003805 File Offset: 0x00001A05
		public Tts(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000380F File Offset: 0x00001A0F
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed
			};
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000382C File Offset: 0x00001A2C
		public TtsResponse Synthesis(string text, Dictionary<string, object> options = null)
		{
			base.PreAction();
			base.CheckNotNull(text, "text");
			AipHttpRequest aipHttpRequest = this.DefaultRequest("http://tsn.baidu.com/text2audio");
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			if (!aipHttpRequest.Bodys.ContainsKey("cuid"))
			{
				aipHttpRequest.Bodys["cuid"] = base.Cuid;
			}
			if (!aipHttpRequest.Bodys.ContainsKey("lang"))
			{
				aipHttpRequest.Bodys["lan"] = "zh";
			}
			if (!aipHttpRequest.Bodys.ContainsKey("ctp"))
			{
				aipHttpRequest.Bodys["ctp"] = 1;
			}
			aipHttpRequest.Bodys["tok"] = base.Token;
			aipHttpRequest.Bodys["tex"] = text;
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003954 File Offset: 0x00001B54
		protected new TtsResponse PostAction(AipHttpRequest aipReq)
		{
			TtsResponse ttsResponse = new TtsResponse();
			HttpContent httpContent = base.SendRequetRaw(aipReq);
			if (httpContent.Headers.ContentType.ToString().ToLower() == "application/json")
			{
				string text = Utils.StreamToString(httpContent.ReadAsStreamAsync().Result, Encoding.UTF8);
				try
				{
					JObject jobject = JsonConvert.DeserializeObject(text) as JObject;
					ttsResponse.ErrorCode = (int)jobject["err_no"];
					ttsResponse.ErrorMsg = (string)jobject["err_msg"];
					JToken jtoken;
					if (jobject.TryGetValue("sn", out jtoken))
					{
						ttsResponse.Sn = jtoken.ToString();
					}
					if (jobject.TryGetValue("idx", out jtoken))
					{
						ttsResponse.Idx = int.Parse(jtoken.ToString());
					}
					return ttsResponse;
				}
				catch (Exception ex)
				{
					throw new AipException(ex.Message + ": " + text);
				}
			}
			ttsResponse.ErrorCode = 0;
			ttsResponse.Data = Utils.StreamToBytes(httpContent.ReadAsStreamAsync().Result);
			return ttsResponse;
		}

		// Token: 0x04000029 RID: 41
		private const string UrlTts = "http://tsn.baidu.com/text2audio";
	}
}
