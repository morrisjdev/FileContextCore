using System;
using System.Collections.Generic;
using System.Text;

namespace FileContextCore.ValueGeneration.Internal
{
    class FileContextIntegerValueGeneratorCache
    {
		public readonly Dictionary<string, Dictionary<string, long>> LastIds = new Dictionary<string, Dictionary<string, long>>();
	}
}
