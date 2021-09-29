using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class IconsAnimator : MonoBehaviour
{

}

[System.Serializable]
public class MoveAbleItems
{
    [SerializeField]
    private RectTransform icon;
    //start_Position will be the icon on screen Position. 
    [SerializeField]
    private Vector3 start_Position;
    //end_Position will be the icon out screen position.
    [SerializeField]
    private Vector3 end_Position;
    //Transition time will be the animation time to move icon f=rom start positon to end position.
    [SerializeField]
    private float transition_Time = 0f;
    //used_Current_Pos true will tell us that the icon will use the position which is set in the scene.
    [SerializeField]
    private bool use_Current_Pos = false;

    [SerializeField]
    private float waitDelayBetweenItems = 0f;

    //MoveIconIn() will move the icons in front of the screen.
    public IEnumerator MoveIconIn()
    {
        Vector3 current_Pos;
        float time_Elapsed = 0f;

        if (use_Current_Pos)
            current_Pos = icon.transform.localPosition;
        else
            current_Pos = start_Position;

        icon.transform.localPosition = current_Pos;

        yield return new WaitForSeconds(waitDelayBetweenItems);

        while (time_Elapsed < transition_Time)
        {
            time_Elapsed += Time.deltaTime / transition_Time;
            icon.transform.localPosition = Vector3.LerpUnclamped(current_Pos, end_Position, MyMath.EaseOutElastic(time_Elapsed / transition_Time));
            yield return new WaitForEndOfFrame();
        }
        icon.transform.localPosition = end_Position;
    }
    //MoveIconOut() will move the icons beyond the screen.
    public IEnumerator MoveIconOut()
    {

        Vector3 current_Pos;
        float time_Elapsed = 0f;

        if (use_Current_Pos)
            current_Pos = icon.transform.localPosition;
        else
            current_Pos = end_Position;

        icon.transform.localPosition = current_Pos;
        yield return new WaitForSeconds(waitDelayBetweenItems);


        while (time_Elapsed < transition_Time)
        {
            time_Elapsed += Time.deltaTime / transition_Time;
            icon.transform.localPosition = Vector3.LerpUnclamped(current_Pos, start_Position, MyMath.EaseOutElastic(time_Elapsed / transition_Time));
            yield return new WaitForEndOfFrame();
        }

    }
}

[System.Serializable]
public class ScalableItems
{
    [SerializeField]
    private RectTransform icon;
    //start_Scale will be the icon on screen Scale.
    [SerializeField]
    private Vector3 start_Scale;
    //end_Scale will be the icon out screen Scale.
    [SerializeField]
    private Vector3 end_Scale;
    //Transition time will be the animation time to move icon from start scale to end scale.
    [SerializeField]
    private float transition_Time = 0f;
    //used_Current_Scale true will tell us that the icon will use the scale which is set in the scene.
    [SerializeField]
    private bool use_Current_Scale = false;

    //MoveScaleUp() will scale up the icons in front of the screen.
    public IEnumerator MoveScaleUp()
    {
        Vector3 current_Scale;
        float time_Elapsed = 0f;

        if (use_Current_Scale)
            current_Scale = icon.transform.localScale;
        else
            current_Scale = start_Scale;

        icon.transform.localScale = current_Scale;
        while (time_Elapsed < transition_Time )
        {
            time_Elapsed += Time.deltaTime / transition_Time ;
            icon.transform.localScale = Vector3.LerpUnclamped(current_Scale, end_Scale, MyMath.EaseOutElastic(time_Elapsed/ transition_Time)) ;
            yield return new WaitForEndOfFrame();
        }

    }
    //MoveScaleDown() will scale down the icons;
    public IEnumerator MoveScaleDown()
    {
        Vector3 current_Scale;
        float time_Elapsed = 0f;

        if (use_Current_Scale)
            current_Scale = icon.transform.localScale;
        else
            current_Scale = end_Scale;

        icon.transform.localScale = current_Scale;
        while (time_Elapsed < transition_Time)
        {
            time_Elapsed += Time.deltaTime / transition_Time;
            icon.transform.localScale = Vector3.LerpUnclamped(current_Scale, start_Scale, MyMath.EaseOutElastic(time_Elapsed / transition_Time));
            yield return new WaitForEndOfFrame();
        }

    }

}

[System.Serializable]
public class ContinueElements
{
    [SerializeField]
    private RectTransform icon;
    public float animation_Time = 0f;
    [HideInInspector] public bool is_Loop_Break = false;
    private float time_Elapsed = 0f;

    public IEnumerator ContinueAnimation()
    {
        float fill_Amount = 0f;
        float current_Value = 1f;
        time_Elapsed = 0f;
        icon.gameObject.GetComponent<Image>().fillAmount = current_Value;

        while (time_Elapsed <= animation_Time && !is_Loop_Break)
        {
            if (is_Loop_Break)
                break;
            time_Elapsed += Time.deltaTime / animation_Time;
            icon.gameObject.GetComponent<Image>().fillAmount = Mathf.Lerp(current_Value, fill_Amount, time_Elapsed);
            Debug.Log("Here");
            yield return null;
        }

    }

}

[System.Serializable]
public class ContinuousScaleUpAndDown
{
    [SerializeField]
    private RectTransform icon;
    //start_Scale will be the icon on screen Scale.
    [SerializeField]
    private Vector3 start_Scale;
    //end_Scale will be the icon out screen Scale.
    [SerializeField]
    private Vector3 end_Scale;
    //Transition time will be the animation time to move icon from start scale to end scale.
    [SerializeField]
    private float transition_Time = 0f;
    //used_Current_Scale true will tell us that the icon will use the scale which is set in the scene.
    [SerializeField]
    private bool use_Current_Scale = false;
    [HideInInspector]
    public bool isAnimate = false;
    
