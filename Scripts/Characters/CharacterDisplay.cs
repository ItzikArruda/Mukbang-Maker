using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    public Material BaseMat;
    public GameObject CharModel;
    Material Mat;
    CharacterManager.Character character;

    // Start is called before the first frame update
    void Start()
    {
        Mat = new Material(BaseMat);
        Mat.name = "CharacterMat";
        CharModel.GetComponent<MeshRenderer>().sharedMaterial = Mat;
    }

    // Update is called once per frame
    void Update()
    {
        // Get The Currently Selected Character
        character = CharacterManager.Instance.GetSelectedCharacter();

        // Assign Texture
        Mat.mainTexture = character.CharacterTexture;
        // Assgn Size
        CharModel.transform.localScale = character.info.GetDimentions();
    }
}
