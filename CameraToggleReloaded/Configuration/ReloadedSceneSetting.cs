using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Camera2.Configuration;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace CameraToggleReloaded.Configuration
{
    [UsedImplicitly]
    internal class ReloadedSceneSetting
    {
        public virtual string Scene { get; set; } = "";
        public virtual bool UseCommand { get; set; }
        public virtual string Command { get; set; } = "";
        //public virtual bool UseReward { get; set; }
        //public virtual string Reward { get; set; } = "";

        [UseConverter(typeof(ListConverter<SceneTypes>))]
        public virtual List<SceneTypes> ValidForSceneTypes { get; set; } = new List<SceneTypes>();

        //public bool IsEnabled => UseCommand || UseReward;
        public bool IsEnabled => UseCommand;
    }
}