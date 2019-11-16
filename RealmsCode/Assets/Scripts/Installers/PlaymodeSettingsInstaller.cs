using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PlaymodeSettingsInstaller", menuName = "Installers/PlaymodeSettingsInstaller")]
public class PlaymodeSettingsInstaller : ScriptableObjectInstaller<PlaymodeSettingsInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<WorldManager>().AsSingle().NonLazy();
    }
}
