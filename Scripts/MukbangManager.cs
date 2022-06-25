using MukbangMaker.Audio;
using System.Collections;
using UnityEngine;

public class MukbangManager : Manager<MukbangManager>
{
    public enum MukbangStates { Hello, Idle, Spinning, Picking, Selected, Eating, Finished }
    [Space]
    public MukbangStates MukbangState;
    [System.Serializable]
    public class ProgressableState
    {
        public MukbangStates State;
        public float MinAutoProgressTime;
        public float MaxAutoProgressTime;

        public float RandProgressionTime()
        {
            return Random.Range(MinAutoProgressTime, MaxAutoProgressTime + float.Epsilon);
        }
    }
    public ProgressableState[] ProgressableMukStates;
    public bool AutoProgression;
    public int MukbangStateIndex;
    public SpinningWheel Wheel;
    bool ProgressionTimerRunning;
    float ProgressionTimer;

    [Header("Camera And Animations")]
    public Transform DefaultComposition;
    [System.Serializable]
    public class CamComposition
    {
        public MukbangStates CompositionState;
        public Transform Composition;
    }
    public CamComposition[] CameraCompositions;
    Transform Cam;
    bool AnimatingEat;
    bool AnimatingEnd;

    [Space]
    public Animator PlayerAnim;
    public Animator EatHolder;

    [Header("Other Stuff")]
    public GameObject SelectedFood;
    public Transform SelectionComp;
    public ParticleSystem EatParticles;
    public TMPro.TMP_Text SelectedFoodText;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Manager Object
        Init(this);

        // Play Music
        AudioManager.Instance.SetMusicTrack("Breaktime");

        // Hide The Cursor So That The Viewers Dont Notice
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Locate Main Camera
        Cam = Camera.main.transform;

        // Assign The Auto Progression Toggle
        AutoProgression = CharacterManager.Instance.AutomaticProgression && CharacterManager.IsInstanced;

        // Start The Mukbang Seqence
        StartCoroutine(Begin());
    }
    IEnumerator Begin()
    {
        SoundEffect HelloSound = AudioManager.Instance.GetSound("Hello " + Random.Range(1, 5));
        HelloSound.Interact(SoundEffectBehaviour.Play);
        PlayerAnim.SetBool("Is Idle", false);
        yield return new WaitForSeconds(HelloSound.Clip.length);
        PlayerAnim.SetBool("Is Idle", true);
        StartCoroutine(Wheel.ReconstructWheel());
    }

    // Update is called once per frame
    void Update()
    {
        MukbangState = (MukbangStates)MukbangStateIndex;

        ProgressableState progressableState = null;
        bool CanProgress = IsProgressableState(out progressableState);
        if(CanProgress)
        {
            if(AutoProgression)
            {
                if(!ProgressionTimerRunning) { ProgressionTimerRunning = true; ProgressionTimer = progressableState.RandProgressionTime(); }
                else { ProgressionTimer -= Time.deltaTime; }
            }

            bool WantsToProgress = (AutoProgression) ? ProgressionTimer <= 0f : Input.GetKeyDown(KeyCode.Space);
            if(WantsToProgress)
            {
                if(AutoProgression) { ProgressionTimerRunning = false; }
                MukbangStateIndex++;
            }
        }

        switch (MukbangState)
        {
            case MukbangStates.Spinning:
            {
                Wheel.WheelState = SpinningWheel.WheelStates.Spinning;
                break;
            }
            case MukbangStates.Picking:
            {
                Wheel.WheelState = SpinningWheel.WheelStates.Slowing;
                break;
            }
            case MukbangStates.Eating:
            {
                Wheel.WheelState = SpinningWheel.WheelStates.Frozen;
                if(!AnimatingEat)
                {
                    StartCoroutine(EatAnimation());
                }
                break;
            }
            case MukbangStates.Finished:
            {
                Wheel.WheelState = SpinningWheel.WheelStates.Frozen;
                if(!AnimatingEnd)
                {
                    StartCoroutine(EndAnimation());
                }
                break;
            }
            default:
            {
                Wheel.WheelState = SpinningWheel.WheelStates.Frozen;
                break;
            }
        }
        if(SelectedFood != null) { SelectedFoodText.text = SelectedFood.name + "!"; }
        SelectedFoodText.enabled = MukbangState == MukbangStates.Selected;

        UpdateCameraCompositions();
    }
    void UpdateCameraCompositions()
    {
        for (int CMP = 0; CMP < CameraCompositions.Length; CMP++)
        {
            bool MatchingComp = MukbangState == CameraCompositions[CMP].CompositionState;
            CameraCompositions[CMP].Composition.gameObject.SetActive(MatchingComp);
            if(MatchingComp) { AssignCamComp(CameraCompositions[CMP].Composition); return; }
        }

        AssignCamComp(DefaultComposition);
    }
    void AssignCamComp(Transform Comp)
    {
        Cam.position = Comp.position;
        Cam.rotation = Comp.rotation;
    }

    bool IsProgressableState(out ProgressableState state)
    {
        for (int MS = 0; MS < ProgressableMukStates.Length; MS++)
        {
            if(MukbangState == ProgressableMukStates[MS].State) {  state = ProgressableMukStates[MS]; return true; }
        }

        state = null;
        return false;
    }

    public void SetState(MukbangStates State)
    {
        MukbangStateIndex = (int)State;
        MukbangState = State;
    }

    public void SetSelectedFood(GameObject FoodPrefab)
    {
        SelectedFood = Instantiate(FoodPrefab, SelectionComp.position, SelectionComp.rotation, SelectionComp);
        SelectedFood.name = FoodPrefab.name;
        AudioManager.Instance.InteractWithSFX("Picked Food", SoundEffectBehaviour.Play);
    }

    IEnumerator EatAnimation()
    {
        AnimatingEat = true;

        SelectedFood.transform.SetParent(EatHolder.transform);
        SelectedFood.transform.position = EatHolder.transform.position;

        SoundEffect LaughSound = AudioManager.Instance.GetSound("Laugh " + Random.Range(1, 5));
        LaughSound.Interact(SoundEffectBehaviour.Play);
        PlayerAnim.SetBool("Is Idle", false);
        yield return new WaitForSeconds(LaughSound.Clip.length);
        PlayerAnim.SetBool("Is Idle", true);
        EatHolder.SetBool("Eating", true);
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.InteractWithSFX("Eat Food", SoundEffectBehaviour.Play);
        yield return new WaitForSeconds(4f);
        PlayerAnim.SetBool("Is Idle", false);
        AudioManager.Instance.InteractWithSFX("Eat Food", SoundEffectBehaviour.Stop);
        EatParticles.Play();
        AudioManager.Instance.InteractWithSFX("Finish Eat", SoundEffectBehaviour.Play);
        yield return new WaitForSeconds(4f);
        PlayerAnim.SetBool("Is Idle", true);

        Destroy(SelectedFood);
        AnimatingEat = false;
        EatHolder.SetBool("Eating", false);
        
        StartCoroutine(Wheel.ReconstructWheel());
    }
    IEnumerator EndAnimation()
    {
        AnimatingEnd = true;

        AudioManager.Instance.InteractWithSFX("Bye Bye", SoundEffectBehaviour.Play);
        PlayerAnim.SetBool("Is Idle", false);
        yield return new WaitForSeconds(0.92f);
        PlayerAnim.SetBool("Is Idle", true);
        yield return new WaitForSeconds(0.5f);

        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #endif
    }

    public void FinishMukbang()
    {
        SetState(MukbangStates.Finished);
    }
}
