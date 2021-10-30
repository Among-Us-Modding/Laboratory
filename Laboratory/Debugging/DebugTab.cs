using System;

namespace Laboratory.Debugging
{
    public class DebugTab
    {
        /// <summary>
        /// Tabs used in the debug window
        /// </summary>
        /// <param name="tabName">Name of the tab</param>
        /// <param name="buildUI">Action which will build the contents of the tab</param>
        /// <param name="visible">Func which should return if the tab is visible - defaults to always visible</param>
        public DebugTab(string tabName, Action buildUI, Action onGUI = null, Func<bool> visible = null)
        {
            TabName = tabName;
            BuildUI = buildUI;
            OnGUI = onGUI;
            Visible = visible ?? (() => true);
        }
        
        public string TabName { get; }
        public Action BuildUI { get; }
        public Action OnGUI { get; }
        public Func<bool> Visible { get; }
    }
}