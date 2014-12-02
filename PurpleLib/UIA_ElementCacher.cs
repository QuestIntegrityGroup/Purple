using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurpleLib
{
    public class UIA_ElementCacher
    {
        private List<UIA_ElementInfo> _ElementsInCache;
        private bool _cachedElements = false;
        public bool CachedElements { get { return _cachedElements; }}
        public List<UIA_ElementInfo> ElementsInCache { get { return _ElementsInCache; }}

        public UIA_ElementCacher()
        {
            _ElementsInCache = new List<UIA_ElementInfo>();
        }

        public void addElement(UIA_ElementInfo theElement)
        {
            _ElementsInCache.Add(theElement);
            _cachedElements = true;
        }
    }
}
