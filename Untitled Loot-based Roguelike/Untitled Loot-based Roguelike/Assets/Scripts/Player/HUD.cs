using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HUD : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    private RectTransform m_HealthBar;
    [SerializeField]
    private Text text;
    [SerializeField]
    private RectTransform m_StaminaBar;
    #endregion

    #region Private Variables
    private float m_HealthBarOrigWidth;
    private float m_StaminaBarOrigWidth;
    private float currAtkSpd;
    private float timeSinceAtk;
    #endregion

    #region Initialization
    private void Awake()
    {
        m_HealthBarOrigWidth = m_HealthBar.sizeDelta.x;
        m_StaminaBarOrigWidth = m_StaminaBar.sizeDelta.x;
    }
    #endregion

    #region Update Health
    public void UpdateHealth(float percent)
    {
        m_HealthBar.sizeDelta = new Vector2(m_HealthBarOrigWidth * percent, m_HealthBar.sizeDelta.y);
    }

    public void UpdateText(float current, float max)
    {
        text.text = current + "/" + max;
    }
    #endregion

    #region Update Stamina
    public void UpdateAttackSpeed(float atkSpd)
    {
        currAtkSpd = atkSpd;
    }

    public void ResetStamina()
    {
        m_StaminaBar.sizeDelta = new Vector2(0, m_StaminaBar.sizeDelta.y);
        timeSinceAtk = 0;
    }

    private void UpdateStamina()
    {
        timeSinceAtk += Time.deltaTime;
        if (m_StaminaBar.sizeDelta.x < m_StaminaBarOrigWidth)
        {
            float percent = (timeSinceAtk > currAtkSpd) ? 1 : timeSinceAtk / currAtkSpd;
            m_StaminaBar.sizeDelta = new Vector2(m_StaminaBarOrigWidth * percent, m_StaminaBar.sizeDelta.y);
        }
    }
    #endregion

    private void Update()
    {
        UpdateStamina();
    }
}

