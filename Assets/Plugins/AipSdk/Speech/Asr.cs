using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.Speech
{
	// Token: 0x02000008 RID: 8
	public class Asr : Base
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00002ED3 File Offset: 0x000010D3
		public Asr(string appId, string apiKey, string secretKey) : base(appId, apiKey, secretKey)
		{
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002EDE File Offset: 0x000010DE
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Json
			};
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002EF8 File Offset: 0x000010F8
		public JObject Recognize(byte[] data, string format, int rate, Dictionary<string, object> options = null)
		{
			base.PreAction();
			base.CheckNotNull(data, "data");
			base.CheckNotNull(format, "format");
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://vop.baidu.com/server_api");
			aipHttpRequest.Bodys["format"] = format;
			aipHttpRequest.Bodys["rate"] = rate;
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
			if (!aipHttpRequest.Bodys.ContainsKey("channel"))
			{
				aipHttpRequest.Bodys["channel"] = 1;
			}
			aipHttpRequest.Bodys["len"] = data.Length;
			aipHttpRequest.Bodys["speech"] = Convert.ToBase64String(data);
			aipHttpRequest.Bodys["token"] = base.Token;
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000304C File Offset: 0x0000124C
		public JObject Recognize(string url, string callback, string format, int rate, Dictionary<string, object> options = null)
		{
			base.PreAction();
			base.CheckNotNull(url, "url");
			base.CheckNotNull(format, "format");
			base.CheckNotNull(callback, "callback");
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://vop.baidu.com/server_api");
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["callback"] = callback;
			aipHttpRequest.Bodys["format"] = format;
			aipHttpRequest.Bodys["rate"] = rate;
			if (!aipHttpRequest.Bodys.ContainsKey("cuid"))
			{
				aipHttpRequest.Bodys["cuid"] = base.Cuid;
			}
			if (!aipHttpRequest.Bodys.ContainsKey("channel"))
			{
				aipHttpRequest.Bodys["channel"] = 1;
			}
			aipHttpRequest.Bodys["token"] = base.Token;
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000031A0 File Offset: 0x000013A0
		public JObject RecognizePro(byte[] data, string format, int rate = 16000, int devPid = 80001, Dictionary<string, object> options = null)
		{
			base.PreAction();
			base.CheckNotNull(data, "data");
			base.CheckNotNull(format, "format");
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://vop.baidu.com/pro_api");
			aipHttpRequest.Bodys["format"] = format;
			aipHttpRequest.Bodys["rate"] = rate;
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
			if (!aipHttpRequest.Bodys.ContainsKey("channel"))
			{
				aipHttpRequest.Bodys["channel"] = 1;
			}
			aipHttpRequest.Bodys["len"] = data.Length;
			aipHttpRequest.Bodys["speech"] = Convert.ToBase64String(data);
			aipHttpRequest.Bodys["token"] = base.Token;
			aipHttpRequest.Bodys["dev_pid"] = devPid;
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003308 File Offset: 0x00001508
		public JObject Recognize(Stream speech, string cuid, string format, int rate, int pid)
		{
			string value = JsonConvert.SerializeObject(new Dictionary<string, object>
			{
				{
					"apikey",
					base.ApiKey
				},
				{
					"secretkey",
					base.SecretKey
				},
				{
					"appid",
					int.Parse(base.AppId)
				},
				{
					"cuid",
					cuid
				},
				{
					"sample_rate",
					rate
				},
				{
					"format",
					format
				},
				{
					"task_id",
					pid
				}
			}, 0);
			byte[] array = new TlvPacket(TlvType.AsrBegin, value).ToBytes();
			byte[] array2 = new byte[2560];
			int i = speech.Read(array2, 0, 2560);
			if (i == 0)
			{
				throw new AipException("Speech bytes stream empty");
			}
			string str = string.Format("{0}{1}", Guid.NewGuid(), Guid.NewGuid()).Replace("-", "");
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://vop.baidu.com/open/asr?id=" + str);
			httpWebRequest.Method = "POST";
			httpWebRequest.ReadWriteTimeout = base.Timeout;
			httpWebRequest.Timeout = base.Timeout;
			httpWebRequest.SendChunked = true;
			httpWebRequest.AllowWriteStreamBuffering = false;
			httpWebRequest.ContentType = "application/octet-stream";
			Stream requestStream = httpWebRequest.GetRequestStream();
			requestStream.Write(array, 0, array.Length);
			while (i > 0)
			{
				byte[] array3 = new TlvPacket(TlvType.AsrData, array2, i).ToBytes();
				requestStream.Write(array3, 0, array3.Length);
				i = speech.Read(array2, 0, 2560);
			}
			byte[] array4 = new TlvPacket(TlvType.AsrEnd).ToBytes();
			requestStream.Write(array4, 0, array4.Length);
			requestStream.Close();
			List<TlvPacket> list = TlvPacket.ParseFromBytes(Utils.StreamToBytes(((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream())).ToList<TlvPacket>();
			if (list.Count == 0)
			{
				throw new AipException("Server return empty");
			}
			string text = "";
			JObject result;
			try
			{
				text = Encoding.UTF8.GetString(list[0].V);
				result = (JsonConvert.DeserializeObject(text) as JObject);
			}
			catch (Exception ex)
			{
				throw new AipException(ex.Message + text);
			}
			return result;
		}

		// Token: 0x04000017 RID: 23
		public const string UrlAsr = "https://vop.baidu.com/server_api";

		// Token: 0x04000018 RID: 24
		public const string UrlAsrPro = "https://vop.baidu.com/pro_api";

		// Token: 0x04000019 RID: 25
		public const string UrlAsrStream = "https://vop.baidu.com/open/asr";

		// Token: 0x0400001A RID: 26
		private const int AsrStreamingChunkDataSize = 2560;
	}
}
