﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public enum NetMessageType : byte
	{
		UnboundRpc,
		BoundRpc,
		Genesis
	}
}
