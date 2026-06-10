using System;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x02000019 RID: 25
	public class Base : AipServiceBase
	{
		// Token: 0x060001DD RID: 477 RVA: 0x000114BC File Offset: 0x0000F6BC
		public Base(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001DE RID: 478 RVA: 0x000114C6 File Offset: 0x0000F6C6
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed
			};
		}
	}
}
