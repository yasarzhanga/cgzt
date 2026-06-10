using System;

namespace Baidu.Aip
{
	// Token: 0x02000002 RID: 2
	[Serializable]
	public class AipException : Exception
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public AipException()
		{
			this.Code = -1;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205F File Offset: 0x0000025F
		public AipException(string message) : base(message)
		{
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002068 File Offset: 0x00000268
		public AipException(int code, string message) : base(message)
		{
			this.Code = code;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002078 File Offset: 0x00000278
		// (set) Token: 0x06000005 RID: 5 RVA: 0x00002080 File Offset: 0x00000280
		public int Code { get; set; }

		// Token: 0x06000006 RID: 6 RVA: 0x00002089 File Offset: 0x00000289
		public static AipException TokenException(string message)
		{
			return new AipException("Token request failed! " + message);
		}
	}
}
