using MukbangMaker.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    public PrevNextMenu Selector;
    public TMP_Text CharNameText;
    public Toggle AutoProgToggle;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.SetMusicTrack("Ukulele");

        Selector.Value = CharacterManager.Instance.SelectedIndex;
        Selector.MinValue = 0;
        Selector.MaxValue = CharacterManager.Instance.SelectableCharacters.Count - 1;
    }

    // Update is called once per frame
    void Update()
    {
        CharacterManager.Instance.SelectedIndex = Selector.Value;
        CharNameText.text = CharacterManager.Instance.GetSelectedCharacter().info.DisplayName;
        CharacterManager.Instance.AutomaticProgression = AutoProgToggle.isOn;
    }

    public void SelectRandom()
    {
        int PrevValue = Selector.Value;
        Selector.Value = Random.Range(Selector.MinValue, Selector.MaxValue + 1);
        if(Selector.Value == PrevValue)
        {
            SelectRandom();
        }
    }

    public void Select()
    {
        SceneManager.LoadSceneAsync("Main");
    }
}
