using MessagePack.Resolvers;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PlaymodeSettingsInstaller", menuName = "Installers/PlaymodeSettingsInstaller")]
public class PlaymodeSettingsInstaller : ScriptableObjectInstaller<PlaymodeSettingsInstaller>
{
    public WorldSettings settings;

    public override void InstallBindings()
    {
        InitializeMessagePack();
        Container.BindInstance(settings).AsSingle();
        Container.BindInterfacesAndSelfTo<WorldManager>().AsSingle().NonLazy();
    }

    private void InitializeMessagePack()
    {
        MessagePack.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
            // use generated resolver first, and combine many other generated/custom resolvers
            MessagePack.Resolvers.GeneratedResolver.Instance,

            // finally, use builtin/primitive resolver(don't use StandardResolver, it includes dynamic generation)
            MessagePack.Resolvers.BuiltinResolver.Instance,
            AttributeFormatterResolver.Instance,
            MessagePack.Resolvers.PrimitiveObjectResolver.Instance
        );
    }
}
