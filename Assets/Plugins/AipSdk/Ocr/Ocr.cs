using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.Ocr
{
	// Token: 0x0200000E RID: 14
	public class Ocr : AipServiceBase
	{
		// Token: 0x06000061 RID: 97 RVA: 0x00003A64 File Offset: 0x00001C64
		public Ocr(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003A6E File Offset: 0x00001C6E
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed
			};
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003A88 File Offset: 0x00001C88
		public JObject GeneralBasic(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic");
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

		// Token: 0x06000064 RID: 100 RVA: 0x00003B28 File Offset: 0x00001D28
		public JObject GeneralBasicUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic");
			base.CheckNotNull(url, "url");
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

		// Token: 0x06000065 RID: 101 RVA: 0x00003BC4 File Offset: 0x00001DC4
		public JObject GeneralBasicPdf(byte[] pdf, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic");
			base.CheckNotNull(pdf, "pdf");
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

		// Token: 0x06000066 RID: 102 RVA: 0x00003C64 File Offset: 0x00001E64
		public JObject AccurateBasic(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/accurate_basic");
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

		// Token: 0x06000067 RID: 103 RVA: 0x00003D04 File Offset: 0x00001F04
		public JObject AccurateBasicUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/accurate_basic");
			base.CheckNotNull(url, "url");
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

		// Token: 0x06000068 RID: 104 RVA: 0x00003DA0 File Offset: 0x00001FA0
		public JObject AccurateBasicPdf(byte[] pdf, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/accurate_basic");
			base.CheckNotNull(pdf, "pdf");
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

		// Token: 0x06000069 RID: 105 RVA: 0x00003E40 File Offset: 0x00002040
		public JObject General(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/general");
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

		// Token: 0x0600006A RID: 106 RVA: 0x00003EE0 File Offset: 0x000020E0
		public JObject GeneralUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/general");
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

		// Token: 0x0600006B RID: 107 RVA: 0x00003F70 File Offset: 0x00002170
		public JObject GeneralPdf(byte[] pdf, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/general");
			base.CheckNotNull(pdf, "pdf");
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

		// Token: 0x0600006C RID: 108 RVA: 0x00004010 File Offset: 0x00002210
		public JObject Accurate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/accurate");
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

		// Token: 0x0600006D RID: 109 RVA: 0x000040B0 File Offset: 0x000022B0
		public JObject AccurateUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/accurate");
			base.CheckNotNull(url, "url");
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

		// Token: 0x0600006E RID: 110 RVA: 0x0000414C File Offset: 0x0000234C
		public JObject AccuratePdf(byte[] pdf, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/accurate");
			base.CheckNotNull(pdf, "pdf");
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

		// Token: 0x0600006F RID: 111 RVA: 0x000041EC File Offset: 0x000023EC
		public JObject GeneralEnhanced(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/general_enhanced");
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

		// Token: 0x06000070 RID: 112 RVA: 0x0000428C File Offset: 0x0000248C
		public JObject GeneralEnhancedUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/general_enhanced");
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

		// Token: 0x06000071 RID: 113 RVA: 0x0000431C File Offset: 0x0000251C
		public JObject WebImage(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/webimage");
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

		// Token: 0x06000072 RID: 114 RVA: 0x000043BC File Offset: 0x000025BC
		public JObject WebImageUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/webimage");
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

		// Token: 0x06000073 RID: 115 RVA: 0x0000444C File Offset: 0x0000264C
		public JObject Idcard(byte[] image, string idCardSide, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/idcard");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["id_card_side"] = idCardSide;
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

		// Token: 0x06000074 RID: 116 RVA: 0x000044FC File Offset: 0x000026FC
		public JObject IdcardUrl(string url, string idCardSide, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/idcard");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["id_card_side"] = idCardSide;
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

		// Token: 0x06000075 RID: 117 RVA: 0x000045A8 File Offset: 0x000027A8
		public JObject Bankcard(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/bankcard");
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

		// Token: 0x06000076 RID: 118 RVA: 0x00004648 File Offset: 0x00002848
		public JObject DrivingLicense(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/driving_license");
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

		// Token: 0x06000077 RID: 119 RVA: 0x000046E8 File Offset: 0x000028E8
		public JObject DrivingLicenseUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/driving_license");
			base.CheckNotNull(url, "url");
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

		// Token: 0x06000078 RID: 120 RVA: 0x00004784 File Offset: 0x00002984
		public JObject VehicleLicense(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_license");
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

		// Token: 0x06000079 RID: 121 RVA: 0x00004824 File Offset: 0x00002A24
		public JObject VehicleLicenseUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_license");
			base.CheckNotNull(url, "url");
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

		// Token: 0x0600007A RID: 122 RVA: 0x000048C0 File Offset: 0x00002AC0
		public JObject LicensePlate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/license_plate");
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

		// Token: 0x0600007B RID: 123 RVA: 0x00004960 File Offset: 0x00002B60
		public JObject BusinessLicense(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/business_license");
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

		// Token: 0x0600007C RID: 124 RVA: 0x00004A00 File Offset: 0x00002C00
		public JObject Receipt(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/receipt");
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

		// Token: 0x0600007D RID: 125 RVA: 0x00004AA0 File Offset: 0x00002CA0
		public JObject TrainTicket(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/train_ticket");
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

		// Token: 0x0600007E RID: 126 RVA: 0x00004B40 File Offset: 0x00002D40
		public JObject TrainTicketUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/train_ticket");
			base.CheckNotNull(url, "url");
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

		// Token: 0x0600007F RID: 127 RVA: 0x00004BDC File Offset: 0x00002DDC
		public JObject TaxiReceipt(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/taxi_receipt");
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

		// Token: 0x06000080 RID: 128 RVA: 0x00004C7C File Offset: 0x00002E7C
		public JObject TaxiReceiptUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/taxi_receipt");
			base.CheckNotNull(url, "url");
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

		// Token: 0x06000081 RID: 129 RVA: 0x00004D18 File Offset: 0x00002F18
		public JObject Form(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/form");
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

		// Token: 0x06000082 RID: 130 RVA: 0x00004DB8 File Offset: 0x00002FB8
		public JObject TableRecognitionRequest(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/form_ocr/request");
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

		// Token: 0x06000083 RID: 131 RVA: 0x00004E58 File Offset: 0x00003058
		public JObject TableRecognitionGetResult(string requestId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/form_ocr/get_request_result");
			aipHttpRequest.Bodys["request_id"] = requestId;
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

		// Token: 0x06000084 RID: 132 RVA: 0x00004EE8 File Offset: 0x000030E8
		public JObject VinCode(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vin_code");
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

		// Token: 0x06000085 RID: 133 RVA: 0x00004F88 File Offset: 0x00003188
		public JObject VinCodeUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vin_code");
			base.CheckNotNull(url, "url");
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

		// Token: 0x06000086 RID: 134 RVA: 0x00005024 File Offset: 0x00003224
		public JObject QuotaInvoice(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/quota_invoice");
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

		// Token: 0x06000087 RID: 135 RVA: 0x000050C4 File Offset: 0x000032C4
		public JObject HouseholdRegister(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/household_register");
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

		// Token: 0x06000088 RID: 136 RVA: 0x00005164 File Offset: 0x00003364
		public JObject HkMacauExitentrypermit(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/HK_Macau_exitentrypermit");
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

		// Token: 0x06000089 RID: 137 RVA: 0x00005204 File Offset: 0x00003404
		public JObject TaiwanExitentrypermit(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/taiwan_exitentrypermit");
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

		// Token: 0x0600008A RID: 138 RVA: 0x000052A4 File Offset: 0x000034A4
		public JObject BirthCertificate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/birth_certificate");
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

		// Token: 0x0600008B RID: 139 RVA: 0x00005344 File Offset: 0x00003544
		public JObject VehicleInvoice(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_invoice");
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

		// Token: 0x0600008C RID: 140 RVA: 0x000053E4 File Offset: 0x000035E4
		public JObject VehicleCertificate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_certificate");
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

		// Token: 0x0600008D RID: 141 RVA: 0x00005484 File Offset: 0x00003684
		public JObject Invoice(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/invoice");
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

		// Token: 0x0600008E RID: 142 RVA: 0x00005524 File Offset: 0x00003724
		public JObject InvoiceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/invoice");
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

		// Token: 0x0600008F RID: 143 RVA: 0x000055B4 File Offset: 0x000037B4
		public JObject AirTicket(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/air_ticket");
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

		// Token: 0x06000090 RID: 144 RVA: 0x00005654 File Offset: 0x00003854
		public JObject InsuranceDocuments(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/insurance_documents");
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

		// Token: 0x06000091 RID: 145 RVA: 0x000056F4 File Offset: 0x000038F4
		public JObject VatInvoice(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vat_invoice");
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

		// Token: 0x06000092 RID: 146 RVA: 0x00005794 File Offset: 0x00003994
		public JObject VatInvoiceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vat_invoice");
			base.CheckNotNull(url, "url");
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

		// Token: 0x06000093 RID: 147 RVA: 0x00005830 File Offset: 0x00003A30
		public JObject VatInvoicePdf(byte[] pdf, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vat_invoice");
			base.CheckNotNull(pdf, "pdf");
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

		// Token: 0x06000094 RID: 148 RVA: 0x000058D0 File Offset: 0x00003AD0
		public JObject Qrcode(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/qrcode");
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

		// Token: 0x06000095 RID: 149 RVA: 0x00005970 File Offset: 0x00003B70
		public JObject Numbers(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/numbers");
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

		// Token: 0x06000096 RID: 150 RVA: 0x00005A10 File Offset: 0x00003C10
		public JObject Lottery(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/lottery");
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

		// Token: 0x06000097 RID: 151 RVA: 0x00005AB0 File Offset: 0x00003CB0
		public JObject Passport(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/passport");
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

		// Token: 0x06000098 RID: 152 RVA: 0x00005B50 File Offset: 0x00003D50
		public JObject BusinessCard(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/business_card");
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

		// Token: 0x06000099 RID: 153 RVA: 0x00005BF0 File Offset: 0x00003DF0
		public JObject Handwriting(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/handwriting");
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

		// Token: 0x0600009A RID: 154 RVA: 0x00005C90 File Offset: 0x00003E90
		public JObject Custom(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise");
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

		// Token: 0x0600009B RID: 155 RVA: 0x00005D30 File Offset: 0x00003F30
		public JObject CustomUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise");
			base.CheckNotNull(url, "url");
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

		// Token: 0x0600009C RID: 156 RVA: 0x00005DCC File Offset: 0x00003FCC
		public JObject TableRecognition(byte[] image, long timeoutMiliseconds = 20000L, Dictionary<string, object> options = null)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			JObject jobject = this.TableRecognitionRequest(image, null);
			if (!(jobject["result"] is JArray) || ((JArray)jobject["result"]).Count != 1)
			{
				return jobject;
			}
			string requestId = jobject["result"][0]["request_id"].ToString();
			this.Log("Table recognize: wait for result...");
			while (stopwatch.ElapsedMilliseconds < timeoutMiliseconds)
			{
				JObject jobject2 = this.TableRecognitionGetResult(requestId, options);
				JToken jtoken;
				if (jobject2.TryGetValue("error_code", out jtoken))
				{
					this.Log("Table recognize: fail!");
					return jobject2;
				}
				if ((int)jobject2["result"]["ret_code"] == 3)
				{
					this.Log("Table recognize: success!");
					return jobject2;
				}
				string str = "Table recognize: not ready yet, wait 1s...";
				JObject jobject3 = jobject2;
				this.Log(str + ((jobject3 != null) ? jobject3.ToString() : null));
				Thread.Sleep(1000);
			}
			this.Log("Timeout!");
			throw new AipException("SDK Error: Timeout for form recognition");
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005EDE File Offset: 0x000040DE
		public JObject TableRecognitionToJson(byte[] image, long timeoutMiliseconds = 20000L, Dictionary<string, object> options = null)
		{
			if (options == null)
			{
				options = new Dictionary<string, object>();
			}
			options["result_type"] = "json";
			return this.TableRecognition(image, timeoutMiliseconds, options);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005F03 File Offset: 0x00004103
		public JObject TableRecognitionToExcel(byte[] image, long timeoutMiliseconds = 20000L, Dictionary<string, object> options = null)
		{
			if (options == null)
			{
				options = new Dictionary<string, object>();
			}
			options["result_type"] = "excel";
			return this.TableRecognition(image, timeoutMiliseconds, options);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005F28 File Offset: 0x00004128
		public JObject WebimageLocUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/webimage_loc");
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

		// Token: 0x060000A0 RID: 160 RVA: 0x00005FB8 File Offset: 0x000041B8
		public JObject Meter(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/meter");
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

		// Token: 0x060000A1 RID: 161 RVA: 0x00006058 File Offset: 0x00004258
		public JObject MeterUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/meter");
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

		// Token: 0x060000A2 RID: 162 RVA: 0x000060E8 File Offset: 0x000042E8
		public JObject WebimageLoc(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/webimage_loc");
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

		// Token: 0x060000A3 RID: 163 RVA: 0x00006188 File Offset: 0x00004388
		public JObject Seal(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/seal");
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

		// Token: 0x060000A4 RID: 164 RVA: 0x00006228 File Offset: 0x00004428
		public JObject DocAnalysis(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_analysis");
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

		// Token: 0x060000A5 RID: 165 RVA: 0x000062C8 File Offset: 0x000044C8
		public JObject DocAnalysisUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_analysis");
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

		// Token: 0x060000A6 RID: 166 RVA: 0x00006358 File Offset: 0x00004558
		public JObject DocAnalysisOffice(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_analysis_office");
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

		// Token: 0x060000A7 RID: 167 RVA: 0x000063F8 File Offset: 0x000045F8
		public JObject QrcodeUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/qrcode");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000A8 RID: 168 RVA: 0x00006494 File Offset: 0x00004694
		public JObject VehicleInvoiceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_invoice");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000A9 RID: 169 RVA: 0x00006530 File Offset: 0x00004730
		public JObject VehicleCertificateUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_certificate");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000AA RID: 170 RVA: 0x000065CC File Offset: 0x000047CC
		public JObject HouseholdRegisterUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/household_register");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000AB RID: 171 RVA: 0x00006668 File Offset: 0x00004868
		public JObject HandwritingUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/handwriting");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000AC RID: 172 RVA: 0x00006704 File Offset: 0x00004904
		public JObject AirTicketUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/air_ticket");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000AD RID: 173 RVA: 0x000067A0 File Offset: 0x000049A0
		public JObject PassportUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/passport");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000AE RID: 174 RVA: 0x0000683C File Offset: 0x00004A3C
		public JObject OnlineTaxiItinerary(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/online_taxi_itinerary");
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

		// Token: 0x060000AF RID: 175 RVA: 0x000068DC File Offset: 0x00004ADC
		public JObject OnlineTaxiItineraryUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/online_taxi_itinerary");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000B0 RID: 176 RVA: 0x00006978 File Offset: 0x00004B78
		public JObject OnlineTaxiItineraryPdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/online_taxi_itinerary");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000B1 RID: 177 RVA: 0x00006A18 File Offset: 0x00004C18
		public JObject WeightNote(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/weight_note");
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

		// Token: 0x060000B2 RID: 178 RVA: 0x00006AB8 File Offset: 0x00004CB8
		public JObject WeightNoteUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/weight_note");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000B3 RID: 179 RVA: 0x00006B54 File Offset: 0x00004D54
		public JObject WeightNotePdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/weight_note");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000B4 RID: 180 RVA: 0x00006BF4 File Offset: 0x00004DF4
		public JObject MedicalDetail(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_detail");
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

		// Token: 0x060000B5 RID: 181 RVA: 0x00006C94 File Offset: 0x00004E94
		public JObject MedicalDetailUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_detail");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000B6 RID: 182 RVA: 0x00006D30 File Offset: 0x00004F30
		public JObject Formula(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/formula");
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

		// Token: 0x060000B7 RID: 183 RVA: 0x00006DD0 File Offset: 0x00004FD0
		public JObject FormulaUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/formula");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000B8 RID: 184 RVA: 0x00006E6C File Offset: 0x0000506C
		public JObject Finance(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise/finance");
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

		// Token: 0x060000B9 RID: 185 RVA: 0x00006F0C File Offset: 0x0000510C
		public JObject FinanceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise/finance");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000BA RID: 186 RVA: 0x00006FA8 File Offset: 0x000051A8
		public JObject FinancePdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise/finance");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000BB RID: 187 RVA: 0x00007048 File Offset: 0x00005248
		public JObject Facade(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/facade");
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

		// Token: 0x060000BC RID: 188 RVA: 0x000070E8 File Offset: 0x000052E8
		public JObject BusTicket(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/bus_ticket");
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

		// Token: 0x060000BD RID: 189 RVA: 0x00007188 File Offset: 0x00005388
		public JObject BusTicketUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/bus_ticket");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000BE RID: 190 RVA: 0x00007224 File Offset: 0x00005424
		public JObject TollInvoice(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/toll_invoice");
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

		// Token: 0x060000BF RID: 191 RVA: 0x000072C4 File Offset: 0x000054C4
		public JObject TollInvoiceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/toll_invoice");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000C0 RID: 192 RVA: 0x00007360 File Offset: 0x00005560
		public JObject MultiCardClassify(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/multi_card_classify");
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

		// Token: 0x060000C1 RID: 193 RVA: 0x00007400 File Offset: 0x00005600
		public JObject MultiCardClassifyUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/multi_card_classify");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000C2 RID: 194 RVA: 0x0000749C File Offset: 0x0000569C
		public JObject IntelligentOcr(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/intelligent_ocr");
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

		// Token: 0x060000C3 RID: 195 RVA: 0x0000753C File Offset: 0x0000573C
		public JObject IntelligentOcrUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/intelligent_ocr");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000C4 RID: 196 RVA: 0x000075D8 File Offset: 0x000057D8
		public JObject MedicalRecord(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_record");
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

		// Token: 0x060000C5 RID: 197 RVA: 0x00007678 File Offset: 0x00005878
		public JObject MedicalRecordUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_record");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000C6 RID: 198 RVA: 0x00007714 File Offset: 0x00005914
		public JObject MedicalStatement(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_statement");
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

		// Token: 0x060000C7 RID: 199 RVA: 0x000077B4 File Offset: 0x000059B4
		public JObject MedicalStatementUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_statement");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000C8 RID: 200 RVA: 0x00007850 File Offset: 0x00005A50
		public JObject MedicalInvoice(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_invoice");
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

		// Token: 0x060000C9 RID: 201 RVA: 0x000078F0 File Offset: 0x00005AF0
		public JObject MedicalInvoiceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_invoice");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000CA RID: 202 RVA: 0x0000798C File Offset: 0x00005B8C
		public JObject FerryTicket(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/ferry_ticket");
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

		// Token: 0x060000CB RID: 203 RVA: 0x00007A2C File Offset: 0x00005C2C
		public JObject FerryTicketUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/ferry_ticket");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000CC RID: 204 RVA: 0x00007AC8 File Offset: 0x00005CC8
		public JObject UsedVehicleInvoice(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/used_vehicle_invoice");
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

		// Token: 0x060000CD RID: 205 RVA: 0x00007B68 File Offset: 0x00005D68
		public JObject UsedVehicleInvoiceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/used_vehicle_invoice");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000CE RID: 206 RVA: 0x00007C04 File Offset: 0x00005E04
		public JObject MultiIdcard(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/multi_idcard");
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

		// Token: 0x060000CF RID: 207 RVA: 0x00007CA4 File Offset: 0x00005EA4
		public JObject MultiIdcardUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/multi_idcard");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000D0 RID: 208 RVA: 0x00007D40 File Offset: 0x00005F40
		public JObject TravelCard(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/travel_card");
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

		// Token: 0x060000D1 RID: 209 RVA: 0x00007DE0 File Offset: 0x00005FE0
		public JObject SocialSecurityCard(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/social_security_card");
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

		// Token: 0x060000D2 RID: 210 RVA: 0x00007E80 File Offset: 0x00006080
		public JObject SocialSecurityCardUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/social_security_card");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000D3 RID: 211 RVA: 0x00007F1C File Offset: 0x0000611C
		public JObject MedicalReportDetection(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_report_detection");
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

		// Token: 0x060000D4 RID: 212 RVA: 0x00007FBC File Offset: 0x000061BC
		public JObject MedicalReportDetectionUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_report_detection");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000D5 RID: 213 RVA: 0x00008058 File Offset: 0x00006258
		public JObject MedicalReciptsClassify(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_recipts_classify");
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

		// Token: 0x060000D6 RID: 214 RVA: 0x000080F8 File Offset: 0x000062F8
		public JObject MedicalReciptsClassifyUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_recipts_classify");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000D7 RID: 215 RVA: 0x00008194 File Offset: 0x00006394
		public JObject WayBill(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/waybill");
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

		// Token: 0x060000D8 RID: 216 RVA: 0x00008234 File Offset: 0x00006434
		public JObject WayBillUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/waybill");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000D9 RID: 217 RVA: 0x000082D0 File Offset: 0x000064D0
		public JObject MedicalSummary(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_summary");
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

		// Token: 0x060000DA RID: 218 RVA: 0x00008370 File Offset: 0x00006570
		public JObject MedicalSummaryUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_summary");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000DB RID: 219 RVA: 0x0000840C File Offset: 0x0000660C
		public JObject ShoppingReceipt(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/shopping_receipt");
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

		// Token: 0x060000DC RID: 220 RVA: 0x000084AC File Offset: 0x000066AC
		public JObject ShoppingReceiptUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/shopping_receipt");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000DD RID: 221 RVA: 0x00008548 File Offset: 0x00006748
		public JObject ShoppingReceiptPdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/shopping_receipt");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000DE RID: 222 RVA: 0x000085E8 File Offset: 0x000067E8
		public JObject RoadTransportCertificate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/road_transport_certificate");
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

		// Token: 0x060000DF RID: 223 RVA: 0x00008688 File Offset: 0x00006888
		public JObject RoadTransportCertificateUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/road_transport_certificate");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000E0 RID: 224 RVA: 0x00008724 File Offset: 0x00006924
		public JObject RoadTransportCertificatePdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/road_transport_certificate");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000E1 RID: 225 RVA: 0x000087C4 File Offset: 0x000069C4
		public JObject Table(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/table");
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

		// Token: 0x060000E2 RID: 226 RVA: 0x00008864 File Offset: 0x00006A64
		public JObject TableUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/table");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000E3 RID: 227 RVA: 0x00008900 File Offset: 0x00006B00
		public JObject TablePdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/table");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000E4 RID: 228 RVA: 0x000089A0 File Offset: 0x00006BA0
		public JObject RemoveHandwriting(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/remove_handwriting");
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

		// Token: 0x060000E5 RID: 229 RVA: 0x00008A40 File Offset: 0x00006C40
		public JObject RemoveHandwritingUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/remove_handwriting");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000E6 RID: 230 RVA: 0x00008ADC File Offset: 0x00006CDC
		public JObject RemoveHandwritingPdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/remove_handwriting");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000E7 RID: 231 RVA: 0x00008B7C File Offset: 0x00006D7C
		public JObject DocCropEnhance(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_crop_enhance");
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

		// Token: 0x060000E8 RID: 232 RVA: 0x00008C1C File Offset: 0x00006E1C
		public JObject DocCropEnhanceUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_crop_enhance");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000E9 RID: 233 RVA: 0x00008CB8 File Offset: 0x00006EB8
		public JObject DocCropEnhancePdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_crop_enhance");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000EA RID: 234 RVA: 0x00008D58 File Offset: 0x00006F58
		public JObject HealthCode(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/health_code");
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

		// Token: 0x060000EB RID: 235 RVA: 0x00008DF8 File Offset: 0x00006FF8
		public JObject CovidTest(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/covid_test");
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

		// Token: 0x060000EC RID: 236 RVA: 0x00008E98 File Offset: 0x00007098
		public JObject MedicalPrescription(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_prescription");
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

		// Token: 0x060000ED RID: 237 RVA: 0x00008F38 File Offset: 0x00007138
		public JObject MedicalPrescriptionUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_prescription");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000EE RID: 238 RVA: 0x00008FD4 File Offset: 0x000071D4
		public JObject MedicalOutpatient(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_outpatient");
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

		// Token: 0x060000EF RID: 239 RVA: 0x00009074 File Offset: 0x00007274
		public JObject MedicalOutpatientUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_outpatient");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000F0 RID: 240 RVA: 0x00009110 File Offset: 0x00007310
		public JObject MedicalSummaryDiagnosis(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_summary_diagnosis");
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

		// Token: 0x060000F1 RID: 241 RVA: 0x000091B0 File Offset: 0x000073B0
		public JObject MedicalSummaryDiagnosisUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/medical_summary_diagnosis");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000F2 RID: 242 RVA: 0x0000924C File Offset: 0x0000744C
		public JObject HealthReport(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/health_report");
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

		// Token: 0x060000F3 RID: 243 RVA: 0x000092EC File Offset: 0x000074EC
		public JObject HealthReportUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/health_report");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000F4 RID: 244 RVA: 0x00009388 File Offset: 0x00007588
		public JObject DocConvertRequestV1(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_convert/request");
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

		// Token: 0x060000F5 RID: 245 RVA: 0x00009428 File Offset: 0x00007628
		public JObject DocConvertRequestV1Url(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_convert/request");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000F6 RID: 246 RVA: 0x000094C4 File Offset: 0x000076C4
		public JObject DocConvertRequestV1Pdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_convert/request");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000F7 RID: 247 RVA: 0x00009564 File Offset: 0x00007764
		public JObject DocConvertResultV1(string task_id)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/doc_convert/get_request_result");
			base.CheckNotNull(task_id, "task_id");
			aipHttpRequest.Bodys["task_id"] = task_id;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x000095A8 File Offset: 0x000077A8
		public JObject BankReceiptNew(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/bank_receipt_new");
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

		// Token: 0x060000F9 RID: 249 RVA: 0x00009648 File Offset: 0x00007848
		public JObject BankReceiptNewUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/bank_receipt_new");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000FA RID: 250 RVA: 0x000096E4 File Offset: 0x000078E4
		public JObject BankReceiptNewPdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/bank_receipt_new");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000FB RID: 251 RVA: 0x00009784 File Offset: 0x00007984
		public JObject MarriageCertificate(byte[] image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/marriage_certificate");
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

		// Token: 0x060000FC RID: 252 RVA: 0x00009824 File Offset: 0x00007A24
		public JObject MarriageCertificateUrl(string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/marriage_certificate");
			base.CheckNotNull(url, "url");
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

		// Token: 0x060000FD RID: 253 RVA: 0x000098C0 File Offset: 0x00007AC0
		public JObject MarriageCertificatePdf(byte[] pdf_file, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/marriage_certificate");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
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

		// Token: 0x060000FE RID: 254 RVA: 0x00009960 File Offset: 0x00007B60
		public JObject HkMacauTaiwanExitentrypermit(byte[] image, string exitentrypermitType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/hk_macau_taiwan_exitentrypermit");
			base.CheckNotNull(image, "image");
			aipHttpRequest.Bodys["image"] = Convert.ToBase64String(image);
			aipHttpRequest.Bodys["exitentrypermit_type"] = exitentrypermitType;
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

		// Token: 0x060000FF RID: 255 RVA: 0x00009A10 File Offset: 0x00007C10
		public JObject HkMacauTaiwanExitentrypermitUrl(string url, string exitentrypermitType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/hk_macau_taiwan_exitentrypermit");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["exitentrypermit_type"] = exitentrypermitType;
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

		// Token: 0x06000100 RID: 256 RVA: 0x00009ABC File Offset: 0x00007CBC
		public JObject HkMacauTaiwanExitentrypermitPdf(byte[] pdf_file, string exitentrypermitType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/ocr/v1/hk_macau_taiwan_exitentrypermit");
			base.CheckNotNull(pdf_file, "pdf_file");
			aipHttpRequest.Bodys["pdf_file"] = Convert.ToBase64String(pdf_file);
			aipHttpRequest.Bodys["exitentrypermit_type"] = exitentrypermitType;
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

		// Token: 0x0400002A RID: 42
		private const string MEDICAL_DETAIL = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_detail";

		// Token: 0x0400002B RID: 43
		private const string WEIGHT_NOTE = "https://aip.baidubce.com/rest/2.0/ocr/v1/weight_note";

		// Token: 0x0400002C RID: 44
		private const string ONLINE_TAXI_ITINERARY = "https://aip.baidubce.com/rest/2.0/ocr/v1/online_taxi_itinerary";

		// Token: 0x0400002D RID: 45
		private const string DOC_ANALYSIS_OFFICE = "https://aip.baidubce.com/rest/2.0/ocr/v1/doc_analysis_office";

		// Token: 0x0400002E RID: 46
		private const string DOC_ANALYSIS = "https://aip.baidubce.com/rest/2.0/ocr/v1/doc_analysis";

		// Token: 0x0400002F RID: 47
		private const string SEAL = "https://aip.baidubce.com/rest/2.0/ocr/v1/seal";

		// Token: 0x04000030 RID: 48
		private const string METER = "https://aip.baidubce.com/rest/2.0/ocr/v1/meter";

		// Token: 0x04000031 RID: 49
		private const string WEBIMAGE_LOC = "https://aip.baidubce.com/rest/2.0/ocr/v1/webimage_loc";

		// Token: 0x04000032 RID: 50
		private const string GENERAL_BASIC = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic";

		// Token: 0x04000033 RID: 51
		private const string ACCURATE_BASIC = "https://aip.baidubce.com/rest/2.0/ocr/v1/accurate_basic";

		// Token: 0x04000034 RID: 52
		private const string GENERAL = "https://aip.baidubce.com/rest/2.0/ocr/v1/general";

		// Token: 0x04000035 RID: 53
		private const string ACCURATE = "https://aip.baidubce.com/rest/2.0/ocr/v1/accurate";

		// Token: 0x04000036 RID: 54
		private const string GENERAL_ENHANCED = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_enhanced";

		// Token: 0x04000037 RID: 55
		private const string WEB_IMAGE = "https://aip.baidubce.com/rest/2.0/ocr/v1/webimage";

		// Token: 0x04000038 RID: 56
		private const string IDCARD = "https://aip.baidubce.com/rest/2.0/ocr/v1/idcard";

		// Token: 0x04000039 RID: 57
		private const string BANKCARD = "https://aip.baidubce.com/rest/2.0/ocr/v1/bankcard";

		// Token: 0x0400003A RID: 58
		private const string DRIVING_LICENSE = "https://aip.baidubce.com/rest/2.0/ocr/v1/driving_license";

		// Token: 0x0400003B RID: 59
		private const string VEHICLE_LICENSE = "https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_license";

		// Token: 0x0400003C RID: 60
		private const string LICENSE_PLATE = "https://aip.baidubce.com/rest/2.0/ocr/v1/license_plate";

		// Token: 0x0400003D RID: 61
		private const string BUSINESS_LICENSE = "https://aip.baidubce.com/rest/2.0/ocr/v1/business_license";

		// Token: 0x0400003E RID: 62
		private const string RECEIPT = "https://aip.baidubce.com/rest/2.0/ocr/v1/receipt";

		// Token: 0x0400003F RID: 63
		private const string TRAIN_TICKET = "https://aip.baidubce.com/rest/2.0/ocr/v1/train_ticket";

		// Token: 0x04000040 RID: 64
		private const string TAXI_RECEIPT = "https://aip.baidubce.com/rest/2.0/ocr/v1/taxi_receipt";

		// Token: 0x04000041 RID: 65
		private const string FORM = "https://aip.baidubce.com/rest/2.0/ocr/v1/form";

		// Token: 0x04000042 RID: 66
		private const string TABLE_RECOGNIZE = "https://aip.baidubce.com/rest/2.0/solution/v1/form_ocr/request";

		// Token: 0x04000043 RID: 67
		private const string TABLE_RESULT_GET = "https://aip.baidubce.com/rest/2.0/solution/v1/form_ocr/get_request_result";

		// Token: 0x04000044 RID: 68
		private const string VIN_CODE = "https://aip.baidubce.com/rest/2.0/ocr/v1/vin_code";

		// Token: 0x04000045 RID: 69
		private const string QUOTA_INVOICE = "https://aip.baidubce.com/rest/2.0/ocr/v1/quota_invoice";

		// Token: 0x04000046 RID: 70
		private const string HOUSEHOLD_REGISTER = "https://aip.baidubce.com/rest/2.0/ocr/v1/household_register";

		// Token: 0x04000047 RID: 71
		private const string HK_MACAU_EXITENTRYPERMIT = "https://aip.baidubce.com/rest/2.0/ocr/v1/HK_Macau_exitentrypermit";

		// Token: 0x04000048 RID: 72
		private const string TAIWAN_EXITENTRYPERMIT = "https://aip.baidubce.com/rest/2.0/ocr/v1/taiwan_exitentrypermit";

		// Token: 0x04000049 RID: 73
		private const string BIRTH_CERTIFICATE = "https://aip.baidubce.com/rest/2.0/ocr/v1/birth_certificate";

		// Token: 0x0400004A RID: 74
		private const string VEHICLE_INVOICE = "https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_invoice";

		// Token: 0x0400004B RID: 75
		private const string VEHICLE_CERTIFICATE = "https://aip.baidubce.com/rest/2.0/ocr/v1/vehicle_certificate";

		// Token: 0x0400004C RID: 76
		private const string INVOICE = "https://aip.baidubce.com/rest/2.0/ocr/v1/invoice";

		// Token: 0x0400004D RID: 77
		private const string AIR_TICKET = "https://aip.baidubce.com/rest/2.0/ocr/v1/air_ticket";

		// Token: 0x0400004E RID: 78
		private const string INSURANCE_DOCUMENTS = "https://aip.baidubce.com/rest/2.0/ocr/v1/insurance_documents";

		// Token: 0x0400004F RID: 79
		private const string VAT_INVOICE = "https://aip.baidubce.com/rest/2.0/ocr/v1/vat_invoice";

		// Token: 0x04000050 RID: 80
		private const string QRCODE = "https://aip.baidubce.com/rest/2.0/ocr/v1/qrcode";

		// Token: 0x04000051 RID: 81
		private const string NUMBERS = "https://aip.baidubce.com/rest/2.0/ocr/v1/numbers";

		// Token: 0x04000052 RID: 82
		private const string LOTTERY = "https://aip.baidubce.com/rest/2.0/ocr/v1/lottery";

		// Token: 0x04000053 RID: 83
		private const string PASSPORT = "https://aip.baidubce.com/rest/2.0/ocr/v1/passport";

		// Token: 0x04000054 RID: 84
		private const string BUSINESS_CARD = "https://aip.baidubce.com/rest/2.0/ocr/v1/business_card";

		// Token: 0x04000055 RID: 85
		private const string HANDWRITING = "https://aip.baidubce.com/rest/2.0/ocr/v1/handwriting";

		// Token: 0x04000056 RID: 86
		private const string CUSTOM = "https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise";

		// Token: 0x04000057 RID: 87
		private const string FORMULA = "https://aip.baidubce.com/rest/2.0/ocr/v1/formula";

		// Token: 0x04000058 RID: 88
		private const string FINANCE = "https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise/finance";

		// Token: 0x04000059 RID: 89
		private const string FACADE = "https://aip.baidubce.com/rest/2.0/ocr/v1/facade";

		// Token: 0x0400005A RID: 90
		private const string BUSTICKET = "https://aip.baidubce.com/rest/2.0/ocr/v1/bus_ticket";

		// Token: 0x0400005B RID: 91
		private const string TOLLINVOICE = "https://aip.baidubce.com/rest/2.0/ocr/v1/toll_invoice";

		// Token: 0x0400005C RID: 92
		private const string MULTICARDCLASSIFY = "https://aip.baidubce.com/rest/2.0/ocr/v1/multi_card_classify";

		// Token: 0x0400005D RID: 93
		private const string INTELLIGENTOCR = "https://aip.baidubce.com/rest/2.0/ocr/v1/intelligent_ocr";

		// Token: 0x0400005E RID: 94
		private const string MEDICALRECORD = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_record";

		// Token: 0x0400005F RID: 95
		private const string MEDICALSTATEMENT = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_statement";

		// Token: 0x04000060 RID: 96
		private const string MEDICALINVOICE = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_invoice";

		// Token: 0x04000061 RID: 97
		private const string FERRYTICKET = "https://aip.baidubce.com/rest/2.0/ocr/v1/ferry_ticket";

		// Token: 0x04000062 RID: 98
		private const string USEDVEHICLEINVOICE = "https://aip.baidubce.com/rest/2.0/ocr/v1/used_vehicle_invoice";

		// Token: 0x04000063 RID: 99
		private const string MULTIIDCARD = "https://aip.baidubce.com/rest/2.0/ocr/v1/multi_idcard";

		// Token: 0x04000064 RID: 100
		private const string TRAVELCARD = "https://aip.baidubce.com/rest/2.0/ocr/v1/travel_card";

		// Token: 0x04000065 RID: 101
		private const string SOCIALSECURITYCARD = "https://aip.baidubce.com/rest/2.0/ocr/v1/social_security_card";

		// Token: 0x04000066 RID: 102
		private const string MEDICALREPORTDETECTION = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_report_detection";

		// Token: 0x04000067 RID: 103
		private const string MEDICALRECIPTSCLASSIFY = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_recipts_classify";

		// Token: 0x04000068 RID: 104
		private const string WAYBILL = "https://aip.baidubce.com/rest/2.0/ocr/v1/waybill";

		// Token: 0x04000069 RID: 105
		private const string MEDICALSUMMARY = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_summary";

		// Token: 0x0400006A RID: 106
		private const string SHOPPINGRECEIPT = "https://aip.baidubce.com/rest/2.0/ocr/v1/shopping_receipt";

		// Token: 0x0400006B RID: 107
		private const string ROADTRANSPORTCERTIFICATE = "https://aip.baidubce.com/rest/2.0/ocr/v1/road_transport_certificate";

		// Token: 0x0400006C RID: 108
		private const string TABLE = "https://aip.baidubce.com/rest/2.0/ocr/v1/table";

		// Token: 0x0400006D RID: 109
		private const string REMOVEHANDWRITING = "https://aip.baidubce.com/rest/2.0/ocr/v1/remove_handwriting";

		// Token: 0x0400006E RID: 110
		private const string DOCCROPENHANCE = "https://aip.baidubce.com/rest/2.0/ocr/v1/doc_crop_enhance";

		// Token: 0x0400006F RID: 111
		private const string HEALTHCODE = "https://aip.baidubce.com/rest/2.0/ocr/v1/health_code";

		// Token: 0x04000070 RID: 112
		private const string COVIDTEST = "https://aip.baidubce.com/rest/2.0/ocr/v1/covid_test";

		// Token: 0x04000071 RID: 113
		private const string MEDICALPRESCRIPTION = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_prescription";

		// Token: 0x04000072 RID: 114
		private const string MEDICALOUTPATIENT = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_outpatient";

		// Token: 0x04000073 RID: 115
		private const string MEDICALSUMMARYDIAGNOSIS = "https://aip.baidubce.com/rest/2.0/ocr/v1/medical_summary_diagnosis";

		// Token: 0x04000074 RID: 116
		private const string HEALTHREPORT = "https://aip.baidubce.com/rest/2.0/ocr/v1/health_report";

		// Token: 0x04000075 RID: 117
		private const string DOCCONVERT = "https://aip.baidubce.com/rest/2.0/ocr/v1/doc_convert/request";

		// Token: 0x04000076 RID: 118
		private const string DOCCONVERTRESULT = "https://aip.baidubce.com/rest/2.0/ocr/v1/doc_convert/get_request_result";

		// Token: 0x04000077 RID: 119
		private const string BANK_RECEIPT_NEW = "https://aip.baidubce.com/rest/2.0/ocr/v1/bank_receipt_new";

		// Token: 0x04000078 RID: 120
		private const string MARRIAGE_CERTIFICATE = "https://aip.baidubce.com/rest/2.0/ocr/v1/marriage_certificate";

		// Token: 0x04000079 RID: 121
		private const string HK_MACAU_TAIWAN_EXITENTRYPERMIT = "https://aip.baidubce.com/rest/2.0/ocr/v1/hk_macau_taiwan_exitentrypermit";
	}
}
