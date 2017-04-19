// HyperealVR Defines|SDK_HyperealVR|001
namespace VRTK
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Handles all the scripting define symbols for the HyperealVR SDK.
    /// </summary>
    public static class SDK_HyperealVRDefines
    {
        /// <summary>
        /// The scripting define symbol for the HyperealVR SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_HYPEREALVR";

        private const string BuildTargetGroupName = "Standalone";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "HYPEREALVR_PLUGIN_1_0_OR_NEWER", BuildTargetGroupName)]
        private static bool IsPluginVersion1OrNewer()
        {
            return true;
        }
    }
}