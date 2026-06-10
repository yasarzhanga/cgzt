using System;

namespace Baidu.Aip.Speech
{
	// Token: 0x0200000A RID: 10
	public enum TlvType
	{
		// Token: 0x0400001C RID: 28
		Unknown,
		// Token: 0x0400001D RID: 29
		AsrBegin = 16,
		// Token: 0x0400001E RID: 30
		AsrData,
		// Token: 0x0400001F RID: 31
		AsrEnd,
		// Token: 0x04000020 RID: 32
		AsrResult = 80,
		// Token: 0x04000021 RID: 33
		AsrResultFinish
	}
}
