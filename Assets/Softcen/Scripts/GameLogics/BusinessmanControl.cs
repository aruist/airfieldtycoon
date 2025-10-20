using UnityEngine;
using System.Collections;

public class BusinessmanControl : MonoBehaviour {
    public SpeechBubble speechBubble;
    //public Sprite[] chapterSprites;
    public SkinnedMeshRenderer smr;
    public Vector3 smrAdjust;
    public Vector3 smrSize;
    public Transform trCamTarget;
    public string[] animNames;
    public Animator anim;
    public float animChangeTime = 30f;
    public int animChangeChance = 50;
    //public Transform[] positions;
    private GameManager gm;
    void Start()
    {
        PlayerData_OnLevelChanged();
    }

    void OnDisable()
    {
        speechBubble.gameObject.SetActive(false);
        CancelInvoke();
        //PlayerData.OnLevelChanged -= PlayerData_OnLevelChanged;
#if SOFTCEN_DEBUG
        Debug.Log("BusinessmanControl OnDisable");
        //DevOptions.OnReset -= DevOptions_OnReset;
#endif
    }

    void OnEnable()
    {
        Invoke("CharAnimations", animChangeTime);
        CheckSpeechBubble();

        //PlayerData.OnLevelChanged += PlayerData_OnLevelChanged;
#if SOFTCEN_DEBUG
        Debug.Log("BusinessmanControl OnEnable");
        //DevOptions.OnReset += DevOptions_OnReset;
#endif
    }

#if SOFTCEN_DEBUG
    /*private void DevOptions_OnReset()
    {
        PlayerData_OnLevelChanged();
    }*/
#endif

#if SOFTCEN_DEBUG
    /*void Update()
    {
        UpdateBounds();
    }*/
#endif

    public void UpdatePosition(Transform trNew)
    {
        speechBubble.gameObject.SetActive(false);
        if (trNew != null)
        {
            transform.position = trNew.position;
            transform.rotation = trNew.rotation;
            UpdateBounds();
        }
    }

    private void PlayerData_OnLevelChanged()
    {
        /*
        if (positions.Length > 0)
        {
            int level = GameManager.Instance.playerData.Level;
            if (level >= positions.Length)
                level = positions.Length - 1;

            transform.position = positions[level].position;
            transform.rotation = positions[level].rotation;
        }
        UpdateBounds();
        */
    }

    private void UpdateBounds()
    {
        if (smr != null)
        {
            Vector3 center = transform.position + smrAdjust;
            Bounds b = new Bounds(center, smrSize);
            smr.localBounds = b;
        }
    }

    private void CharAnimations()
    {
        AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
        //Debug.Log("CharAnimations is Idle: " + asi.IsName("Idle"));
        if (asi.IsName("Idle"))
        {
            int change = Random.Range(0, 100);
            if (change < animChangeChance)
            {
                int index = Random.Range(0, animNames.Length);
                anim.SetTrigger(animNames[index]);
            }
        }
        Invoke("CharAnimations", animChangeTime);
    }

