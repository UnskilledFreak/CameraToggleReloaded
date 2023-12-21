using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;

namespace CameraToggleReloaded.Helper
{
    public class ReloadedSceneEntry
    {
        [UIValue("Name")]
        public string Scene { get;  }
        public bool WhiteListed { get; }
        
        [UsedImplicitly]
        [UIValue("WhiteListedString")]
        public string WhiteListedString => WhiteListed 
            ? "<color=\"green\">Configured \u2713</color>" 
            : "<color=\"red\">not set up \u2717</color>";

        public ReloadedSceneEntry(string scene, bool whiteListed)
        {
            Scene = scene;
            WhiteListed = whiteListed;
        }
    }
}