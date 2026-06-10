using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.BodyAnalysis
{
	// Token: 0x02000020 RID: 32
	public class Body : AipServiceBase
	{
		// Token: 0x060001FF RID: 511 RVA: 0x000123F4 File Offset: 0x000105F4
		public Body(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x06000200 RID: 512 RVA: 0x000123FE File Offset: 0x000105FE
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00012424 File Offset: 0x00010624
		public JObject BodyAnalysis(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/body_analysis");
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

		// Token: 0x06000202 RID: 514 RVA: 0x000124C4 File Offset: 0x000106C4
		public JObject BodyAttr(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/body_attr");
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

		// Token: 0x06000203 RID: 515 RVA: 0x00012564 File Offset: 0x00010764
		public JObject BodyNum(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/body_num");
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

		// Token: 0x06000204 RID: 516 RVA: 0x00012604 File Offset: 0x00010804
		public JObject Gesture(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/gesture");
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

		// Token: 0x06000205 RID: 517 RVA: 0x000126A4 File Offset: 0x000108A4
		public JObject BodySeg(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/body_seg");
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

		// Token: 0x06000206 RID: 518 RVA: 0x00012744 File Offset: 0x00010944
		public JObject DriverBehavior(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/driver_behavior");
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

		// Token: 0x06000207 RID: 519 RVA: 0x000127E4 File Offset: 0x000109E4
		public JObject BodyTracking(byte[] image, string dynamic, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/body_tracking");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["dynamic"] = dynamic;
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

		// Token: 0x06000208 RID: 520 RVA: 0x00012894 File Offset: 0x00010A94
		public JObject HandAnalysisV1(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/hand_analysis");
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

		// Token: 0x06000209 RID: 521 RVA: 0x00012934 File Offset: 0x00010B34
		public JObject BodyDangerV1(byte[] videoData, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/video-classify/v1/body_danger");
			base.CheckNotNull(videoData, "videoData");
			aipHttpRequest.Bodys["data"] = Convert.ToBase64String(videoData);
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

		// Token: 0x0600020A RID: 522 RVA: 0x000129D4 File Offset: 0x00010BD4
		public JObject FingertipV1(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/fingertip");
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

		// Token: 0x0400011D RID: 285
		private const string BODY_ANALYSIS = "https://aip.baidubce.com/rest/2.0/image-classify/v1/body_analysis";

		// Token: 0x0400011E RID: 286
		private const string BODY_ATTR = "https://aip.baidubce.com/rest/2.0/image-classify/v1/body_attr";

		// Token: 0x0400011F RID: 287
		private const string BODY_NUM = "https://aip.baidubce.com/rest/2.0/image-classify/v1/body_num";

		// Token: 0x04000120 RID: 288
		private const string GESTURE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/gesture";

		// Token: 0x04000121 RID: 289
		private const string BODY_SEG = "https://aip.baidubce.com/rest/2.0/image-classify/v1/body_seg";

		// Token: 0x04000122 RID: 290
		private const string DRIVER_BEHAVIOR = "https://aip.baidubce.com/rest/2.0/image-classify/v1/driver_behavior";

		// Token: 0x04000123 RID: 291
		private const string BODY_TRACKING = "https://aip.baidubce.com/rest/2.0/image-classify/v1/body_tracking";

		// Token: 0x04000124 RID: 292
		private const string HAND_ANALYSIS_V1 = "https://aip.baidubce.com/rest/2.0/image-classify/v1/hand_analysis";

		// Token: 0x04000125 RID: 293
		private const string BODY_DANGER_V1 = "https://aip.baidubce.com/rest/2.0/video-classify/v1/body_danger";

		// Token: 0x04000126 RID: 294
		private const string FINGERTIP_V1 = "https://aip.baidubce.com/rest/2.0/image-classify/v1/fingertip";
	}
}
