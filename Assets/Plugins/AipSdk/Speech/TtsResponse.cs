using System;

namespace Baidu.Aip.Speech
{
	// Token: 0x0200000C RID: 12
	public class TtsResponse
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000051 RID: 81 RVA: 0x0000379D File Offset: 0x0000199D
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000037A5 File Offset: 0x000019A5
		public int ErrorCode { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000037AE File Offset: 0x000019AE
		// (set) Token: 0x06000054 RID: 84 RVA: 0x000037B6 File Offset: 0x000019B6
		public string ErrorMsg { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000037BF File Offset: 0x000019BF
		// (set) Token: 0x06000056 RID: 86 RVA: 0x000037C7 File Offset: 0x000019C7
		public string Sn { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000057 RID: 87 RVA: 0x000037D0 File Offset: 0x000019D0
		// (set) Token: 0x06000058 RID: 88 RVA: 0x000037D8 File Offset: 0x000019D8
		public int Idx { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000059 RID: 89 RVA: 0x000037E1 File Offset: 0x000019E1
		// (set) Token: 0x0600005A RID: 90 RVA: 0x000037E9 File Offset: 0x000019E9
		public byte[] Data { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005B RID: 91 RVA: 0x000037F2 File Offset: 0x000019F2
		public bool Success
		{
			get
			{
				return this.ErrorCode == 0;
			}
		}
	}
}
