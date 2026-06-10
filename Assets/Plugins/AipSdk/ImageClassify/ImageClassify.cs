using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ImageClassify
{
	// Token: 0x02000014 RID: 20
	public class ImageClassify : AipServiceBase
	{
		// Token: 0x06000183 RID: 387 RVA: 0x0000E114 File Offset: 0x0000C314
		public ImageClassify(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000E11E File Offset: 0x0000C31E
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000E144 File Offset: 0x0000C344
		public JObject AdvancedGeneral(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v2/advanced_general");
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

		// Token: 0x06000186 RID: 390 RVA: 0x0000E1E4 File Offset: 0x0000C3E4
		public JObject DishDetect(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v2/dish");
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

		// Token: 0x06000187 RID: 391 RVA: 0x0000E284 File Offset: 0x0000C484
		public JObject CarDetect(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/car");
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

		// Token: 0x06000188 RID: 392 RVA: 0x0000E324 File Offset: 0x0000C524
		public JObject LogoSearch(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v2/logo");
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

		// Token: 0x06000189 RID: 393 RVA: 0x0000E3C4 File Offset: 0x0000C5C4
		public JObject LogoAdd(byte[] image, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/v1/logo/add");
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

		// Token: 0x0600018A RID: 394 RVA: 0x0000E474 File Offset: 0x0000C674
		public JObject LogoDeleteByImage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/v1/logo/delete");
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

		// Token: 0x0600018B RID: 395 RVA: 0x0000E514 File Offset: 0x0000C714
		public JObject LogoDeleteBySign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/realtime_search/v1/logo/delete");
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

		// Token: 0x0600018C RID: 396 RVA: 0x0000E5A4 File Offset: 0x0000C7A4
		public JObject AnimalDetect(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/animal");
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

		// Token: 0x0600018D RID: 397 RVA: 0x0000E644 File Offset: 0x0000C844
		public JObject PlantDetect(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/plant");
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

		// Token: 0x0600018E RID: 398 RVA: 0x0000E6E4 File Offset: 0x0000C8E4
		public JObject ObjectDetect(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/object_detect");
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

		// Token: 0x0600018F RID: 399 RVA: 0x0000E784 File Offset: 0x0000C984
		public JObject Landmark(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/landmark");
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

		// Token: 0x06000190 RID: 400 RVA: 0x0000E824 File Offset: 0x0000CA24
		public JObject Flower(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/flower");
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

		// Token: 0x06000191 RID: 401 RVA: 0x0000E8C4 File Offset: 0x0000CAC4
		public JObject Ingredient(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/classify/ingredient");
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

		// Token: 0x06000192 RID: 402 RVA: 0x0000E964 File Offset: 0x0000CB64
		public JObject Redwine(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/redwine");
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

		// Token: 0x06000193 RID: 403 RVA: 0x0000EA04 File Offset: 0x0000CC04
		public JObject RedwineUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/redwine");
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

		// Token: 0x06000194 RID: 404 RVA: 0x0000EA94 File Offset: 0x0000CC94
		public JObject Currency(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/currency");
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

		// Token: 0x06000195 RID: 405 RVA: 0x0000EB34 File Offset: 0x0000CD34
		public JObject CurrencyUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/currency");
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

		// Token: 0x06000196 RID: 406 RVA: 0x0000EBC4 File Offset: 0x0000CDC4
		public JObject MultObjectDetect(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/multi_object_detect");
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

		// Token: 0x06000197 RID: 407 RVA: 0x0000EC64 File Offset: 0x0000CE64
		public JObject DishSearch(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/dish/search");
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

		// Token: 0x06000198 RID: 408 RVA: 0x0000ED04 File Offset: 0x0000CF04
		public JObject DishDeleteByImage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/dish/delete");
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

		// Token: 0x06000199 RID: 409 RVA: 0x0000EDA4 File Offset: 0x0000CFA4
		public JObject Combination(byte[] image, string[] scenes, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/api/v1/solution/direct/imagerecognition/combination");
			aipHttpRequest.BodyType = AipHttpRequest.BodyFormat.Json;
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			base.CheckNotNull(scenes, "scenes");
			aipHttpRequest.Bodys["scenes"] = scenes;
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

		// Token: 0x0600019A RID: 410 RVA: 0x0000EE68 File Offset: 0x0000D068
		public JObject CombinationUrl(string url, string[] scenes, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/api/v1/solution/direct/imagerecognition/combination");
			aipHttpRequest.BodyType = AipHttpRequest.BodyFormat.Json;
			aipHttpRequest.Bodys["imgUrl"] = url;
			base.CheckNotNull(scenes, "scenes");
			aipHttpRequest.Bodys["scenes"] = scenes;
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

		// Token: 0x0600019B RID: 411 RVA: 0x0000EF1C File Offset: 0x0000D11C
		public JObject DishDeleteByContSign(string contSign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/dish/delete");
			aipHttpRequest.Bodys["contSign"] = contSign;
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

		// Token: 0x0600019C RID: 412 RVA: 0x0000EFAC File Offset: 0x0000D1AC
		public JObject Dishadd(byte[] image, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/dish/add");
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

		// Token: 0x0600019D RID: 413 RVA: 0x0000F05C File Offset: 0x0000D25C
		public JObject VehicleAttr(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_attr");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000F0F4 File Offset: 0x0000D2F4
		public JObject VehicleAttrUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_attr");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000F188 File Offset: 0x0000D388
		public JObject VehicleDetectHigh(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_detect_high");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000F220 File Offset: 0x0000D420
		public JObject VehicleDetectHighUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_detect_high");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000F2B4 File Offset: 0x0000D4B4
		public JObject CarDetectUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/car");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000F348 File Offset: 0x0000D548
		public JObject VehicleDetectUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_detect");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000F3DC File Offset: 0x0000D5DC
		public JObject VehicleSeg(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_seg");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000F474 File Offset: 0x0000D674
		public JObject VehicleDamage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_damage");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000F50C File Offset: 0x0000D70C
		public JObject TrafficFlow(byte[] image, int case_id, string case_init, string area, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/traffic_flow");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			base.CheckNotNull(case_id, "case_id");
			aipHttpRequest.Bodys["case_id"] = case_id;
			base.CheckNotNull(case_init, "case_init");
			aipHttpRequest.Bodys["case_init"] = case_init;
			base.CheckNotNull(area, "area");
			aipHttpRequest.Bodys["area"] = area;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000F60C File Offset: 0x0000D80C
		public JObject TrafficFlowUrl(string url, int case_id, string case_init, string area, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/traffic_flow");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			base.CheckNotNull(case_id, "case_id");
			aipHttpRequest.Bodys["case_id"] = case_id;
			base.CheckNotNull(case_init, "case_init");
			aipHttpRequest.Bodys["case_init"] = case_init;
			base.CheckNotNull(area, "area");
			aipHttpRequest.Bodys["area"] = area;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000F704 File Offset: 0x0000D904
		public JObject VehicleDetect(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_detect");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000F79C File Offset: 0x0000D99C
		public JObject RedwineAddV1Image(byte[] image, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/add");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["brief"] = brief;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000F848 File Offset: 0x0000DA48
		public JObject RedwineAddV1Url(string url, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/add");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["brief"] = brief;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000F8E0 File Offset: 0x0000DAE0
		public JObject RedwineSearchV1Image(byte[] image, string custom_lib, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/search");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["custom_lib"] = custom_lib;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000F98C File Offset: 0x0000DB8C
		public JObject RedwineSearchV1Url(string url, string custom_lib, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/search");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["custom_lib"] = custom_lib;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000FA24 File Offset: 0x0000DC24
		public JObject RedwineDeleteV1Image(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/delete");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000FABC File Offset: 0x0000DCBC
		public JObject RedwineDeleteV1Sign(string sign, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/delete");
			aipHttpRequest.Bodys["cont_sign_list"] = sign;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000FB44 File Offset: 0x0000DD44
		public JObject RedwineUpdateV1Image(byte[] image, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/update");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["brief"] = brief;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000FBF0 File Offset: 0x0000DDF0
		public JObject RedwineUpdateV1Url(string url, string brief, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/update");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["brief"] = brief;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000FC88 File Offset: 0x0000DE88
		public JObject VehicleAttrClassifyV2Image(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v2/vehicle_attr");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000FD20 File Offset: 0x0000DF20
		public JObject VehicleAttrClassifyV2Url(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/image-classify/v2/vehicle_attr");
			aipHttpRequest.Bodys["url"] = url;
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x040000CA RID: 202
		private const string VEHICLE_DETECT = "https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_detect";

		// Token: 0x040000CB RID: 203
		private const string TRAFFIC_FLOW = "https://aip.baidubce.com/rest/2.0/image-classify/v1/traffic_flow";

		// Token: 0x040000CC RID: 204
		private const string VEHICLE_DAMAGE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_damage";

		// Token: 0x040000CD RID: 205
		private const string VEHICLE_SEG = "https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_seg";

		// Token: 0x040000CE RID: 206
		private const string VEHICLE_DETECT_HIGH = "https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_detect_high";

		// Token: 0x040000CF RID: 207
		private const string VEHICLE_ATTR = "https://aip.baidubce.com/rest/2.0/image-classify/v1/vehicle_attr";

		// Token: 0x040000D0 RID: 208
		private const string DISHADD = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/dish/add";

		// Token: 0x040000D1 RID: 209
		private const string COMBINATION = "https://aip.baidubce.com/api/v1/solution/direct/imagerecognition/combination";

		// Token: 0x040000D2 RID: 210
		private const string DISHDELETE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/dish/delete";

		// Token: 0x040000D3 RID: 211
		private const string DISHSEARCH = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/dish/search";

		// Token: 0x040000D4 RID: 212
		private const string MULT_OBJECT_DETECT = "https://aip.baidubce.com/rest/2.0/image-classify/v1/multi_object_detect";

		// Token: 0x040000D5 RID: 213
		private const string ADVANCED_GENERAL = "https://aip.baidubce.com/rest/2.0/image-classify/v2/advanced_general";

		// Token: 0x040000D6 RID: 214
		private const string DISH_DETECT = "https://aip.baidubce.com/rest/2.0/image-classify/v2/dish";

		// Token: 0x040000D7 RID: 215
		private const string CAR_DETECT = "https://aip.baidubce.com/rest/2.0/image-classify/v1/car";

		// Token: 0x040000D8 RID: 216
		private const string LOGO_SEARCH = "https://aip.baidubce.com/rest/2.0/image-classify/v2/logo";

		// Token: 0x040000D9 RID: 217
		private const string LOGO_ADD = "https://aip.baidubce.com/rest/2.0/realtime_search/v1/logo/add";

		// Token: 0x040000DA RID: 218
		private const string LOGO_DELETE = "https://aip.baidubce.com/rest/2.0/realtime_search/v1/logo/delete";

		// Token: 0x040000DB RID: 219
		private const string ANIMAL_DETECT = "https://aip.baidubce.com/rest/2.0/image-classify/v1/animal";

		// Token: 0x040000DC RID: 220
		private const string PLANT_DETECT = "https://aip.baidubce.com/rest/2.0/image-classify/v1/plant";

		// Token: 0x040000DD RID: 221
		private const string OBJECT_DETECT = "https://aip.baidubce.com/rest/2.0/image-classify/v1/object_detect";

		// Token: 0x040000DE RID: 222
		private const string LANDMARK = "https://aip.baidubce.com/rest/2.0/image-classify/v1/landmark";

		// Token: 0x040000DF RID: 223
		private const string FLOWER = "https://aip.baidubce.com/rest/2.0/image-classify/v1/flower";

		// Token: 0x040000E0 RID: 224
		private const string INGREDIENT = "https://aip.baidubce.com/rest/2.0/image-classify/v1/classify/ingredient";

		// Token: 0x040000E1 RID: 225
		private const string REDWINE = "https://aip.baidubce.com/rest/2.0/image-classify/v1/redwine";

		// Token: 0x040000E2 RID: 226
		private const string CURRENCY = "https://aip.baidubce.com/rest/2.0/image-classify/v1/currency";

		// Token: 0x040000E3 RID: 227
		private const string CAR_CLASSIFY_V1 = "https://aip.baidubce.com/rest/2.0/vis-classify/v1/car";

		// Token: 0x040000E4 RID: 228
		private const string REDWINE_ADD_V1 = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/add";

		// Token: 0x040000E5 RID: 229
		private const string REDWINE_SEARCH_V1 = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/search";

		// Token: 0x040000E6 RID: 230
		private const string REDWINE_DELETE_V1 = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/delete";

		// Token: 0x040000E7 RID: 231
		private const string REDWINE_UPDATE_V1 = "https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/redwine/update";

		// Token: 0x040000E8 RID: 232
		private const string VEHICLE_ATTR_CLASSIFY_V2 = "https://aip.baidubce.com/rest/2.0/image-classify/v2/vehicle_attr";
	}
}
