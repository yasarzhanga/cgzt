using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.Face
{
	// Token: 0x02000015 RID: 21
	public class Face : AipServiceBase
	{
		// Token: 0x060001B2 RID: 434 RVA: 0x0000FDA8 File Offset: 0x0000DFA8
		public Face(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000FDB2 File Offset: 0x0000DFB2
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Json,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000FDD8 File Offset: 0x0000DFD8
		public JObject Match(JArray faces)
		{
			base.CheckNotNull(faces, "faces");
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/match");
			aipHttpRequest.BodyType = AipHttpRequest.BodyFormat.JsonRaw;
			base.PreAction();
			aipHttpRequest.Bodys["RAw"] = faces;
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000FE24 File Offset: 0x0000E024
		public JObject Faceverify(JArray faces)
		{
			base.CheckNotNull(faces, "faces");
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceverify");
			aipHttpRequest.BodyType = AipHttpRequest.BodyFormat.JsonRaw;
			base.PreAction();
			aipHttpRequest.Bodys["RAw"] = faces;
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000FE70 File Offset: 0x0000E070
		public JObject Detect(string image, string imageType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/detect");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000FF10 File Offset: 0x0000E110
		public JObject Search(string image, string imageType, string groupIdList, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/search");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["group_id_list"] = groupIdList;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
		public JObject MultiSearch(string image, string imageType, string groupIdList, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/multi-search");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["group_id_list"] = groupIdList;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00010078 File Offset: 0x0000E278
		public JObject UserAdd(string image, string imageType, string groupId, string userId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/add");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["group_id"] = groupId;
			aipHttpRequest.Bodys["user_id"] = userId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0001013C File Offset: 0x0000E33C
		public JObject UserUpdate(string image, string imageType, string groupId, string userId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/update");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["group_id"] = groupId;
			aipHttpRequest.Bodys["user_id"] = userId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00010200 File Offset: 0x0000E400
		public JObject FaceDelete(string userId, string groupId, string faceToken, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/face/delete");
			aipHttpRequest.Bodys["user_id"] = userId;
			aipHttpRequest.Bodys["group_id"] = groupId;
			aipHttpRequest.Bodys["face_token"] = faceToken;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x000102B4 File Offset: 0x0000E4B4
		public JObject UserGet(string userId, string groupId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/get");
			aipHttpRequest.Bodys["user_id"] = userId;
			aipHttpRequest.Bodys["group_id"] = groupId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00010354 File Offset: 0x0000E554
		public JObject FaceGetlist(string userId, string groupId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/face/getlist");
			aipHttpRequest.Bodys["user_id"] = userId;
			aipHttpRequest.Bodys["group_id"] = groupId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x000103F4 File Offset: 0x0000E5F4
		public JObject GroupGetusers(string groupId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/group/getusers");
			aipHttpRequest.Bodys["group_id"] = groupId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00010484 File Offset: 0x0000E684
		public JObject UserCopy(string userId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/copy");
			aipHttpRequest.Bodys["user_id"] = userId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00010514 File Offset: 0x0000E714
		public JObject UserDelete(string groupId, string userId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/delete");
			aipHttpRequest.Bodys["group_id"] = groupId;
			aipHttpRequest.Bodys["user_id"] = userId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x000105B4 File Offset: 0x0000E7B4
		public JObject GroupAdd(string groupId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/group/add");
			aipHttpRequest.Bodys["group_id"] = groupId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00010644 File Offset: 0x0000E844
		public JObject GroupDelete(string groupId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/group/delete");
			aipHttpRequest.Bodys["group_id"] = groupId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x000106D4 File Offset: 0x0000E8D4
		public JObject GroupGetlist(Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/faceset/group/getlist");
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00010750 File Offset: 0x0000E950
		public JObject PersonVerify(string image, string imageType, string idCardNumber, string name, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/person/verify");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["id_card_number"] = idCardNumber;
			aipHttpRequest.Bodys["name"] = name;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00010814 File Offset: 0x0000EA14
		public JObject VideoSessioncode(Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v1/faceliveness/sessioncode");
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00010890 File Offset: 0x0000EA90
		public JObject faceMingJingVerify(string idCardNumber, string name, string image, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v4/mingjing/verify");
			aipHttpRequest.BodyType = AipHttpRequest.BodyFormat.Json;
			aipHttpRequest.Bodys["id_card_number"] = idCardNumber;
			aipHttpRequest.Bodys["name"] = name;
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

		// Token: 0x060001C7 RID: 455 RVA: 0x00010948 File Offset: 0x0000EB48
		public JObject faceMingJingMatch(string image, string imageType, string registerImage, string registerImageType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v4/mingjing/match");
			aipHttpRequest.BodyType = AipHttpRequest.BodyFormat.Json;
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["register_image"] = registerImage;
			aipHttpRequest.Bodys["register_image_type"] = registerImageType;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00010A14 File Offset: 0x0000EC14
		public JObject onlinePictureLiveV4(string sdkVersion, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v4/faceverify");
			aipHttpRequest.BodyType = AipHttpRequest.BodyFormat.Json;
			aipHttpRequest.Bodys["sdk_version"] = sdkVersion;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00010AA8 File Offset: 0x0000ECA8
		public JObject FacelivenessVerifyV1(string videoBase64, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v1/faceliveness/verify");
			aipHttpRequest.Bodys["video_base64"] = videoBase64;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00010B38 File Offset: 0x0000ED38
		public JObject FacePersonIdmatchV3(string idCardNumber, string name, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v3/person/idmatch");
			aipHttpRequest.Bodys["id_card_number"] = idCardNumber;
			aipHttpRequest.Bodys["name"] = name;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00010BD8 File Offset: 0x0000EDD8
		public JObject FaceMergeV1(Dictionary<string, object> imageTemplate, Dictionary<string, object> imageTarget, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v1/merge");
			aipHttpRequest.Bodys["image_template"] = imageTemplate;
			aipHttpRequest.Bodys["image_target"] = imageTarget;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00010C78 File Offset: 0x0000EE78
		public JObject FaceSkinSmoothV1(string image, string imageType, string actionType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v1/editattr");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["action_type"] = actionType;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00010D2C File Offset: 0x0000EF2C
		public JObject FaceLandmarkV1(string image, string imageType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v1/landmark");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00010DCC File Offset: 0x0000EFCC
		public JObject FaceSceneFacesetUserAdd(string image, string imageType, string groupId, string userId, string sceneType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/scene/faceset/user/add");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["group_id"] = groupId;
			aipHttpRequest.Bodys["user_id"] = userId;
			aipHttpRequest.Bodys["scene_type"] = sceneType;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00010EA4 File Offset: 0x0000F0A4
		public JObject FaceSceneFacesetUserUpdate(string image, string imageType, string groupId, string userId, string sceneType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/scene/faceset/user/update");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["group_id"] = groupId;
			aipHttpRequest.Bodys["user_id"] = userId;
			aipHttpRequest.Bodys["scene_type"] = sceneType;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00010F7C File Offset: 0x0000F17C
		public JObject FaceSceneFacesetGroupAdd(string groupId, string sceneType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/scene/faceset/group/add");
			aipHttpRequest.Bodys["group_id"] = groupId;
			aipHttpRequest.Bodys["scene_type"] = sceneType;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0001101C File Offset: 0x0000F21C
		public JObject FaceCaptureSearch(string image, string imageType, string groupIdList, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/capture/search");
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			aipHttpRequest.Bodys["group_id_list"] = groupIdList;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x000110D0 File Offset: 0x0000F2D0
		public JObject FaceIdmatchDateV4(string name, string idCardNumber, string startDate, string endDate, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v4/idmatch_date");
			aipHttpRequest.Bodys["name"] = name;
			aipHttpRequest.Bodys["id_card_number"] = idCardNumber;
			aipHttpRequest.Bodys["start_date"] = startDate;
			aipHttpRequest.Bodys["end_date"] = endDate;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00011194 File Offset: 0x0000F394
		public JObject FaceVerifyDateV4(string name, string idCardNumber, string startDate, string endDate, string image, string imageType, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/face/v4/verify_date");
			aipHttpRequest.Bodys["name"] = name;
			aipHttpRequest.Bodys["id_card_number"] = idCardNumber;
			aipHttpRequest.Bodys["start_date"] = startDate;
			aipHttpRequest.Bodys["end_date"] = endDate;
			aipHttpRequest.Bodys["image"] = image;
			aipHttpRequest.Bodys["image_type"] = imageType;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x040000E9 RID: 233
		private const string VERIFY = "https://aip.baidubce.com/rest/2.0/face/v4/mingjing/verify";

		// Token: 0x040000EA RID: 234
		private const string DETECT = "https://aip.baidubce.com/rest/2.0/face/v3/detect";

		// Token: 0x040000EB RID: 235
		private const string SEARCH = "https://aip.baidubce.com/rest/2.0/face/v3/search";

		// Token: 0x040000EC RID: 236
		private const string MULTI_SEARCH = "https://aip.baidubce.com/rest/2.0/face/v3/multi-search";

		// Token: 0x040000ED RID: 237
		private const string USER_ADD = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/add";

		// Token: 0x040000EE RID: 238
		private const string USER_UPDATE = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/update";

		// Token: 0x040000EF RID: 239
		private const string FACE_DELETE = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/face/delete";

		// Token: 0x040000F0 RID: 240
		private const string USER_GET = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/get";

		// Token: 0x040000F1 RID: 241
		private const string FACE_GETLIST = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/face/getlist";

		// Token: 0x040000F2 RID: 242
		private const string GROUP_GETUSERS = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/group/getusers";

		// Token: 0x040000F3 RID: 243
		private const string USER_COPY = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/copy";

		// Token: 0x040000F4 RID: 244
		private const string USER_DELETE = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/user/delete";

		// Token: 0x040000F5 RID: 245
		private const string GROUP_ADD = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/group/add";

		// Token: 0x040000F6 RID: 246
		private const string GROUP_DELETE = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/group/delete";

		// Token: 0x040000F7 RID: 247
		private const string GROUP_GETLIST = "https://aip.baidubce.com/rest/2.0/face/v3/faceset/group/getlist";

		// Token: 0x040000F8 RID: 248
		private const string PERSON_VERIFY = "https://aip.baidubce.com/rest/2.0/face/v3/person/verify";

		// Token: 0x040000F9 RID: 249
		private const string VIDEO_SESSIONCODE = "https://aip.baidubce.com/rest/2.0/face/v1/faceliveness/sessioncode";

		// Token: 0x040000FA RID: 250
		private const string FACELIVENESS_VERIFY_V1 = "https://aip.baidubce.com/rest/2.0/face/v1/faceliveness/verify";

		// Token: 0x040000FB RID: 251
		private const string FACE_PERSON_IDMATCH_V3 = "https://aip.baidubce.com/rest/2.0/face/v3/person/idmatch";

		// Token: 0x040000FC RID: 252
		private const string FACE_MERGE_V1 = "https://aip.baidubce.com/rest/2.0/face/v1/merge";

		// Token: 0x040000FD RID: 253
		private const string FACE_SKIN_SMOOTH_V1 = "https://aip.baidubce.com/rest/2.0/face/v1/editattr";

		// Token: 0x040000FE RID: 254
		private const string FACE_LANDMARK_V1 = "https://aip.baidubce.com/rest/2.0/face/v1/landmark";

		// Token: 0x040000FF RID: 255
		private const string FACE_SCENE_FACESET_USER_ADD = "https://aip.baidubce.com/rest/2.0/face/scene/faceset/user/add";

		// Token: 0x04000100 RID: 256
		private const string FACE_SCENE_FACESET_USER_UPDATE = "https://aip.baidubce.com/rest/2.0/face/scene/faceset/user/update";

		// Token: 0x04000101 RID: 257
		private const string FACE_SCENE_FACESET_GROUP_ADD = "https://aip.baidubce.com/rest/2.0/face/scene/faceset/group/add";

		// Token: 0x04000102 RID: 258
		private const string FACE_CAPTURE_SEARCH = "https://aip.baidubce.com/rest/2.0/face/capture/search";

		// Token: 0x04000103 RID: 259
		private const string FACE_IDMATCH_DATE_V4 = "https://aip.baidubce.com/rest/2.0/face/v4/idmatch_date";

		// Token: 0x04000104 RID: 260
		private const string FACE_VERIFY_DATE_V4 = "https://aip.baidubce.com/rest/2.0/face/v4/verify_date";

		// Token: 0x04000105 RID: 261
		private const string MATCH = "https://aip.baidubce.com/rest/2.0/face/v3/match";

		// Token: 0x04000106 RID: 262
		private const string FACEVERIFY = "https://aip.baidubce.com/rest/2.0/face/v3/faceverify";

		// Token: 0x04000107 RID: 263
		private const string FACE_MATCH_V4 = "https://aip.baidubce.com/rest/2.0/face/v4/mingjing/match";

		// Token: 0x04000108 RID: 264
		private const string ONLINE_PICTURE_LIVE_V4 = "https://aip.baidubce.com/rest/2.0/face/v4/faceverify";
	}
}
