﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vortragsmanager.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.4.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("vortragsmanager.sqlite3")]
        public string sqlite {
            get {
                return ((string)(this["sqlite"]));
            }
            set {
                this["sqlite"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Name des Kreisaufseher")]
        public string NameKreisaufseher {
            get {
                return ((string)(this["NameKreisaufseher"]));
            }
            set {
                this["NameKreisaufseher"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2000-01-01")]
        public global::System.DateTime NextUpdateSearch {
            get {
                return ((global::System.DateTime)(this["NextUpdateSearch"]));
            }
            set {
                this["NextUpdateSearch"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SearchForUpdates {
            get {
                return ((bool)(this["SearchForUpdates"]));
            }
            set {
                this["SearchForUpdates"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SearchSpeaker_RednerCheckHistory {
            get {
                return ((bool)(this["SearchSpeaker_RednerCheckHistory"]));
            }
            set {
                this["SearchSpeaker_RednerCheckHistory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SearchSpeaker_RednerCheckFuture {
            get {
                return ((bool)(this["SearchSpeaker_RednerCheckFuture"]));
            }
            set {
                this["SearchSpeaker_RednerCheckFuture"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SearchSpeaker_VortragCheckFuture {
            get {
                return ((bool)(this["SearchSpeaker_VortragCheckFuture"]));
            }
            set {
                this["SearchSpeaker_VortragCheckFuture"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SearchSpeaker_VortragCheckHistory {
            get {
                return ((bool)(this["SearchSpeaker_VortragCheckHistory"]));
            }
            set {
                this["SearchSpeaker_VortragCheckHistory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int SearchSpeaker_MaxEntfernung {
            get {
                return ((int)(this["SearchSpeaker_MaxEntfernung"]));
            }
            set {
                this["SearchSpeaker_MaxEntfernung"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DashboardShowDetails {
            get {
                return ((bool)(this["DashboardShowDetails"]));
            }
            set {
                this["DashboardShowDetails"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SearchSpeaker_OffeneAnfrage {
            get {
                return ((bool)(this["SearchSpeaker_OffeneAnfrage"]));
            }
            set {
                this["SearchSpeaker_OffeneAnfrage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SearchSpeaker_RednerCheckCancelation {
            get {
                return ((bool)(this["SearchSpeaker_RednerCheckCancelation"]));
            }
            set {
                this["SearchSpeaker_RednerCheckCancelation"] = value;
            }
        }
    }
}
