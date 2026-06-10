using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ImageSearch
{
	// Token: 0x02000012 RID: 18
	public class ImageSearch : AipServiceBase
	{
		// Token: 0x06000134 RID: 308 RVA: 0x0000B3B4 File Offset: 0x000095B4
		public ImageSearch(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000B3BE File Offset: 0x000095BE
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000B3E4 File Offset: 0x000095E4
		public JObject MaterielAdd(byte[] image, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/add");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["brief"] = brief;
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

		// Token: 0x06000137 RID: 311 RVA: 0x0000B494 File Offset: 0x00009694
		public JObject MaterielAddUrl(string url, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/add");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["brief"] = brief;
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

		// Token: 0x06000138 RID: 312 RVA: 0x0000B534 File Offset: 0x00009734
		public JObject MaterielSearch(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/search");
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

		// Token: 0x06000139 RID: 313 RVA: 0x0000B5D4 File Offset: 0x000097D4
		public JObject MaterielSearchUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/search");
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

		// Token: 0x0600013A RID: 314 RVA: 0x0000B664 File Offset: 0x00009864
		public JObject MaterielUpdate(byte[] image, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/update");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["brief"] = brief;
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

		// Token: 0x0600013B RID: 315 RVA: 0x0000B714 File Offset: 0x00009914
		public JObject MaterielUpdateUrl(string url, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/update");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["brief"] = brief;
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

		// Token: 0x0600013C RID: 316 RVA: 0x0000B7B4 File Offset: 0x000099B4
		public JObject MaterielUpdateContSign(string contSign, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/update");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
			aipHttpRequest.Bodys["brief"] = brief;
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

		// Token: 0x0600013D RID: 317 RVA: 0x0000B854 File Offset: 0x00009A54
		public JObject MaterielDeleteByImage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/delete");
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

		// Token: 0x0600013E RID: 318 RVA: 0x0000B8F4 File Offset: 0x00009AF4
		public JObject MaterielDeleteByUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/delete");
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

		// Token: 0x0600013F RID: 319 RVA: 0x0000B984 File Offset: 0x00009B84
		public JObject MaterielDeleteBySign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/delete");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x06000140 RID: 320 RVA: 0x0000BA14 File Offset: 0x00009C14
		public JObject SameHqAdd(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/add");
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

		// Token: 0x06000141 RID: 321 RVA: 0x0000BAB4 File Offset: 0x00009CB4
		public JObject SameHqAddUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/add");
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

		// Token: 0x06000142 RID: 322 RVA: 0x0000BB44 File Offset: 0x00009D44
		public JObject SameHqSearch(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/search");
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

		// Token: 0x06000143 RID: 323 RVA: 0x0000BBE4 File Offset: 0x00009DE4
		public JObject SameHqSearchUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/search");
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

		// Token: 0x06000144 RID: 324 RVA: 0x0000BC74 File Offset: 0x00009E74
		public JObject SameHqUpdate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/update");
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

		// Token: 0x06000145 RID: 325 RVA: 0x0000BD14 File Offset: 0x00009F14
		public JObject SameHqUpdateUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/update");
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

		// Token: 0x06000146 RID: 326 RVA: 0x0000BDA4 File Offset: 0x00009FA4
		public JObject SameHqUpdateContSign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/update");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x06000147 RID: 327 RVA: 0x0000BE34 File Offset: 0x0000A034
		public JObject SameHqDeleteByImage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/delete");
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

		// Token: 0x06000148 RID: 328 RVA: 0x0000BED4 File Offset: 0x0000A0D4
		public JObject SameHqDeleteByUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/delete");
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

		// Token: 0x06000149 RID: 329 RVA: 0x0000BF64 File Offset: 0x0000A164
		public JObject SameHqDeleteBySign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/delete");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x0600014A RID: 330 RVA: 0x0000BFF4 File Offset: 0x0000A1F4
		public JObject SimilarAdd(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/add");
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

		// Token: 0x0600014B RID: 331 RVA: 0x0000C094 File Offset: 0x0000A294
		public JObject SimilarAddUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/add");
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

		// Token: 0x0600014C RID: 332 RVA: 0x0000C124 File Offset: 0x0000A324
		public JObject SimilarSearch(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/search");
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

		// Token: 0x0600014D RID: 333 RVA: 0x0000C1C4 File Offset: 0x0000A3C4
		public JObject SimilarSearchUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/search");
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

		// Token: 0x0600014E RID: 334 RVA: 0x0000C254 File Offset: 0x0000A454
		public JObject SimilarUpdate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/update");
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

		// Token: 0x0600014F RID: 335 RVA: 0x0000C2F4 File Offset: 0x0000A4F4
		public JObject SimilarUpdateUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/update");
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

		// Token: 0x06000150 RID: 336 RVA: 0x0000C384 File Offset: 0x0000A584
		public JObject SimilarUpdateContSign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/update");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x06000151 RID: 337 RVA: 0x0000C414 File Offset: 0x0000A614
		public JObject SimilarDeleteByImage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/delete");
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

		// Token: 0x06000152 RID: 338 RVA: 0x0000C4B4 File Offset: 0x0000A6B4
		public JObject SimilarDeleteByUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/delete");
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

		// Token: 0x06000153 RID: 339 RVA: 0x0000C544 File Offset: 0x0000A744
		public JObject SimilarDeleteBySign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/delete");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x06000154 RID: 340 RVA: 0x0000C5D4 File Offset: 0x0000A7D4
		public JObject ProductAdd(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/add");
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

		// Token: 0x06000155 RID: 341 RVA: 0x0000C674 File Offset: 0x0000A874
		public JObject ProductAddUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/add");
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

		// Token: 0x06000156 RID: 342 RVA: 0x0000C704 File Offset: 0x0000A904
		public JObject ProductSearch(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/search");
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

		// Token: 0x06000157 RID: 343 RVA: 0x0000C7A4 File Offset: 0x0000A9A4
		public JObject ProductSearchUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/search");
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

		// Token: 0x06000158 RID: 344 RVA: 0x0000C834 File Offset: 0x0000AA34
		public JObject ProductUpdate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/update");
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

		// Token: 0x06000159 RID: 345 RVA: 0x0000C8D4 File Offset: 0x0000AAD4
		public JObject ProductUpdateUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/update");
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

		// Token: 0x0600015A RID: 346 RVA: 0x0000C964 File Offset: 0x0000AB64
		public JObject ProductUpdateContSign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/update");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x0600015B RID: 347 RVA: 0x0000C9F4 File Offset: 0x0000ABF4
		public JObject ProductDeleteByImage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/delete");
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

		// Token: 0x0600015C RID: 348 RVA: 0x0000CA94 File Offset: 0x0000AC94
		public JObject ProductDeleteByUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/delete");
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

		// Token: 0x0600015D RID: 349 RVA: 0x0000CB24 File Offset: 0x0000AD24
		public JObject ProductDeleteBySign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/delete");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x0600015E RID: 350 RVA: 0x0000CBB4 File Offset: 0x0000ADB4
		public JObject PicturebookAdd(byte[] image, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/add");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["brief"] = brief;
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

		// Token: 0x0600015F RID: 351 RVA: 0x0000CC64 File Offset: 0x0000AE64
		public JObject PicturebookAddUrl(string url, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/add");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["brief"] = brief;
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

		// Token: 0x06000160 RID: 352 RVA: 0x0000CD04 File Offset: 0x0000AF04
		public JObject PicturebookSearch(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/search");
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

		// Token: 0x06000161 RID: 353 RVA: 0x0000CDA4 File Offset: 0x0000AFA4
		public JObject PicturebookSearchUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/search");
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

		// Token: 0x06000162 RID: 354 RVA: 0x0000CE34 File Offset: 0x0000B034
		public JObject PicturebookDeleteByUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/delete");
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

		// Token: 0x06000163 RID: 355 RVA: 0x0000CEC4 File Offset: 0x0000B0C4
		public JObject PicturebookDeleteByImage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/delete");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = image;
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

		// Token: 0x06000164 RID: 356 RVA: 0x0000CF60 File Offset: 0x0000B160
		public JObject PicturebookDeleteByContSign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/delete");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x06000165 RID: 357 RVA: 0x0000CFF0 File Offset: 0x0000B1F0
		public JObject PicturebookUpdateByCountSign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/update");
			aipHttpRequest.Bodys["cont_sign"] = contSign;
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

		// Token: 0x06000166 RID: 358 RVA: 0x0000D080 File Offset: 0x0000B280
		public JObject PicturebookUpdate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/update");
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

		// Token: 0x06000167 RID: 359 RVA: 0x0000D120 File Offset: 0x0000B320
		public JObject PicturebookUpdateUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/update");
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

		// Token: 0x040000A7 RID: 167
		private const string MATERIEL_ADD = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/add";

		// Token: 0x040000A8 RID: 168
		private const string MATERIEL_SEARCH = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/search";

		// Token: 0x040000A9 RID: 169
		private const string MATERIEL_UPDATE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/update";

		// Token: 0x040000AA RID: 170
		private const string MATERIEL_DELETE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/materiel/delete";

		// Token: 0x040000AB RID: 171
		private const string SAME_HQ_ADD = "https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/add";

		// Token: 0x040000AC RID: 172
		private const string SAME_HQ_SEARCH = "https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/search";

		// Token: 0x040000AD RID: 173
		private const string SAME_HQ_UPDATE = "https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/update";

		// Token: 0x040000AE RID: 174
		private const string SAME_HQ_DELETE = "https://aip.baidubce.com/rest/2.0/realtime_search/same_hq/delete";

		// Token: 0x040000AF RID: 175
		private const string SIMILAR_ADD = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/add";

		// Token: 0x040000B0 RID: 176
		private const string SIMILAR_SEARCH = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/search";

		// Token: 0x040000B1 RID: 177
		private const string SIMILAR_UPDATE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/update";

		// Token: 0x040000B2 RID: 178
		private const string SIMILAR_DELETE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/delete";

		// Token: 0x040000B3 RID: 179
		private const string PRODUCT_ADD = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/add";

		// Token: 0x040000B4 RID: 180
		private const string PRODUCT_SEARCH = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/search";

		// Token: 0x040000B5 RID: 181
		private const string PRODUCT_UPDATE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/update";

		// Token: 0x040000B6 RID: 182
		private const string PRODUCT_DELETE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/product/delete";

		// Token: 0x040000B7 RID: 183
		private const string PICTUREBOOK_ADD = "https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/add";

		// Token: 0x040000B8 RID: 184
		private const string PICTUREBOOK_SEARCH = "https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/search";

		// Token: 0x040000B9 RID: 185
		private const string PICTUREBOOK_DELETE = "https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/delete";

		// Token: 0x040000BA RID: 186
		private const string PICTUREBOOK_UPDATE = "https://aip.baidubce.com/rest/2.0/imagesearch/v1/realtime_search/picturebook/update";
	}
}
