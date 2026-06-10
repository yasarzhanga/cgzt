using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Baidu.Aip.Speech
{
	// Token: 0x0200000B RID: 11
	public class TlvPacket
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00003636 File Offset: 0x00001836
		public TlvPacket(TlvType type)
		{
			this.T = type;
			this.V = new byte[0];
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003651 File Offset: 0x00001851
		public TlvPacket(TlvType type, byte[] value)
		{
			this.T = type;
			this.V = new byte[value.Length];
			value.CopyTo(this.V, 0);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000367B File Offset: 0x0000187B
		public TlvPacket(TlvType type, byte[] value, int count)
		{
			this.T = type;
			this.V = new byte[value.Length];
			Array.Copy(value, this.V, count);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000036A5 File Offset: 0x000018A5
		public TlvPacket(TlvType type, byte[] value, int offset, int count)
		{
			this.T = type;
			this.V = new byte[value.Length];
			Array.Copy(value, offset, this.V, 0, count);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000036D2 File Offset: 0x000018D2
		public TlvPacket(TlvType type, string value)
		{
			this.T = type;
			this.V = Encoding.UTF8.GetBytes(value);
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000036F2 File Offset: 0x000018F2
		// (set) Token: 0x0600004B RID: 75 RVA: 0x000036FA File Offset: 0x000018FA
		public TlvType T { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00003703 File Offset: 0x00001903
		// (set) Token: 0x0600004D RID: 77 RVA: 0x0000370B File Offset: 0x0000190B
		public byte[] V { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00003714 File Offset: 0x00001914
		public int L
		{
			get
			{
				return this.V.Length;
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003720 File Offset: 0x00001920
		public byte[] ToBytes()
		{
			byte[] array = new byte[8 + this.L];
			int value = (int)this.T;
			int num = this.V.Length;
			if (BitConverter.IsLittleEndian)
			{
				value = IPAddress.HostToNetworkOrder((int)this.T);
				num = IPAddress.HostToNetworkOrder(num);
			}
			BitConverter.GetBytes(value).CopyTo(array, 0);
			BitConverter.GetBytes(num).CopyTo(array, 4);
			this.V.CopyTo(array, 8);
			return array;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000378D File Offset: 0x0000198D
		public static IEnumerable<TlvPacket> ParseFromBytes(byte[] data)
		{
			int j;
			for (int i = 0; i < data.Length; i += j)
			{
				int num = BitConverter.ToInt32(data, i);
				i += 4;
				j = BitConverter.ToInt32(data, i);
				i += 4;
				if (BitConverter.IsLittleEndian)
				{
					num = IPAddress.NetworkToHostOrder(num);
					j = IPAddress.NetworkToHostOrder(j);
				}
				yield return new TlvPacket((TlvType)num, data, i, j);
			}
			yield break;
		}
	}
}
