using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.Kg
{
	// Token: 0x02000011 RID: 17
	public class Pie : AipServiceBase
	{
		// Token: 0x0600012C RID: 300 RVA: 0x0000AFE0 File Offset: 0x000091E0
		public Pie(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000AFEA File Offset: 0x000091EA
		protected AipHttpRequest DefaultRequest(string uri)
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Formed,
				ContentEncoding = Encoding.UTF8
			};
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000B010 File Offset: 0x00009210
		public JObject CreateTask(string name, string templateContent, string inputMappingFile, string outputFile, string urlPattern, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_create");
			aipHttpRequest.Bodys["name"] = name;
			aipHttpRequest.Bodys["template_content"] = templateContent;
			aipHttpRequest.Bodys["input_mapping_file"] = inputMappingFile;
			aipHttpRequest.Bodys["output_file"] = outputFile;
			aipHttpRequest.Bodys["url_pattern"] = urlPattern;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000B0E8 File Offset: 0x000092E8
		public JObject UpdateTask(int id, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_update");
			aipHttpRequest.Bodys["id"] = id;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000B17C File Offset: 0x0000937C
		public JObject TaskInfo(int id, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_info");
			aipHttpRequest.Bodys["id"] = id;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000B210 File Offset: 0x00009410
		public JObject TaskQuery(Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_query");
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000B28C File Offset: 0x0000948C
		public JObject TaskStart(int id, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_start");
			aipHttpRequest.Bodys["id"] = id;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000B320 File Offset: 0x00009520
		public JObject TaskStatus(int id, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_status");
			aipHttpRequest.Bodys["id"] = id;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x040000A1 RID: 161
		private const string CREATE_TASK = "https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_create";

		// Token: 0x040000A2 RID: 162
		private const string UPDATE_TASK = "https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_update";

		// Token: 0x040000A3 RID: 163
		private const string TASK_INFO = "https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_info";

		// Token: 0x040000A4 RID: 164
		private const string TASK_QUERY = "https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_query";

		// Token: 0x040000A5 RID: 165
		private const string TASK_START = "https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_start";

		// Token: 0x040000A6 RID: 166
		private const string TASK_STATUS = "https://aip.baidubce.com/rest/2.0/kg/v1/pie/task_status";
	}
}
