using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.EasyDL
{
	// Token: 0x02000016 RID: 22
	public class EasyDL : AipServiceBase
	{
		// Token: 0x060001D4 RID: 468 RVA: 0x0001127C File Offset: 0x0000F47C
		public EasyDL(string appId, string apiKey, string secretKey) : base(appId, apiKey, secretKey)
		{
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00011287 File Offset: 0x0000F487
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Json,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x000112AC File Offset: 0x0000F4AC
		public JObject requestImage(string fullurl, byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest(fullurl);
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

		// Token: 0x060001D7 RID: 471 RVA: 0x0001133C File Offset: 0x0000F53C
		public JObject requestSound(string fullurl, byte[] sound, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest(fullurl);
			aipHttpRequest.Bodys["sound"] = Convert.ToBase64String(sound);
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
	}
}
