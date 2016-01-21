// Copyright ExM
// Licensed as LGPL
// Source at git://github.com/ExM/OncRpc.git

using System;

namespace Stellar.Generated
{
	public interface IByteReader
	{
		byte[] Read(uint count);
		byte Read();
	}
}