    private int speechBubbleIndex = 0;
    private float speechBubbleTimeout;
    private void CheckSpeechBubble()
    {
        gm = GameManager.Instance;
        if (gm != null)
        {
#if SOFTCEN_DEBUG
            /*Debug.Log("CheckSpeechBubble Chpater: " + gm.playerData.CurrentChapter + ", Phase: " + gm.playerData.CurrentPhase
                + ", Index: " + speechBubbleIndex
                );*/
#endif

            if (gm.selectedChapter == 1)
                Chapter1();
            else if (gm.selectedChapter == 2)
                Chapter2();
            else if (gm.selectedChapter == 3)
                Chapter3();
            else if (gm.selectedChapter == 4)
                Chapter4();
            else if (gm.selectedChapter == 5)
                Chapter5();
        }
    }
    private void Chapter1()
    {
        float invokeTime = 60f;

        if (gm.selectedChapter < gm.playerData.CurrentChapter)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Boss, Welcome back to Airfield 1";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 1;
                speechBubble.fadeOut = true;
                invokeTime = 0f;
            }

        }
        else if (gm.playerData.CurrentChapter == 1 && gm.playerData.CurrentPhase == 0)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                //speechBubble.imgSpeech.sprite = chapterSprites[0];
                speechBubble.txtSpeech.text = "Let's create something like this!";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubble.newText = "Tap screen to make progress!";
                speechBubble.startTextChange = true;
                speechBubbleIndex++;
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 3)
            {
                speechBubble.newText = "Buy upgrades to speed up progress!";
                speechBubble.startTextChange = true;
                speechBubbleIndex++;
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 4)
            {
                speechBubbleIndex = 0;
                speechBubble.fadeOut = true;
                speechBubbleIndex++;
                invokeTime = 60f;
            }
            else if (speechBubbleIndex == 5)
            {
                int rnd = Random.Range(0, 100);
                if (rnd < 33)
                    speechBubble.txtSpeech.text = "One day, I get this finished!";
                else if (rnd < 66)
                    speechBubble.txtSpeech.text = "Tap, tap, tap...";
                else
                    speechBubble.txtSpeech.text = "How are you doing?";

                speechBubble.gameObject.SetActive(true);
                speechBubbleIndex++;
                invokeTime = 6f;
            }
            else if (speechBubbleIndex == 6)
            {
                speechBubbleIndex = 5;
                speechBubble.fadeOut = true;
                invokeTime = 60f;
            }
        }
        else if (gm.playerData.CurrentChapter == 1 && gm.playerData.CurrentPhase > 0)
        {
            speechBubbleIndex = 0;
            return;
        }

        if (invokeTime > 0)
            Invoke("CheckSpeechBubble", invokeTime);

    }
    private void Chapter2()
    {
        float invokeTime = 60f;

        if (gm.selectedChapter < gm.playerData.CurrentChapter)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Boss, Welcome back to Airfield 2";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 1;
                speechBubble.fadeOut = true;
                return;
            }

        }
        else if (gm.playerData.CurrentChapter == 2 && gm.playerData.CurrentPhase == 0)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Yes! Second Airfield!";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubble.fadeOut = true;
                speechBubbleIndex = 5;
                invokeTime = 60f;
            }

            else if (speechBubbleIndex == 5)
            {
                int rnd = Random.Range(0, 100);
                if (rnd < 33)
                    speechBubble.txtSpeech.text = "One day, I get this finished!";
                else if (rnd < 66)
                    speechBubble.txtSpeech.text = "Tap, tap, tap...";
                else
                    speechBubble.txtSpeech.text = "How are you doing?";

                speechBubble.gameObject.SetActive(true);
                speechBubbleIndex++;
                invokeTime = 6f;
            }
            else if (speechBubbleIndex == 6)
            {
                speechBubbleIndex = 5;
                speechBubble.fadeOut = true;
                invokeTime = 60f;
            }
        }
        else if (gm.playerData.CurrentChapter == 2 && gm.playerData.CurrentPhase > 0)
        {
            speechBubbleIndex = 0;
            return;
        }
        Invoke("CheckSpeechBubble", invokeTime);

    }

    private void Chapter3()
    {
        float invokeTime = 60f;

        if (gm.selectedChapter < gm.playerData.CurrentChapter)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Boss, Welcome back to Airfield 3";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 1;
                speechBubble.fadeOut = true;
                return;
            }

        }
        else if (gm.playerData.CurrentChapter == 3 && gm.playerData.CurrentPhase == 0)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Well Done! Third Airfield!";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubble.fadeOut = true;
                speechBubbleIndex = 5;
                invokeTime = 60f;
            }

            else if (speechBubbleIndex == 5)
            {
                int rnd = Random.Range(0, 100);
                if (rnd < 33)
                    speechBubble.txtSpeech.text = "Business people need the airfield!";
                else if (rnd < 66)
                {
                    if (GameManager.Instance.playerData.AutoTapOwned)
                    {
                        speechBubble.txtSpeech.text = "Hold finger down!";
                    }
                    else
                    {
                        speechBubble.txtSpeech.text = "Auto Tap would be handy!";
                    }
                }
                else
                    speechBubble.txtSpeech.text = "How's it going?";

                speechBubble.gameObject.SetActive(true);
                speechBubbleIndex++;
                invokeTime = 6f;
            }
            else if (speechBubbleIndex == 6)
            {
                speechBubbleIndex = 5;
                speechBubble.fadeOut = true;
                invokeTime = 60f;
            }
        }
        else if (gm.playerData.CurrentChapter == 3 && gm.playerData.CurrentPhase > 0)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {

                if (!GameManager.Instance.playerData.AutoTapOwned)
                {
                    speechBubble.txtSpeech.text = "Auto Tap would be handy!";
                    speechBubbleIndex++;
                    speechBubble.gameObject.SetActive(true);
                    invokeTime = 10f;
                }
                else
                {
                    speechBubbleIndex = 0;
                    speechBubble.fadeOut = true;
                    return;
                }
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 0;
                speechBubble.fadeOut = true;
                return;
            }

            else
            {
                speechBubbleIndex = 0;
                return;
            }

        }
        Invoke("CheckSpeechBubble", invokeTime);
    }

    private void Chapter4()
    {
        float invokeTime = 60f;

        if (gm.selectedChapter < gm.playerData.CurrentChapter)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Boss, Welcome back to Airfield 4";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 1;
                speechBubble.fadeOut = true;
                return;
            }

        }
        else if (gm.playerData.CurrentChapter == 4 && gm.playerData.CurrentPhase == 0)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Let's make something like this!";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubble.fadeOut = true;
                speechBubbleIndex = 5;
                invokeTime = 60f;
            }

            else if (speechBubbleIndex == 5)
            {
                int rnd = Random.Range(0, 100);
                if (rnd < 33)
                    speechBubble.txtSpeech.text = "Meridian people need the airfield!";
                else if (rnd < 66)
                {
                    if (GameManager.Instance.playerData.AutoTapOwned)
                    {
                        speechBubble.txtSpeech.text = "Hold finger down!";
                    }
                    else
                    {
                        speechBubble.txtSpeech.text = "Auto Tap would be handy!";
                    }
                }
                else
                    speechBubble.txtSpeech.text = "To wrongs don't make a right but to wrights make an airplane.";

                speechBubble.gameObject.SetActive(true);
                speechBubbleIndex++;
                invokeTime = 6f;
            }
            else if (speechBubbleIndex == 6)
            {
                speechBubbleIndex = 5;
                speechBubble.fadeOut = true;
                invokeTime = 60f;
            }
        }
        else if (gm.playerData.CurrentChapter == 4 && gm.playerData.CurrentPhase > 0 && gm.playerData.CurrentPhase < 3)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {

                if (!GameManager.Instance.playerData.AutoTapOwned)
                {
                    speechBubble.txtSpeech.text = "Tower: Height and position?\nPilot: I am 1.8m and I'm sitting.";
                    speechBubbleIndex++;
                    speechBubble.gameObject.SetActive(true);
                    invokeTime = 10f;
                }
                else
                {
                    speechBubbleIndex = 0;
                    speechBubble.fadeOut = true;
                    return;
                }
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 0;
                speechBubble.fadeOut = true;
                return;
            }

            else
            {
                speechBubbleIndex = 0;
                return;
            }

        }
        Invoke("CheckSpeechBubble", invokeTime);
    }

    private void Chapter5()
    {
        float invokeTime = 60f;

        if (gm.selectedChapter < gm.playerData.CurrentChapter)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Boss, Welcome back to Airfield 5";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 1;
                speechBubble.fadeOut = true;
                return;
            }

        }
        else if (gm.playerData.CurrentChapter == 5 && gm.playerData.CurrentPhase == 0)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Let's make something like this!";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubble.fadeOut = true;
                speechBubbleIndex = 5;
                invokeTime = 60f;
            }

            else if (speechBubbleIndex == 5)
            {
                int rnd = Random.Range(0, 100);
                if (rnd < 33)
                    speechBubble.txtSpeech.text = "Midland Domestic airfield!";
                else if (rnd < 66)
                {
                    if (GameManager.Instance.playerData.AutoTapOwned)
                    {
                        speechBubble.txtSpeech.text = "Hold finger down!";
                    }
                    else
                    {
                        speechBubble.txtSpeech.text = "Auto Tap would be handy!";
                    }
                }
                else
                    speechBubble.txtSpeech.text = "In life you are either a passanger or a pilot, it's your choice!";

                speechBubble.gameObject.SetActive(true);
                speechBubbleIndex++;
                invokeTime = 6f;
            }
            else if (speechBubbleIndex == 6)
            {
                speechBubbleIndex = 5;
                speechBubble.fadeOut = true;
                invokeTime = 60f;
            }
        }
        else if (gm.playerData.CurrentChapter == 5 && gm.playerData.CurrentPhase > 0 && gm.playerData.CurrentPhase < 3)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {

                if (!GameManager.Instance.playerData.AutoTapOwned)
                {
                    speechBubble.txtSpeech.text = "In life you are either a passanger or a pilot, it's your choice!";
                    speechBubbleIndex++;
                    speechBubble.gameObject.SetActive(true);
                    invokeTime = 10f;
                }
                else
                {
                    speechBubbleIndex = 0;
                    speechBubble.fadeOut = true;
                    return;
                }
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 0;
                speechBubble.fadeOut = true;
                return;
            }

            else
            {
                speechBubbleIndex = 0;
                return;
            }

        }
        Invoke("CheckSpeechBubble", invokeTime);
    }

    private void Chapter6()
    {
        float invokeTime = 60f;

        if (gm.selectedChapter < gm.playerData.CurrentChapter)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Boss, Welcome back to Airfield 6";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 1;
                speechBubble.fadeOut = true;
                return;
            }

        }
        else if (gm.playerData.CurrentChapter == 6 && gm.playerData.CurrentPhase == 0)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {
                speechBubble.txtSpeech.text = "Let's make something like this!";
                speechBubbleIndex++;
                speechBubble.gameObject.SetActive(true);
                invokeTime = 10f;
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubble.fadeOut = true;
                speechBubbleIndex = 5;
                invokeTime = 60f;
            }

            else if (speechBubbleIndex == 5)
            {
                int rnd = Random.Range(0, 100);
                if (rnd < 33)
                    speechBubble.txtSpeech.text = "Desert Military Airfield!";
                else if (rnd < 66)
                {
                    if (GameManager.Instance.playerData.AutoTapOwned)
                    {
                        speechBubble.txtSpeech.text = "Hold finger down!";
                    }
                    else
                    {
                        speechBubble.txtSpeech.text = "Auto Tap would be handy!";
                    }
                }
                else
                    speechBubble.txtSpeech.text = "How do you know if there is an pilot at your party? He'll tell you.";

                speechBubble.gameObject.SetActive(true);
                speechBubbleIndex++;
                invokeTime = 6f;
            }
            else if (speechBubbleIndex == 6)
            {
                speechBubbleIndex = 5;
                speechBubble.fadeOut = true;
                invokeTime = 60f;
            }
        }
        else if (gm.playerData.CurrentChapter == 6 && gm.playerData.CurrentPhase > 0 && gm.playerData.CurrentPhase < 3)
        {
            if (speechBubbleIndex == 0)
            {
                invokeTime = 5f;
                speechBubbleIndex++;
            }
            else if (speechBubbleIndex == 1)
            {

                if (!GameManager.Instance.playerData.AutoTapOwned)
                {
                    speechBubble.txtSpeech.text = "How do you know if there is an pilot at your party? He'll tell you.";
                    speechBubbleIndex++;
                    speechBubble.gameObject.SetActive(true);
                    invokeTime = 10f;
                }
                else
                {
                    speechBubbleIndex = 0;
                    speechBubble.fadeOut = true;
                    return;
                }
            }
            else if (speechBubbleIndex == 2)
            {
                speechBubbleIndex = 0;
                speechBubble.fadeOut = true;
                return;
            }

            else
            {
                speechBubbleIndex = 0;
                return;
            }

        }
        Invoke("CheckSpeechBubble", invokeTime);
    }

    /*
#if SOFTCEN_DEBUG

    void OnDrawGizmosSelected()
    {
        //Vector3 center = smr.bounds.center;
        //float radius = smr.bounds.. extents.magnitude;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + smrAdjust, smrSize);
        //Gizmos.DrawCube(smr.bounds.center, smr.bounds.size);
        //Gizmos.DrawWireSphere(center, radius);
    }
#endif */
}
