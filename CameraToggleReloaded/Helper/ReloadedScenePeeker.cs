using Newtonsoft.Json;

namespace CameraToggleReloaded.Helper
{
    public class ReloadedScenePeeker
    {
        [JsonProperty("autoswitchFromCustom")]
        public bool AutoSwitchFromCustom { get; set; }
    }
}