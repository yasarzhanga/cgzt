using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.MachineTranslation
{
	// Token: 0x02000010 RID: 16
	public class MachineTranslation : AipServiceBase
	{
		// Token: 0x06000124 RID: 292 RVA: 0x0000AC18 File Offset: 0x00008E18
		public MachineTranslation(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000AC22 File Offset: 0x00008E22
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000AC47 File Offset: 0x00008E47
		protected AipHttpRequest JsonRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Json,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000AC6C File Offset: 0x00008E6C
		public JObject TexttransV1(string from, string to, string q, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.JsonRequest("https://aip.baidubce.com/rpc/2.0/mt/texttrans/v1");
			aipHttpRequest.Bodys["from"] = from;
			aipHttpRequest.Bodys["to"] = to;
			aipHttpRequest.Bodys["q"] = q;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000AD20 File Offset: 0x00008F20
		public JObject TexttransWithDictV1(string from, string to, string q, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.JsonRequest("https://aip.baidubce.com/rpc/2.0/mt/texttrans-with-dict/v1");
			aipHttpRequest.Bodys["from"] = from;
			aipHttpRequest.Bodys["to"] = to;
			aipHttpRequest.Bodys["q"] = q;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000ADD4 File Offset: 0x00008FD4
		public JObject DocTranslationCreateV2(string from, string to, Dictionary<string, object> input, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.JsonRequest("https://aip.baidubce.com/rpc/2.0/mt/v2/doc-translation/create");
			aipHttpRequest.Bodys["from"] = from;
			aipHttpRequest.Bodys["to"] = to;
			aipHttpRequest.Bodys["input"] = input;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000AE88 File Offset: 0x00009088
		public JObject DocTranslationQueryV2(string id, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.JsonRequest("https://aip.baidubce.com/rpc/2.0/mt/v2/doc-translation/query");
			aipHttpRequest.Bodys["id"] = id;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000AF18 File Offset: 0x00009118
		public JObject SpeechTranslationV2(string from, string to, byte[] voice, string format, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.JsonRequest("https://aip.baidubce.com/rpc/2.0/mt/v2/speech-translation");
			aipHttpRequest.Bodys["from"] = from;
			aipHttpRequest.Bodys["to"] = to;
			aipHttpRequest.Bodys["voice"] = Convert.ToBase64String(voice);
			aipHttpRequest.Bodys["format"] = format;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0400009B RID: 155
		private const string PICTRANS_V1 = "https://aip.baidubce.com/file/2.0/mt/pictrans/v1";

		// Token: 0x0400009C RID: 156
		private const string TEXTTRANS_V1 = "https://aip.baidubce.com/rpc/2.0/mt/texttrans/v1";

		// Token: 0x0400009D RID: 157
		private const string TEXTTRANS_WITH_DICT_V1 = "https://aip.baidubce.com/rpc/2.0/mt/texttrans-with-dict/v1";

		// Token: 0x0400009E RID: 158
		private const string DOC_TRANSLATION_CREATE_V2 = "https://aip.baidubce.com/rpc/2.0/mt/v2/doc-translation/create";

		// Token: 0x0400009F RID: 159
		private const string DOC_TRANSLATION_QUERY_V2 = "https://aip.baidubce.com/rpc/2.0/mt/v2/doc-translation/query";

		// Token: 0x040000A0 RID: 160
		private const string SPEECH_TRANSLATION_V2 = "https://aip.baidubce.com/rpc/2.0/mt/v2/speech-translation";
	}
}
