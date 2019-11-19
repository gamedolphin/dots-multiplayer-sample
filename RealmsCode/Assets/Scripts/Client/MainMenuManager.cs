using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuManager : MonoBehaviour
{
    [Inject]
    private ZenjectSceneLoader sceneLoader;
    // Start is called before the first frame update
    [SerializeField]
    private Text Name;

    [SerializeField]
    private Text Ip;

    [SerializeField]
    private Text Port;

    [SerializeField]
    private Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(() =>
        {
            if(Name.text == "" || Ip.text == "" || Port.text == "")
            {
                return;
            }

            sceneLoader.LoadScene("SimulationScene", UnityEngine.SceneManagement.LoadSceneMode.Single, (container) =>
            {
                container.BindInstance(new NetworkSettings {
                    Ip = Ip.text,
                    Name = Name.text,
                    Port = int.Parse(Port.text)
                }).WhenInjectedInto<PlaymodeSettingsInstaller>();
            });
        });
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
    }
}
