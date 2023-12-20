using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace CameraToggleReloaded.Configuration
{
    [UsedImplicitly]
    internal class ReloadedConfig
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual string CommandInitiator { get; set; } = "!ctr";
        public virtual int Cooldown { get; set; } = 10; // 10 sec default cooldown
        public virtual bool CooldownIsGlobal { get; set; } = true;
        public virtual bool ModsCanBypassCooldown { get; set; } = true;
        public virtual bool SendInfos { get; set; } = true;
        public virtual bool AddPreventChar { get; set; }
        public virtual string PreventChar { get; set; } = "!";
        [UseConverter(typeof(ListConverter<ReloadedSceneSetting>))]
        public virtual List<ReloadedSceneSetting> Scenes { get; set; } = new List<ReloadedSceneSetting>();
    }
}