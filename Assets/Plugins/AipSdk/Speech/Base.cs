using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.Speech
{
	// Token: 0x02000009 RID: 9
	public class Base : AipServiceBase
	{
		// Token: 0x06000040 RID: 64 RVA: 0x0000354C File Offset: 0x0000174C
		public Base(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
			this.IsDev = true;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000355F File Offset: 0x0000175F
		public Base(string appId, string apiKey, string secretKey) : base(appId, apiKey, secretKey)
		{
			this.IsDev = true;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00003573 File Offset: 0x00001773
		protected string Cuid
		{
			get
			{
				return Utils.Md5(base.Token);
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003580 File Offset: 0x00001780
		protected override void DoAuthorization()
		{
			object authLock = this.AuthLock;
			lock (authLock)
			{
				if (this.NeetAuth())
				{
					JObject jobject = Auth.OpenApiFetchToken(base.ApiKey, base.SecretKey, true, false);
					base.ExpireAt = DateTime.Now.AddSeconds((double)((int)jobject["expires_in"] - 1));
					this.IsDev = true;
					base.Token = (string)jobject["access_token"];
					this.HasDoneAuthoried = true;
				}
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003628 File Offset: 0x00001828
		protected override Task<HttpResponseMessage> GenerateWebRequest(AipHttpRequest aipRequest)
		{
			return aipRequest.GenerateSpeechRequest(base.Timeout);
		}
	}
}
