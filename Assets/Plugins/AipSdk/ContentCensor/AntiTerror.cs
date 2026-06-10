using System;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x02000018 RID: 24
	public class AntiTerror : Base
	{
		// Token: 0x060001DB RID: 475 RVA: 0x00011468 File Offset: 0x0000F668
		public AntiTerror(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00011474 File Offset: 0x0000F674
		public JObject Detect(byte[] image)
		{
			base.CheckNotNull(image, "image");
			base.PreAction();
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/antiterror/v1/detect");
			aipHttpRequest.Bodys.Add("image", Convert.ToBase64String(image));
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0400010B RID: 267
		public const string ANTI_TERROR = "https://aip.baidubce.com/rest/2.0/antiterror/v1/detect";
	}
}
