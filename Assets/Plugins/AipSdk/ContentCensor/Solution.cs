using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x0200001C RID: 28
	public class Solution : Base
	{
		// Token: 0x060001EF RID: 495 RVA: 0x00011DF0 File Offset: 0x0000FFF0
		public Solution(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00011DFA File Offset: 0x0000FFFA
		protected new AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Json
			};
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00011E14 File Offset: 0x00010014
		private JObject ComboPostAction(AipHttpRequest aipReq, string[] scenes, Dictionary<string, object> options)
		{
			aipReq.Bodys.Add("scenes", scenes);
			if (options != null)
			{
				options.Remove("image");
				options.Remove("imageUrl");
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					if (keyValuePair.Value is string)
					{
						dictionary.Add(keyValuePair.Key, keyValuePair.Value);
					}
					else
					{
						dictionary.Add(keyValuePair.Key, JsonConvert.SerializeObject(keyValuePair.Value));
					}
				}
				aipReq.Bodys.Add("scenesConf", dictionary);
			}
			return this.PostAction(aipReq);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00011EE8 File Offset: 0x000100E8
		public JObject Combo(string imageUrl, string[] scenes, Dictionary<string, object> options = null)
		{
			base.CheckNotNull(imageUrl, "imageUrl");
			base.PreAction();
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/api/v1/solution/direct/img_censor");
			aipHttpRequest.Bodys.Add("imgUrl", imageUrl);
			return this.ComboPostAction(aipHttpRequest, scenes, options);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00011F30 File Offset: 0x00010130
		public JObject Combo(byte[] image, string[] scenes, Dictionary<string, object> options = null)
		{
			base.CheckNotNull(image, "image");
			base.PreAction();
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/api/v1/solution/direct/img_censor");
			aipHttpRequest.Bodys.Add("image", Convert.ToBase64String(image));
			return this.ComboPostAction(aipHttpRequest, scenes, options);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00011F7C File Offset: 0x0001017C
		public JObject FaceAudit(byte[][] images, long? configId = null)
		{
			base.CheckNotNull(images, "images");
			base.PreAction();
			AipHttpRequest aipHttpRequest = new AipHttpRequest("https://aip.baidubce.com/rest/2.0/solution/v1/face_audit")
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed
			};
			if (configId != null)
			{
				aipHttpRequest.Bodys.Add("configId", configId);
			}
			aipHttpRequest.Bodys.Add("images", base.ImagesToParams(images));
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00011FF8 File Offset: 0x000101F8
		public JObject FaceAudit(string[] images, long? configId = null)
		{
			base.CheckNotNull(images, "images");
			base.PreAction();
			AipHttpRequest aipHttpRequest = new AipHttpRequest("https://aip.baidubce.com/rest/2.0/solution/v1/face_audit")
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed
			};
			if (configId != null)
			{
				aipHttpRequest.Bodys.Add("configId", configId);
			}
			aipHttpRequest.Bodys.Add("imgUrls", base.StrJoin(images, ","));
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x04000117 RID: 279
		public const string ComboUrl = "https://aip.baidubce.com/api/v1/solution/direct/img_censor";

		// Token: 0x04000118 RID: 280
		public const string FaceAuditUri = "https://aip.baidubce.com/rest/2.0/solution/v1/face_audit";
	}
}
