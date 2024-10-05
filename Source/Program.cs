using Rabid;
using System;

/* ---------- Terminology  -----------
 * Service - a static manager class
 * Manager - manages other class instances
 * Unbound Rpc - a set of RPC functions on a static class
 * 
 */
class Program
{
	static void Main(string[] args)
	{
		new Application().Run();
	}
}