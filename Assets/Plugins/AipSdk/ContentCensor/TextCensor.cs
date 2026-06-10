using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x0200001D RID: 29
	public class TextCensor : AipServiceBase
	{
		// Token: 0x060001F6 RID: 502 RVA: 0x00012076 File Offset: 0x00010276
		public TextCensor(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00012080 File Offset: 0x00010280
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed,
				ContentEncoding = Encoding.GetEncoding("UTF-8")
			};
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x000120AC File Offset: 0x000102AC
		[Obsolete("AntiSpam is deprecated.")]
		public JObject AntiSpam(string content, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/antispam/v2/spam");
			aipHttpRequest.Bodys["content"] = content;
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

		// Token: 0x060001F9 RID: 505 RVA: 0x0001213C File Offset: 0x0001033C
		public JObject TextCensorUserDefined(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/text_censor/v2/user_defined");
			aipHttpRequest.Bodys["text"] = text;
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

		// Token: 0x04000119 RID: 281
		private const string ANTI_SPAM = "https://aip.baidubce.com/rest/2.0/antispam/v2/spam";

		// Token: 0x0400011A RID: 282
		private const string USER_DEFINED = "https://aip.baidubce.com/rest/2.0/solution/v1/text_censor/v2/user_defined";
	}
}
