using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctopusController;

public class IK_Scorpion : MonoBehaviour
{
    MyScorpionController _myController= new MyScorpionController();

    public IK_tentacles _myOctopus;

    [SerializeField] public GameObject strengthSlider;
    [SerializeField] public GameObject magnusEffectSlider;


    [Header("Body")]
    float animTime;
    public float animDuration = 5;
    bool animPlaying = false;
    public Transform Body;
    public Transform StartPos;
    public Transform EndPos;

    [Header("Tail")]
    public Transform tailTarget;
    public Transform tail;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;
    public Transform[] raycastFutureLegBases;

    float _initBodyHeight;

    float _averageLegHeight;

    bool _isPressingStrength;
    bool _isStrengthAugmenting;

    // Start is called before the first frame update
    void Start()
    {
        _myController.InitLegs(legs,futureLegBases,legTargets);
        _myController.InitTail(tail);

        _averageLegHeight = LegAverageHeight();
        SetInitPositions();

        _isPressingStrength = false;
        _isStrengthAugmenting = true;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Z))
        {
            magnusEffectSlider.GetComponent<UnityEngine.UI.Slider>().value -= Time.deltaTime * 2.0f;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            magnusEffectSlider.GetComponent<UnityEngine.UI.Slider>().value += Time.deltaTime * 2.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPressingStrength = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isPressingStrength = false;
        }

        if (_isPressingStrength)
        {
            if (_isStrengthAugmenting)
            {
                strengthSlider.GetComponent<UnityEngine.UI.Slider>().value += Time.deltaTime * 1f;
                if (strengthSlider.GetComponent<UnityEngine.UI.Slider>().value >= strengthSlider.GetComponent<UnityEngine.UI.Slider>().maxValue)
                {
                    _isStrengthAugmenting = false;
                }
            }
            else
            {
                strengthSlider.GetComponent<UnityEngine.UI.Slider>().value -= Time.deltaTime * 1f;
                if (strengthSlider.GetComponent<UnityEngine.UI.Slider>().value <= strengthSlider.GetComponent<UnityEngine.UI.Slider>().minValue)
                {
                    _isStrengthAugmenting = true;
                }
            }
        }

        if (animPlaying)
            animTime += Time.deltaTime;

        NotifyTailTarget();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NotifyStartWalk();
            animTime = 0;
            animPlaying = true;
        }

        if (animTime < animDuration)
        {
            Body.position = Vector3.Lerp(StartPos.position, EndPos.position, animTime / animDuration);

            UpdateAllValues();
        }
        else if (animTime >= animDuration && animPlaying)
        {
            Body.position = EndPos.position;
            animPlaying = false;
        }

        UpdateAllValues();

        _myController.UpdateIK();
    }
    
    //Function to send the tail target transform to the dll
    public void NotifyTailTarget()
    {
        _myController.NotifyTailTarget(tailTarget);
        _myController.UpdateSliderValues(magnusEffectSlider.GetComponent<UnityEngine.UI.Slider>().value, strengthSlider.GetComponent<UnityEngine.UI.Slider>().value);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {

        _myController.NotifyStartWalk();
    }

    private void SetInitPositions()
    {
        _initBodyHeight = Body.transform.position.y;
    }

    private float LegAverageHeight()
    {
        float averageHeight = 0.0f;
        for (int i = 0; i < futureLegBases.Length; i++)
        {
            averageHeight += futureLegBases[i].position.y;
        }

        return (averageHeight /= futureLegBases.Length);
    }

    private void UpdateFutureLegBases()
    {
        for (int i = 0; i < futureLegBases.Length; i++)
        {
            RaycastHit hit;

            if (Physics.Raycast(raycastFutureLegBases[i].position, Vector3.down, out hit, 2.0f))
            {
                futureLegBases[i].transform.position = hit.point;
            }
        }
    }
    private void UpdateBodyPosition()
    {
        float newAverageHeight = LegAverageHeight();
        float heightDiff = newAverageHeight - _averageLegHeight;

        Vector3 bPos = Body.transform.position;
        Body.transform.position = new Vector3(bPos.x, _initBodyHeight + heightDiff, bPos.z);
    }

    private void UpdateBodyRotation()
    {
        //get the planes
        Vector3 normalVector1 = Vector3.Cross((futureLegBases[1].position - futureLegBases[2].position),
            (futureLegBases[0].position - futureLegBases[2].position));

        Vector3 normalVector2 = Vector3.Cross((futureLegBases[4].position - futureLegBases[3].position),
            (futureLegBases[5].position - futureLegBases[3].position));

        Vector3 resultingNormal = (normalVector1 + normalVector2) / 2.0f;

        Body.transform.up = resultingNormal;
    }

    private void UpdateAllValues()
    {
        UpdateBodyPosition();
        UpdateFutureLegBases();
        UpdateBodyRotation();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < futureLegBases.Length; i++)
        {
            Gizmos.DrawSphere(futureLegBases[i].transform.position, 0.1f);
        }
    }
}
