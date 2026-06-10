using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x0200001F RID: 31
	public class VoiceCensor : Base
	{
		// Token: 0x060001FC RID: 508 RVA: 0x0001228C File Offset: 0x0001048C
		public VoiceCensor(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00012298 File Offset: 0x00010498
		public JObject UserDefined(byte[] voice, string fmt, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/voice_censor/v2/user_defined");
			base.CheckNotNull(voice, "voice");
			aipHttpRequest.Bodys["base64"] = Convert.ToBase64String(voice);
			aipHttpRequest.Bodys["fmt"] = fmt;
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

		// Token: 0x060001FE RID: 510 RVA: 0x00012348 File Offset: 0x00010548
		public JObject UserDefinedUrl(string url, string fmt, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/voice_censor/v2/user_defined");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["fmt"] = fmt;
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

		// Token: 0x0400011C RID: 284
		public const string USER_DEFINED = "https://aip.baidubce.com/rest/2.0/solution/v1/voice_censor/v2/user_defined";
	}
}
