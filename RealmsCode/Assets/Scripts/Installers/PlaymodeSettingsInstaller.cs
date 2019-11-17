using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PlaymodeSettingsInstaller", menuName = "Installers/PlaymodeSettingsInstaller")]
public class PlaymodeSettingsInstaller : ScriptableObjectInstaller<PlaymodeSettingsInstaller>
{
    public WorldSettings settings;

    public override void InstallBindings()
    {
        Container.BindInstance(settings).AsSingle();
        Container.BindInterfacesAndSelfTo<WorldManager>().AsSingle().NonLazy();
    }
}
