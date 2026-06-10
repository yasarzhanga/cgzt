using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x0200001E RID: 30
	public class VideoCensor : Base
	{
		// Token: 0x060001FA RID: 506 RVA: 0x000121CC File Offset: 0x000103CC
		public VideoCensor(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001FB RID: 507 RVA: 0x000121D8 File Offset: 0x000103D8
		public JObject UserDefined(string name, string videoUrl, string extId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/video_censor/v2/user_defined");
			aipHttpRequest.Bodys["name"] = name;
			aipHttpRequest.Bodys["videoUrl"] = videoUrl;
			aipHttpRequest.Bodys["extId"] = extId;
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

		// Token: 0x0400011B RID: 283
		public const string USER_DEFINED = "https://aip.baidubce.com/rest/2.0/solution/v1/video_censor/v2/user_defined";
	}
}
