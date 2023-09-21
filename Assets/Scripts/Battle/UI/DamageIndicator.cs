using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DamageIndicator : MonoBehaviour
{
    public TextMeshPro text;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private Color damageColor, cureColor, missColor, criticColor;

    private Vector3 initialPosition;



    private void Awake()
    {
        gameObject.SetActive(false);
    }
    private void Start()
    {
        initialPosition = getInitialPosition();
    }

    private Vector3 getInitialPosition()
    {
        Vector3 transformParent = transform.parent.position;
        return transformParent - transform.localPosition;
    }

    public void ShowDamage(ResultDamage value)
    {
//        this.transform.position = initialPosition;
        //        Debug.Log(value.debugToString());
        setColorByDamageType(value);
        setText(value);
        this.gameObject.SetActive(true);
    }

    public void ShowDamage(float damage)
    {
//        this.transform.position = initialPosition;
        setColorByDamageType(damage);
        setText(damage);
        gameObject.SetActive(true);
    }

    public void ShowDamage(object value)
    {
//        this.transform.position = initialPosition;
        setColorByDamageType(value);
        this.gameObject.SetActive(true);
        setText((int)value);
    }

    private void setText(ResultDamage value)
    {
        float damageAbs = MathF.Abs(value.Result); //Convert to absolute value for negative damage (heal) 
        if (damageAbs > 0 && damageAbs < 1)
            this.text.text = 1 + "";
        else this.text.text = value.Miss ? "MISS" : Math.Abs(damageAbs).ToString();
    }

    private void setText(float damage)
    {
        float damageAbs = MathF.Abs(damage); //Convert to absolute value for negative damage (heal) 
        this.text.text = (int)damageAbs + "";
        if (damageAbs > 0 && damageAbs < 1)
            this.text.text = 1 + "";
    }

    private void setText(int damage)
    {
        float damageAbs = MathF.Abs(damage); //Convert to absolute value for negative damage (heal) 
        this.text.text = (int)damageAbs + "";
        if (damageAbs > 0 && damageAbs < 1)
            this.text.text = 1 + "";
    }

    public void HideDamage()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeOutAndMoveUp());
    }

    private IEnumerator FadeOutAndMoveUp()
    {
        Color startColor = text.color;
        Vector3 startPosition = transform.position;

        Color tempColor = text.color;
        Vector3 tempPosition = transform.position;

        while (text.color.a > 0f)
        {
            float fadeAmount = fadeSpeed * Time.deltaTime;
            tempColor.a -= fadeAmount;
            text.color = tempColor;

            tempPosition.y += yOffset * Time.deltaTime;
            transform.position = tempPosition;

            yield return null;
        }
        gameObject.SetActive(false);
        transform.position = startPosition;
        text.color = startColor;
    }

    private void setColorByDamageType(ResultDamage value)
    {
        int damageInt = value.Result;
        if (damageInt < 0)
        {
            text.color = cureColor;
        }
        else
            text.color = value.Critic ? criticColor : damageColor;
    }

    private void setColorByDamageType(object value)
    {
        if (value is int)
        {
            int damageInt = (int)value;
            text.color = damageInt < 0 ? cureColor : damageColor;
        }
        else
            text.color = missColor;
    }
}
