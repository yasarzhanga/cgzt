using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.ContentCensor
{
	// Token: 0x0200001A RID: 26
	public class ContentCensor : Base
	{
		// Token: 0x060001DF RID: 479 RVA: 0x000114E0 File Offset: 0x0000F6E0
		public ContentCensor(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x000114EC File Offset: 0x0000F6EC
		public JObject LiveSaveV1(string streamUrl, string streamType, string extId, long startTime, long endTime, string streamName, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/live/v1/config/save");
			aipHttpRequest.Bodys["streamUrl"] = streamUrl;
			aipHttpRequest.Bodys["streamType"] = streamType;
			aipHttpRequest.Bodys["extId"] = extId;
			aipHttpRequest.Bodys["startTime"] = startTime;
			aipHttpRequest.Bodys["endTime"] = endTime;
			aipHttpRequest.Bodys["streamName"] = streamName;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x000115E0 File Offset: 0x0000F7E0
		public JObject LiveStopV1(string taskId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/live/v1/config/stop");
			aipHttpRequest.Bodys["taskId"] = taskId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00011670 File Offset: 0x0000F870
		public JObject LiveViewV1(string taskId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/live/v1/config/view");
			aipHttpRequest.Bodys["taskId"] = taskId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00011700 File Offset: 0x0000F900
		public JObject LivePullV1(string taskId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/live/v1/audit/pull");
			aipHttpRequest.Bodys["taskId"] = taskId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00011790 File Offset: 0x0000F990
		public JObject VideoCensorSubmitV1(string url, string extId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/video_censor/v1/video/submit");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["extId"] = extId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00011830 File Offset: 0x0000FA30
		public JObject VideoCensorPullV1(string taskId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/video_censor/v1/video/pull");
			aipHttpRequest.Bodys["taskId"] = taskId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x000118C0 File Offset: 0x0000FAC0
		public JObject AsyncVoiceSubmitV1(string url, string fmt, int rate, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/async_voice/submit");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["fmt"] = fmt;
			aipHttpRequest.Bodys["rate"] = rate;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00011978 File Offset: 0x0000FB78
		public JObject AsyncVoicePullV1AudioId(string audioId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/async_voice/pull");
			aipHttpRequest.Bodys["audioId"] = audioId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00011A08 File Offset: 0x0000FC08
		public JObject AsyncVoicePullV1TaskId(string taskId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/async_voice/pull");
			aipHttpRequest.Bodys["taskId"] = taskId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00011A98 File Offset: 0x0000FC98
		public JObject DocumentCensorFileSubmit(string fileName, byte[] document, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/solution/document/v1/submit");
			base.CheckNotNull(document, "document");
			base.CheckNotNull(fileName, "fileName");
			aipHttpRequest.Bodys["fileBase64"] = Convert.ToBase64String(document);
			aipHttpRequest.Bodys["fileName"] = fileName;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00011B54 File Offset: 0x0000FD54
		public JObject DocumentCensorUrlSubmit(string fileName, string url, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/solution/document/v1/submit");
			base.CheckNotNull(fileName, "fileName");
			base.CheckNotNull(url, "url");
			aipHttpRequest.Bodys["url"] = url;
			aipHttpRequest.Bodys["fileName"] = fileName;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00011C0C File Offset: 0x0000FE0C
		public JObject DocumentCensorPull(string taskId, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = base.DefaultRequest("https://aip.baidubce.com/rest/2.0/solution/v1/solution/document/v1/pull");
			base.CheckNotNull(taskId, "taskId");
			aipHttpRequest.Bodys["taskId"] = taskId;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0400010C RID: 268
		private const string LIVE_SAVE_V1 = "https://aip.baidubce.com/rest/2.0/solution/v1/live/v1/config/save";

		// Token: 0x0400010D RID: 269
		private const string LIVE_STOP_V1 = "https://aip.baidubce.com/rest/2.0/solution/v1/live/v1/config/stop";

		// Token: 0x0400010E RID: 270
		private const string LIVE_VIEW_V1 = "https://aip.baidubce.com/rest/2.0/solution/v1/live/v1/config/view";

		// Token: 0x0400010F RID: 271
		private const string LIVE_PULL_V1 = "https://aip.baidubce.com/rest/2.0/solution/v1/live/v1/audit/pull";

		// Token: 0x04000110 RID: 272
		private const string VIDEO_CENSOR_SUBMIT_V1 = "https://aip.baidubce.com/rest/2.0/solution/v1/video_censor/v1/video/submit";

		// Token: 0x04000111 RID: 273
		private const string VIDEO_CENSOR_PULL_V1 = "https://aip.baidubce.com/rest/2.0/solution/v1/video_censor/v1/video/pull";

		// Token: 0x04000112 RID: 274
		private const string ASYNC_VOICE_SUBMIT_V1 = "https://aip.baidubce.com/rest/2.0/solution/v1/async_voice/submit";

		// Token: 0x04000113 RID: 275
		private const string ASYNC_VOICE_PULL_V1 = "https://aip.baidubce.com/rest/2.0/solution/v1/async_voice/pull";

		// Token: 0x04000114 RID: 276
		private const string DOCUMENT_CENSOR_SUBMIT_URL = "https://aip.baidubce.com/rest/2.0/solution/v1/solution/document/v1/submit";

		// Token: 0x04000115 RID: 277
		private const string DOCUMENT_CENSOR_PULL_URL = "https://aip.baidubce.com/rest/2.0/solution/v1/solution/document/v1/pull";
	}
}
