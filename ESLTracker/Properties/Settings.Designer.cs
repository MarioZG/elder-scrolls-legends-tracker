﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ESLTracker.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase, ISettings {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00000000-0000-0000-0000-000000000000")]
        public global::System.Nullable<System.Guid> LastActiveDeckId {
            get {
                return ((global::System.Nullable<System.Guid>)(this["LastActiveDeckId"]));
            }
            set {
                this["LastActiveDeckId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowDeckStats {
            get {
                return ((bool)(this["ShowDeckStats"]));
            }
            set {
                this["ShowDeckStats"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool MinimiseOnClose {
            get {
                return ((bool)(this["MinimiseOnClose"]));
            }
            set {
                this["MinimiseOnClose"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        public double MainWindowPositionX {
            get {
                return ((double)(this["MainWindowPositionX"]));
            }
            set {
                this["MainWindowPositionX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        public double MainWindowPositionY {
            get {
                return ((double)(this["MainWindowPositionY"]));
            }
            set {
                this["MainWindowPositionY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public double OverlayWindowPositionX {
            get {
                return ((double)(this["OverlayWindowPositionX"]));
            }
            set {
                this["OverlayWindowPositionX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public double OverlayWindowPositionY {
            get {
                return ((double)(this["OverlayWindowPositionY"]));
            }
            set {
                this["OverlayWindowPositionY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("TheRitual")]
        public global::ESLTracker.DataModel.Enums.PlayerRank PlayerRank {
            get {
                return ((global::ESLTracker.DataModel.Enums.PlayerRank)(this["PlayerRank"]));
            }
            set {
                this["PlayerRank"] = value;
            }
        }
    }
}
