using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip.Nlp
{
	// Token: 0x0200000F RID: 15
	public class Nlp : AipServiceBase
	{
		// Token: 0x06000101 RID: 257 RVA: 0x00009B6C File Offset: 0x00007D6C
		public Nlp(string apiKey, string secretKey) : base(apiKey, secretKey)
		{
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00009B76 File Offset: 0x00007D76
		protected AipHttpRequest DefaultRequest(string uri, string encoding = "GBK")
		{
			return new AipHttpRequest(uri)
			{
				Method = "POST",
				BodyType = AipHttpRequest.BodyFormat.Json,
				ContentEncoding = Encoding.GetEncoding(encoding)
			};
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00009B9C File Offset: 0x00007D9C
		public JObject Lexer(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/lexer", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00009C30 File Offset: 0x00007E30
		public JObject LexerCustom(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/lexer_custom", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00009CC4 File Offset: 0x00007EC4
		public JObject DepParser(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/depparser", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00009D58 File Offset: 0x00007F58
		public JObject WordEmbedding(string word, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v2/word_emb_vec", "GBK");
			aipHttpRequest.Bodys["word"] = word;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00009DEC File Offset: 0x00007FEC
		public JObject DnnlmCn(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v2/dnnlm_cn", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00009E80 File Offset: 0x00008080
		public JObject WordSimEmbedding(string word1, string word2, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v2/word_emb_sim", "GBK");
			aipHttpRequest.Bodys["word_1"] = word1;
			aipHttpRequest.Bodys["word_2"] = word2;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00009F24 File Offset: 0x00008124
		public JObject Simnet(string text1, string text2, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v2/simnet", "GBK");
			aipHttpRequest.Bodys["text_1"] = text1;
			aipHttpRequest.Bodys["text_2"] = text2;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00009FC8 File Offset: 0x000081C8
		public JObject CommentTag(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v2/comment_tag", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000A05C File Offset: 0x0000825C
		public JObject SentimentClassify(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/sentiment_classify", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000A0F0 File Offset: 0x000082F0
		public JObject Keyword(string title, string content, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/keyword", "GBK");
			aipHttpRequest.Bodys["title"] = title;
			aipHttpRequest.Bodys["content"] = content;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000A194 File Offset: 0x00008394
		public JObject Topic(string title, string content, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/topic", "GBK");
			aipHttpRequest.Bodys["title"] = title;
			aipHttpRequest.Bodys["content"] = content;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000A238 File Offset: 0x00008438
		public JObject Ecnet(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/ecnet", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000A2CC File Offset: 0x000084CC
		public JObject TextCorrection(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v2/text_correction", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000A360 File Offset: 0x00008560
		public JObject Emotion(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/emotion", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000A3F4 File Offset: 0x000085F4
		public JObject NewsSummary(string content, int maxSummaryLen, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/news_summary", "GBK");
			aipHttpRequest.Bodys["content"] = content;
			aipHttpRequest.Bodys["max_summary_len"] = maxSummaryLen;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000A49C File Offset: 0x0000869C
		public JObject CommentTagCustom(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v2/comment_tag_custom", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000A530 File Offset: 0x00008730
		public JObject SentimentClassifyCustom(string text)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/sentiment_classify_custom", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000A56C File Offset: 0x0000876C
		public JObject Couplets(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/creation/v1/couplets", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000A600 File Offset: 0x00008800
		public JObject Poem(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/creation/v1/poem", "UTF-8");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000A694 File Offset: 0x00008894
		public JObject EntityLevelSentiment(string title, string content, int type, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment", "GBK");
			aipHttpRequest.Bodys["text"] = title;
			aipHttpRequest.Bodys["content"] = content;
			aipHttpRequest.Bodys["type"] = type;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000A750 File Offset: 0x00008950
		public JObject EntityLevelSentimentAdd(string repository, string[] entities)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/add", "GBK");
			aipHttpRequest.Bodys["repository"] = repository;
			aipHttpRequest.Bodys["entities"] = entities;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000A7A0 File Offset: 0x000089A0
		public JObject EntityLevelSentimentDelete(string repository, string[] entities)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/delete", "GBK");
			aipHttpRequest.Bodys["repository"] = repository;
			aipHttpRequest.Bodys["entities"] = entities;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000A7F0 File Offset: 0x000089F0
		public JObject EntityLevelSentimentDeleteRepo(string repositories)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/delete_repo", "GBK");
			aipHttpRequest.Bodys["repositories"] = repositories;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000A82C File Offset: 0x00008A2C
		public JObject EntityLevelSentimentList()
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/list", "GBK");
			aipHttpRequest.Bodys[""] = null;
			aipHttpRequest.Bodys[""] = null;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000A87C File Offset: 0x00008A7C
		public JObject EntityLevelSentimentQuery(string repository)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/query", "GBK");
			aipHttpRequest.Bodys["repository"] = repository;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000A8B8 File Offset: 0x00008AB8
		public JObject TopicPhrase(string title, string summary, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/creation/v1/topic_phrase", "GBK");
			aipHttpRequest.Bodys["title"] = title;
			aipHttpRequest.Bodys["summary"] = summary;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000A95C File Offset: 0x00008B5C
		public JObject Cvparser(string filename, string filetype, string filedata, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/recruitment/v1/cvparser", "GBK");
			aipHttpRequest.Bodys["resume"] = new Dictionary<string, string>
			{
				{
					"filename",
					filename
				},
				{
					"filetype",
					filetype
				},
				{
					"filedata",
					filedata
				}
			};
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000AA18 File Offset: 0x00008C18
		public JObject PersonPost(Dictionary<string, object> resume, Dictionary<string, object> jobDescription)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/recruitment/v1/person_post", "GBK");
			aipHttpRequest.Bodys["resume"] = resume;
			aipHttpRequest.Bodys["job_description"] = jobDescription;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000AA68 File Offset: 0x00008C68
		public JObject Personas(object[] resume, string filename, string filetype, string filedata)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/recruitment/v1/personas", "GBK");
			aipHttpRequest.Bodys["resume"] = new Dictionary<string, string>
			{
				{
					"filename",
					filename
				},
				{
					"filetype",
					filetype
				},
				{
					"filedata",
					filedata
				}
			};
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000AAD0 File Offset: 0x00008CD0
		public JObject Titlepredictor(string doc)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/titlepredictor", "GBK");
			aipHttpRequest.Bodys["doc"] = doc;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000AB0C File Offset: 0x00008D0C
		public JObject Depparser(string text)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v2/depparser", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000AB48 File Offset: 0x00008D48
		public JObject BlessCreation(string text)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/bless_creation", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000AB84 File Offset: 0x00008D84
		public JObject EntityAnalysis(string text, Dictionary<string, object> options = null)
		{
			AipHttpRequest aipHttpRequest = this.DefaultRequest("https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_analysis", "GBK");
			aipHttpRequest.Bodys["text"] = text;
			base.PreAction();
			if (options != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in options)
				{
					aipHttpRequest.Bodys[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return this.PostAction(aipHttpRequest);
		}

		// Token: 0x0400007A RID: 122
		private const string LEXER = "https://aip.baidubce.com/rpc/2.0/nlp/v1/lexer";

		// Token: 0x0400007B RID: 123
		private const string LEXER_CUSTOM = "https://aip.baidubce.com/rpc/2.0/nlp/v1/lexer_custom";

		// Token: 0x0400007C RID: 124
		private const string DEP_PARSER = "https://aip.baidubce.com/rpc/2.0/nlp/v1/depparser";

		// Token: 0x0400007D RID: 125
		private const string WORD_EMBEDDING = "https://aip.baidubce.com/rpc/2.0/nlp/v2/word_emb_vec";

		// Token: 0x0400007E RID: 126
		private const string DNNLM_CN = "https://aip.baidubce.com/rpc/2.0/nlp/v2/dnnlm_cn";

		// Token: 0x0400007F RID: 127
		private const string WORD_SIM_EMBEDDING = "https://aip.baidubce.com/rpc/2.0/nlp/v2/word_emb_sim";

		// Token: 0x04000080 RID: 128
		private const string SIMNET = "https://aip.baidubce.com/rpc/2.0/nlp/v2/simnet";

		// Token: 0x04000081 RID: 129
		private const string COMMENT_TAG = "https://aip.baidubce.com/rpc/2.0/nlp/v2/comment_tag";

		// Token: 0x04000082 RID: 130
		private const string SENTIMENT_CLASSIFY = "https://aip.baidubce.com/rpc/2.0/nlp/v1/sentiment_classify";

		// Token: 0x04000083 RID: 131
		private const string KEYWORD = "https://aip.baidubce.com/rpc/2.0/nlp/v1/keyword";

		// Token: 0x04000084 RID: 132
		private const string TOPIC = "https://aip.baidubce.com/rpc/2.0/nlp/v1/topic";

		// Token: 0x04000085 RID: 133
		private const string ECNET = "https://aip.baidubce.com/rpc/2.0/nlp/v1/ecnet";

		// Token: 0x04000086 RID: 134
		private const string EMOTION = "https://aip.baidubce.com/rpc/2.0/nlp/v1/emotion";

		// Token: 0x04000087 RID: 135
		private const string NEWS_SUMMARY = "https://aip.baidubce.com/rpc/2.0/nlp/v1/news_summary";

		// Token: 0x04000088 RID: 136
		private const string COMMENT_TAG_CUSTOM = "https://aip.baidubce.com/rpc/2.0/nlp/v2/comment_tag_custom";

		// Token: 0x04000089 RID: 137
		private const string SENTIMENT_CLASSIFY_CUSTOM = "https://aip.baidubce.com/rpc/2.0/nlp/v1/sentiment_classify_custom";

		// Token: 0x0400008A RID: 138
		private const string COUPLETS = "https://aip.baidubce.com/rpc/2.0/creation/v1/couplets";

		// Token: 0x0400008B RID: 139
		private const string POEM = "https://aip.baidubce.com/rpc/2.0/creation/v1/poem";

		// Token: 0x0400008C RID: 140
		private const string ENTITY_LEVEL_SENTIMENT = "https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment";

		// Token: 0x0400008D RID: 141
		private const string ENTITY_LEVEL_SENTIMENT_ADD = "https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/add";

		// Token: 0x0400008E RID: 142
		private const string ENTITY_LEVEL_SENTIMENT_DELETE = "https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/delete";

		// Token: 0x0400008F RID: 143
		private const string ENTITY_LEVEL_SENTIMENT_DELETE_REPO = "https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/delete_repo";

		// Token: 0x04000090 RID: 144
		private const string ENTITY_LEVEL_SENTIMENT_LIST = "https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/list";

		// Token: 0x04000091 RID: 145
		private const string ENTITY_LEVEL_SENTIMENT_QUERY = "https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_level_sentiment/query";

		// Token: 0x04000092 RID: 146
		private const string TOPIC_PHRASE = "https://aip.baidubce.com/rpc/2.0/creation/v1/topic_phrase";

		// Token: 0x04000093 RID: 147
		private const string CVPARSER = "https://aip.baidubce.com/rpc/2.0/recruitment/v1/cvparser";

		// Token: 0x04000094 RID: 148
		private const string PERSON_POST = "https://aip.baidubce.com/rpc/2.0/recruitment/v1/person_post";

		// Token: 0x04000095 RID: 149
		private const string PERSONAS = "https://aip.baidubce.com/rpc/2.0/recruitment/v1/personas";

		// Token: 0x04000096 RID: 150
		private const string TITLEPREDICTOR = "https://aip.baidubce.com/rpc/2.0/nlp/v1/titlepredictor";

		// Token: 0x04000097 RID: 151
		private const string DEPPARSER = "https://aip.baidubce.com/rpc/2.0/nlp/v2/depparser";

		// Token: 0x04000098 RID: 152
		private const string BLESS_CREATION = "https://aip.baidubce.com/rpc/2.0/nlp/v1/bless_creation";

		// Token: 0x04000099 RID: 153
		private const string ENTITY_ANALYSIS = "https://aip.baidubce.com/rpc/2.0/nlp/v1/entity_analysis";

		// Token: 0x0400009A RID: 154
		private const string TEXT_CORRECTION = "https://aip.baidubce.com/rpc/2.0/nlp/v2/text_correction";
	}
}