    public IEnumerator ScaleMovementUpAndDown()
    {
        Vector3 current_Scale;
        yield return new WaitForSeconds(0.2f);
        float time_Elapsed = 0f;
        isAnimate = true;
        if (use_Current_Scale)
            current_Scale = icon.transform.localScale;
        else
            current_Scale = end_Scale;
      
        while (isAnimate)
        {

            while (time_Elapsed <= transition_Time)
            {
                time_Elapsed += Time.deltaTime / transition_Time;
                icon.transform.localScale = Vector3.Lerp(current_Scale, end_Scale, time_Elapsed / transition_Time);
                if (!isAnimate)
                    yield break;
                yield return new WaitForEndOfFrame();
            }
            time_Elapsed = 0f;
            current_Scale = icon.transform.localScale;

            while (time_Elapsed <= transition_Time)
            {
                time_Elapsed += Time.deltaTime / transition_Time;
                icon.transform.localScale = Vector3.Lerp(current_Scale, start_Scale, time_Elapsed / transition_Time);
                if (!isAnimate)
                    yield break;
                yield return new WaitForEndOfFrame();

            }

            if (!isAnimate)
                yield break;
            current_Scale = icon.transform.localScale;
            time_Elapsed = 0f;

        }
      
    }

}

[System.Serializable]
public class FadeAbleItems
{
    [SerializeField]
    private RectTransform icon;
    [SerializeField]
    private float transition_Time = 0f;
    private float time_Elapsed = 0f;
    private Color icon_Color;
    public float end_Color_Alpha = 1f;
    private float start_Color_Alpha = 0f;

    public IEnumerator IconFadeIn()
    {
        icon.gameObject.SetActive(true);
        time_Elapsed = 0f;
        icon_Color = icon.GetComponent<Image>().color;
        float current_Fade = icon_Color.a;
        icon_Color.a = start_Color_Alpha;
        icon.GetComponent<Image>().color = icon_Color;

        while (time_Elapsed <= transition_Time)
        {
            icon_Color.a = Mathf.Lerp(current_Fade, end_Color_Alpha, time_Elapsed / transition_Time);
            icon.GetComponent<Image>().color = icon_Color;
            time_Elapsed += Time.deltaTime / transition_Time;
            yield return new WaitForEndOfFrame();
        }
        icon_Color.a = end_Color_Alpha;
        icon.GetComponent<Image>().color = icon_Color;
    }

    public IEnumerator IconFadeOut()
    {
        time_Elapsed = 0f;
        icon_Color = icon.GetComponent<Image>().color;
        icon_Color.a = end_Color_Alpha;
        float current_Fade = icon_Color.a;
        icon.GetComponent<Image>().color = icon_Color;

        while (time_Elapsed <= transition_Time)
        {
            icon_Color.a = Mathf.Lerp(current_Fade, start_Color_Alpha, time_Elapsed / transition_Time);
            icon.GetComponent<Image>().color = icon_Color;
            time_Elapsed += Time.deltaTime / transition_Time;
            yield return new WaitForEndOfFrame();
        }
        icon_Color.a = 0f;
        icon.GetComponent<Image>().color = icon_Color;
        //yield return new WaitForSeconds(0.5f);
        icon.gameObject.SetActive(false);


    }
}

[System.Serializable]
public class CountDown
{
    [SerializeField]
    private TextMeshProUGUI countDownText;

    [SerializeField]
    private float cound_Down_Wait = 0f;
   
    private float time_Elapsed = 0f;
    public int countDown_StartTime = 3;
    [HideInInspector]public int count_Down_Current_Value = 3;

    [HideInInspector] public bool is_Start = false;
    [HideInInspector] public bool is_Complete = false;

    public IEnumerator StartCountDownTimer()
    {
        countDownText.text = countDown_StartTime.ToString();
        countDownText.gameObject.SetActive(true);
        is_Complete = false;
        is_Start = true;
        yield return new WaitForSeconds(0.3f);
        count_Down_Current_Value = countDown_StartTime;
        countDown_StartTime = count_Down_Current_Value;
        while (count_Down_Current_Value >= 0)
        {
            
            while (time_Elapsed <=  cound_Down_Wait)
            {
                time_Elapsed += Time.deltaTime;
                
                yield return null;
            }
            count_Down_Current_Value--;
            if (count_Down_Current_Value == 0)
            {
                countDownText.text = "GO!!";
            }
            else if(count_Down_Current_Value > 0)
            {

                countDownText.text = count_Down_Current_Value.ToString();
            }
            

            time_Elapsed = 0f;
        }
        
        is_Complete = true;
        is_Start = false;
        countDownText.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class IncrementAnimation
{
    //Text on Screen to increment
    [SerializeField]
    private TextMeshProUGUI totalCoins;

    //Total Animation time
    [SerializeField]
    private float animationTime;
    [HideInInspector] public bool isAnimationCompleted = false;


    public IEnumerator IncrementCoins(int AnimationRewardedCoins)
    {
        isAnimationCompleted = false;
        float timeElapsed = 0f;
        float currentCoins = Convert.ToInt32(totalCoins.text);
        float EndNumberOfCoins = currentCoins + AnimationRewardedCoins;
        while (timeElapsed <= animationTime)
        {
            currentCoins = Mathf.Lerp(currentCoins, EndNumberOfCoins, timeElapsed / animationTime);
            totalCoins.text = Convert.ToInt32(currentCoins).ToString();
            timeElapsed += Time.deltaTime / animationTime;
            yield return new WaitForSeconds(0.05f);
            //yield return new WaitForEndOfFrame();

        }
        totalCoins.text = EndNumberOfCoins.ToString();
        isAnimationCompleted = true;

    }

   

}
