using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MukbangMaker.Audio;

[RequireComponent(typeof(Rotator))]
public class SpinningWheel : MonoBehaviour
{
    public float WheelRadius;
    public enum WheelStates { Frozen, Spinning, Slowing }
    public WheelStates WheelState;
    public float BaseWheelSpeed;
    public float WheelSlowdownTime;
    Rotator Rot;
    class RotRange
    {
        public float StartRot;
        public float EndRot;

        public bool AngleFits(float Angle)
        {
            return (Angle >= StartRot && Angle <= EndRot);
        }
    }
    List<GameObject> Foods;
    List<RotRange> Rotations;

    // Start is called before the first frame update
    void Start()
    {
        Foods = new List<GameObject>(FoodManager.Instance.FoodPrefabs);
        Rotations = new List<RotRange>();

        Rot = GetComponent<Rotator>();
    }

    public IEnumerator ReconstructWheel()
    {
        MukbangManager.Instance.SetState(MukbangManager.MukbangStates.Hello);

        Rot.Freeze();
        if(transform.childCount > 0)
        {
            Rotations.Clear();
            for (int FC = 0; FC < transform.childCount; FC++)
            {
                Destroy(transform.GetChild(FC).gameObject);
            }
        }

        float Angle = 0f;
        float Gaps = 360f / Foods.Count;
        for (int F = 0; F < Foods.Count; F++)
        {
            // Create Food Object Parent
            GameObject FoodParent = CreateFoodParent(Angle, F);

            // Create New Rotation Range For The Food Object
            RotRange rotRange = new RotRange();
            rotRange.StartRot = Angle - (Gaps / 2f);
            rotRange.EndRot = Angle + (Gaps / 2f);
            Rotations.Add(rotRange);

            // Create A New Food Object
            GameObject NewFood = Instantiate(Foods[F], Vector3.zero, Quaternion.identity, FoodParent.transform);
            NewFood.transform.localPosition = Vector3.up * WheelRadius;
            NewFood.transform.localRotation = Quaternion.identity;

            // Increase Angle
            Angle += Gaps;

            // Play New Food Sound
            AudioManager.Instance.InteractWithSFX("New Food", SoundEffectBehaviour.Play);

            // Wait A Little
            yield return new WaitForSeconds(1f / Foods.Count);
        }

        if(Foods.Count == 0)
        {
            MukbangManager.Instance.FinishMukbang();
            yield break;
        }
        else { MukbangManager.Instance.SetState(MukbangManager.MukbangStates.Idle); }
    }

    // Update is called once per frame
    void Update()
    {
        switch (WheelState)
        {
            case WheelStates.Frozen:
            {
                Rot.Speed = 0f;
                AudioManager.Instance.InteractWithSFXOneShot("Wheel Spin", SoundEffectBehaviour.Stop);
                break;
            }
            case WheelStates.Spinning:
            {
                Rot.Speed = BaseWheelSpeed;
                break;
            }
            case WheelStates.Slowing:
            {
                Rot.Speed -= (BaseWheelSpeed / WheelSlowdownTime) * Time.deltaTime;
                if(Rot.Speed <= 0f)
                {
                    WheelState = WheelStates.Frozen;
                    PickFood();
                }
                break;
            }
        }

        if((int)WheelState > 0) { AudioManager.Instance.InteractWithSFXOneShot("Wheel Spin", SoundEffectBehaviour.Play); }
    }

    public GameObject CreateFoodParent(float Angle, int Index)
    {
        GameObject Result = new GameObject("FoodParent_" + Index);
        Result.transform.SetParent(transform);
        Result.transform.position = transform.position;
        Result.transform.rotation = Quaternion.Euler(0f, 0f, Angle);

        return Result;
    }

    void PickFood()
    {
        MukbangManager.Instance.SetState(MukbangManager.MukbangStates.Selected);

        for (int R = 0; R < Rotations.Count; R++)
        {
            if(Rotations[R].AngleFits(Rot.Rot))
            {
                int FoodIndex = Foods.Count - R;
                if(FoodIndex < 0 || FoodIndex > Foods.Count - 1) { FoodIndex = 0; }

                SetSelectedFood(FoodIndex);
                return;
            }
        }

        SetSelectedFood(0);
    }
    void SetSelectedFood(int Index)
    {
        MukbangManager.Instance.SetSelectedFood(Foods[Index]);
        Rotations.RemoveAt(Index);
        Foods.RemoveAt(Index);
    }

    float HermisphereRotation(float Angle)
    {
        if(Angle > 180f) { return (-180f) + (Angle - 180f); }

        return Angle;
    }
}
