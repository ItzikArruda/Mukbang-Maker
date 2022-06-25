using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CharacterManager : Manager<CharacterManager>
{
    [System.Serializable]
    public class Character
    {
        public Texture CharacterTexture;
        public CharacterInfo info;
    }
    [Space]
    public List<Character> SelectableCharacters;
    public int SelectedIndex;
    public bool AutomaticProgression;

    // Awake Is Called When The Script Instance Is Loaded.
    void Awake()
    {
        // Initialize Manager Object.
        Init(this);

        // Get A Path Leading To A Directory Where The Characters Are Stored.
        string CharDirPath = Application.dataPath + "/Characters/";
        // Get Paths To All Character Directories Found in The Characters Directory.
        string[] FoundCharFolders = Directory.GetDirectories(CharDirPath);
        // Add Empty Slots To The Characters List Depending On
        // The Amount Of Found Character Directories.
        for (int C = 0; C < FoundCharFolders.Length; C++) { SelectableCharacters.Add(null); }
        // For All Of The Found Character Directory's Paths...
        foreach (string F in FoundCharFolders)
        {
            // Create New Character Class
            Character character = new Character();

            // Load Character Info
            // Load The Character Info Class From The Found
            // Character Directory We Are Currently Looping Over.
            CharacterInfo CharInf = CharacterInfo.LoadJSON(F);
            // Apply The Loaded Character Info To The New Character Class.
            character.info = CharInf;

            // Load Image =>
            // Load Image Data From An Image File (Only PNG Supported) In The
            // Found Character Directory We Are Currently Looping Over.
            byte[] PNGImageData = File.ReadAllBytes(F + "/CharImage.png");
            // Creating The Character Texture =>
            // Create A New Texture Class.
            Texture2D NewCharTexture = new Texture2D(1, 1);
            // Apply The Loaded Image File Data To The New Texture Class.
            NewCharTexture.LoadImage(PNGImageData);
            // Name The New Texture Class.
            NewCharTexture.name = character.info.DisplayName + "_CharacterTexture_" + NewCharTexture.GetHashCode();
            // Disable Filtering On The New Texture If Filtering Is Disabled In The Loaded Character Info.
            NewCharTexture.filterMode = (character.info.DisableImageFiltering) ? FilterMode.Point : FilterMode.Bilinear;
            // Apply The Settings Of The New Texture.
            NewCharTexture.Apply();
            // Apply The Texture To The New Character.
            character.CharacterTexture = NewCharTexture;

            // At Last, Add The New Character To Characters List At
            // The Given Index Stored In The Loaded Character Info.
            SelectableCharacters[character.info.Index] = character;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Clamp The Selected Index So It Wont Go Out Of Range.
        SelectedIndex = Mathf.Clamp(SelectedIndex, 0, SelectableCharacters.Count - 1);
    }

    // Get The Character Class Found In The Characters List At The Selected Index.
    public Character GetSelectedCharacter()
    {
        return SelectableCharacters[SelectedIndex];
    }
}
