﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vortragsmanager.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int LogLevel {
            get {
                return ((int)(this["LogLevel"]));
            }
            set {
                this["LogLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-")]
        public string LogFolder {
            get {
                return ((string)(this["LogFolder"]));
            }
            set {
                this["LogFolder"] = value;
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
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SaveBackups {
            get {
                return ((bool)(this["SaveBackups"]));
            }
            set {
                this["SaveBackups"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://raw.githubusercontent.com/thomasramm/Vortragsmanager/master/Changelog.md")]
        public string ChangelogPfad {
            get {
                return ((string)(this["ChangelogPfad"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SearchSpeaker_Kreis {
            get {
                return ((string)(this["SearchSpeaker_Kreis"]));
            }
            set {
                this["SearchSpeaker_Kreis"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ListCreate_OpenFile {
            get {
                return ((bool)(this["ListCreate_OpenFile"]));
            }
            set {
                this["ListCreate_OpenFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Office2016White")]
        public string ApplicationThemeName {
            get {
                return ((string)(this["ApplicationThemeName"]));
            }
            set {
                this["ApplicationThemeName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int SonntagAnzeigeMonate {
            get {
                return ((int)(this["SonntagAnzeigeMonate"]));
            }
            set {
                this["SonntagAnzeigeMonate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowActivityButtons {
            get {
                return ((bool)(this["ShowActivityButtons"]));
            }
            set {
                this["ShowActivityButtons"] = value;
            }
        }
    }
}
