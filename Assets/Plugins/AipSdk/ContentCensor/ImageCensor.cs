using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x0200001B RID: 27
	public class ImageCensor : Base
	{
		// Token: 0x060001EC RID: 492 RVA: 0x00011CA8 File Offset: 0x0000FEA8
		public ImageCensor(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00011CB4 File Offset: 0x0000FEB4
		public JObject UserDefined(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/img_censor/v2/user_defined");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
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

		// Token: 0x060001EE RID: 494 RVA: 0x00011D54 File Offset: 0x0000FF54
		public JObject UserDefinedUrl(string imageUrl, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/img_censor/v2/user_defined");
			base.CheckNotNull(imageUrl, "imageUrl");
			aipHttpRequest.Bodys["imgUrl"] = imageUrl;
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

		// Token: 0x04000116 RID: 278
		public const string USER_DEFINED = "https://aip.baidubce.com/rest/2.0/solution/v1/img_censor/v2/user_defined";
	}
}
