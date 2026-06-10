using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ImageProcess
{
	// Token: 0x02000013 RID: 19
	public class ImageProcess : AipServiceBase
	{
		// Token: 0x06000168 RID: 360 RVA: 0x0000D1B0 File Offset: 0x0000B3B0
		public ImageProcess(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000D1BA File Offset: 0x0000B3BA
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000D1E0 File Offset: 0x0000B3E0
		public JObject ImageQualityEnhance(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/image_quality_enhance");
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

		// Token: 0x0600016B RID: 363 RVA: 0x0000D280 File Offset: 0x0000B480
		public JObject Dehaze(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/dehaze");
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

		// Token: 0x0600016C RID: 364 RVA: 0x0000D320 File Offset: 0x0000B520
		public JObject ContrastEnhance(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/contrast_enhance");
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

		// Token: 0x0600016D RID: 365 RVA: 0x0000D3C0 File Offset: 0x0000B5C0
		public JObject Colourize(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/colourize");
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

		// Token: 0x0600016E RID: 366 RVA: 0x0000D460 File Offset: 0x0000B660
		public JObject StretchRestore(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/stretch_restore");
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

		// Token: 0x0600016F RID: 367 RVA: 0x0000D500 File Offset: 0x0000B700
		public JObject ImageStyleTransUrl(string url, string option, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/style_trans");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["option"] = option;
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

		// Token: 0x06000170 RID: 368 RVA: 0x0000D5A0 File Offset: 0x0000B7A0
		public JObject ColorEnhanceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/color_enhance");
			aipHttpRequest.Bodys["url"] = url;
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

		// Token: 0x06000171 RID: 369 RVA: 0x0000D630 File Offset: 0x0000B830
		public JObject SelfieAnimeUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/selfie_anime");
			aipHttpRequest.Bodys["url"] = url;
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

		// Token: 0x06000172 RID: 370 RVA: 0x0000D6C0 File Offset: 0x0000B8C0
		public JObject ImageInpainting(byte[] image, ArrayList rectangle, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/inpainting");
			aipHttpRequest.BodyType = AipHttpRequest.BodyFormat.Json;
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["rectangle"] = rectangle;
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

		// Token: 0x06000173 RID: 371 RVA: 0x0000D778 File Offset: 0x0000B978
		public JObject SkySegUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/sky_seg");
			aipHttpRequest.Bodys["url"] = url;
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

		// Token: 0x06000174 RID: 372 RVA: 0x0000D808 File Offset: 0x0000BA08
		public JObject ColorEnhance(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/color_enhance");
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

		// Token: 0x06000175 RID: 373 RVA: 0x0000D8A8 File Offset: 0x0000BAA8
		public JObject SelfieAnime(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/selfie_anime");
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

		// Token: 0x06000176 RID: 374 RVA: 0x0000D948 File Offset: 0x0000BB48
		public JObject ImageStyleTrans(byte[] image, string option, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/style_trans");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["option"] = option;
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

		// Token: 0x06000177 RID: 375 RVA: 0x0000D9F8 File Offset: 0x0000BBF8
		public JObject SkySeg(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/sky_seg");
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

		// Token: 0x06000178 RID: 376 RVA: 0x0000DA98 File Offset: 0x0000BC98
		public JObject ImageDefinitionEnhance(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/image_definition_enhance");
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

		// Token: 0x06000179 RID: 377 RVA: 0x0000DB38 File Offset: 0x0000BD38
		public JObject ImageDefinitionEnhanceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/image_definition_enhance");
			aipHttpRequest.Bodys["url"] = url;
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

		// Token: 0x0600017A RID: 378 RVA: 0x0000DBC8 File Offset: 0x0000BDC8
		public JObject RemoveMoireV1(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/remove_moire");
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

		// Token: 0x0600017B RID: 379 RVA: 0x0000DC5C File Offset: 0x0000BE5C
		public JObject RemoveMoireV1Url(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/remove_moire");
			aipHttpRequest.Bodys["url"] = url;
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

		// Token: 0x0600017C RID: 380 RVA: 0x0000DCEC File Offset: 0x0000BEEC
		public JObject RemoveMoireV1Pdf(byte[] pdf, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/remove_moire");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf);
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

		// Token: 0x0600017D RID: 381 RVA: 0x0000DD80 File Offset: 0x0000BF80
		public JObject CustomizeStylizationV1(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/customize_stylization");
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

		// Token: 0x0600017E RID: 382 RVA: 0x0000DE14 File Offset: 0x0000C014
		public JObject CustomizeStylizationV1Url(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/customize_stylization");
			aipHttpRequest.Bodys["url"] = url;
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

		// Token: 0x0600017F RID: 383 RVA: 0x0000DEA4 File Offset: 0x0000C0A4
		public JObject DocRepairV1(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/doc_repair");
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

		// Token: 0x06000180 RID: 384 RVA: 0x0000DF38 File Offset: 0x0000C138
		public JObject DocRepairV1Url(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/doc_repair");
			aipHttpRequest.Bodys["url"] = url;
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

		// Token: 0x06000181 RID: 385 RVA: 0x0000DFC8 File Offset: 0x0000C1C8
		public JObject DenoiseV1(byte[] image, int option, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/denoise");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["option"] = option;
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

		// Token: 0x06000182 RID: 386 RVA: 0x0000E070 File Offset: 0x0000C270
		public JObject DenoiseV1Url(string url, int option, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-process/v1/denoise");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["option"] = option;
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

		// Token: 0x040000BB RID: 187
		private const string IMAGE_DEFINITION_ENHANCE = "https://aip.baidubce.com/rest/2.0/image-process/v1/image_definition_enhance";

		// Token: 0x040000BC RID: 188
		private const string SKY_SEG = "https://aip.baidubce.com/rest/2.0/image-process/v1/sky_seg";

		// Token: 0x040000BD RID: 189
		private const string IMAGE_INPAINTING = "https://aip.baidubce.com/rest/2.0/image-process/v1/inpainting";

		// Token: 0x040000BE RID: 190
		private const string SELFIE_ANIME = "https://aip.baidubce.com/rest/2.0/image-process/v1/selfie_anime";

		// Token: 0x040000BF RID: 191
		private const string COLOR_ENHANCE = "https://aip.baidubce.com/rest/2.0/image-process/v1/color_enhance";

		// Token: 0x040000C0 RID: 192
		private const string IMAGE_TYLE_TRANS = "https://aip.baidubce.com/rest/2.0/image-process/v1/style_trans";

		// Token: 0x040000C1 RID: 193
		private const string IMAGE_QUALITY_ENHANCE = "https://aip.baidubce.com/rest/2.0/image-process/v1/image_quality_enhance";

		// Token: 0x040000C2 RID: 194
		private const string DEHAZE = "https://aip.baidubce.com/rest/2.0/image-process/v1/dehaze";

		// Token: 0x040000C3 RID: 195
		private const string CONTRAST_ENHANCE = "https://aip.baidubce.com/rest/2.0/image-process/v1/contrast_enhance";

		// Token: 0x040000C4 RID: 196
		private const string COLOURIZE = "https://aip.baidubce.com/rest/2.0/image-process/v1/colourize";

		// Token: 0x040000C5 RID: 197
		private const string STRETCH_RESTORE = "https://aip.baidubce.com/rest/2.0/image-process/v1/stretch_restore";

		// Token: 0x040000C6 RID: 198
		private const string REMOVE_MOIRE_V1 = "https://aip.baidubce.com/rest/2.0/image-process/v1/remove_moire";

		// Token: 0x040000C7 RID: 199
		private const string CUSTOMIZE_STYLIZATION_V1 = "https://aip.baidubce.com/rest/2.0/image-process/v1/customize_stylization";

		// Token: 0x040000C8 RID: 200
		private const string DOC_REPAIR_V1 = "https://aip.baidubce.com/rest/2.0/image-process/v1/doc_repair";

		// Token: 0x040000C9 RID: 201
		private const string DENOISE_V1 = "https://aip.baidubce.com/rest/2.0/image-process/v1/denoise";
	}
}
