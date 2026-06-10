using System;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x02000017 RID: 23
	public class AntiPorn : Base
	{
		// Token: 0x060001D8 RID: 472 RVA: 0x000113CC File Offset: 0x0000F5CC
		public AntiPorn(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x000113D8 File Offset: 0x0000F5D8
		public JObject Detect(byte[] image)
		{
			base.CheckNotNull(image, "image");
			base.PreAction();
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/antiporn/v1/detect");
			aipHttpRequest.Bodys.Add("image", Convert.ToBase64String(image));
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00011420 File Offset: 0x0000F620
		public JObject DetectGif(byte[] image)
		{
			base.CheckNotNull(image, "image");
			base.PreAction();
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/antiporn/v1/detect_gif");
			aipHttpRequest.Bodys.Add("image", Convert.ToBase64String(image));
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x04000109 RID: 265
		public const string ANTI_PORN_URL = "https://aip.baidubce.com/rest/2.0/antiporn/v1/detect";

		// Token: 0x0400010A RID: 266
		public const string ANTI_PORN_GIF_URL = "https://aip.baidubce.com/rest/2.0/antiporn/v1/detect_gif";
	}
}
