using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class FullScreenEffectController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float fadeTime;
    [SerializeField] private bool activeEffect;
    
    
    [Header("Wifi Event Settings")] 
    [SerializeField] private ScriptableRendererFeature fullScreenWifiEffect;
    [SerializeField] private Material wifiMaterial;

    private int WifiIntensity = Shader.PropertyToID("_Vignette_Intensity");
    private int WifiSpeed = Shader.PropertyToID("_Effect_Speed");
     
    [SerializeField] private float vignetteWifiIntensityStart = 0.7f;
    [SerializeField] private Vector2 effectWifiSpeedStart = new Vector2(15,0.1f) ;
    private float currentWifiVignetteIntensity;
    private Vector2 currentWifiEffectSpeed;
    
    [Header("Gravity Event Settings")]
    [SerializeField] private ScriptableRendererFeature fullScreenGravityEffect;
    [SerializeField] private Material gravityMaterial;
    
    private int gravityIntensity = Shader.PropertyToID("_Full_screen_effect_Intensity");
    private int gravitySpeed = Shader.PropertyToID("_Effect_Speed");
    
    [SerializeField] private float gravityVignetteIntensityStat = 0.1f;
    [SerializeField] private float gravitySpeedEffectStat = 1f;
    private float currentGravitySpeed;
    private float currentGravityIntensity;
    
    [Header("Radio Event Settings")]
    [SerializeField] private ScriptableRendererFeature fullScreenRadioEffect;
    [SerializeField] private Material radioMaterial;

    private int radioIntensity = Shader.PropertyToID("_Vignette_Intensity");
    private int radioSpeed = Shader.PropertyToID("_Effect_Speed");

    [SerializeField] private float radioVignetteIntesityStat = 0.7f;
    [SerializeField] private Vector2 radioSpeedStat = new Vector2(-0.1f, 0f);
    private Vector2 currentRadioSpeed;
    private float currentRaidoIntensity;
    private void Start()
    {

        DisableFullScreenEffects();

       //wifi effect 
        currentWifiVignetteIntensity = activeEffect ? 0f : vignetteWifiIntensityStart;
        currentWifiEffectSpeed = activeEffect ? Vector2.zero : effectWifiSpeedStart;
        wifiMaterial.SetFloat(WifiIntensity, currentWifiVignetteIntensity);
        wifiMaterial.SetVector(WifiSpeed, currentWifiEffectSpeed);
        
        //gravity effect
        currentGravityIntensity = activeEffect ? 0f : gravityVignetteIntensityStat;
        currentGravitySpeed = activeEffect ? 0f : gravitySpeedEffectStat;
        gravityMaterial.SetFloat(gravityIntensity, currentGravityIntensity);
        gravityMaterial.SetFloat(gravitySpeed, currentGravitySpeed);
        
        //Radio Effect
        currentRaidoIntensity = activeEffect ? 0f : radioVignetteIntesityStat;
        currentRadioSpeed = activeEffect ? Vector2.zero : radioSpeedStat;
        radioMaterial.SetFloat(radioIntensity, currentRaidoIntensity);
        radioMaterial.SetVector(radioSpeed,currentRadioSpeed);
    }

    private void OnDestroy()
    {
        DisableFullScreenEffects();
    }

    private void DisableFullScreenEffects()
    {
        fullScreenGravityEffect.SetActive(false);
        fullScreenRadioEffect.SetActive(false);
        fullScreenWifiEffect.SetActive(false);
    }
    // private void Update()// for testing please remove when done
    // {
    //     if (Keyboard.current.hKey.wasPressedThisFrame)
    //     {
    //         ToggleGravityEffect(activeEffect);
    //     }
    //
    //     if (Keyboard.current.jKey.wasPressedThisFrame)
    //     {
    //         ToggleWifiEffect(activeEffect);
    //     }
    //
    //     if (Keyboard.current.kKey.wasPressedThisFrame)
    //     {
    //         ToggleRadioEffect(activeEffect);
    //     }
    // }

    #region Wifi Effect
    public void ToggleWifiEffect(bool activate)
    {
        activeEffect = activate;
        StopAllCoroutines();
        if (activeEffect == false)
        {
            activeEffect = true;
            StartCoroutine(ActiveWifiEffect(true, fadeTime));
        }
        else
        {
            activeEffect = false;
            StartCoroutine(DeactiveWifiEffect(fadeTime));
        }
    }
    private IEnumerator ActiveWifiEffect(bool fadeIn, float fadeTime)
    {
        float timer = 0f;
        float startIntensity = fadeIn ? 0f : currentWifiVignetteIntensity; 
        float endIntensity = fadeIn ? currentWifiVignetteIntensity : 0f;
        fullScreenWifiEffect.SetActive(true);
        
        while (timer < fadeTime)
        { timer += Time.deltaTime;
            currentWifiVignetteIntensity = Mathf.Lerp(startIntensity, endIntensity, timer / fadeTime);
            wifiMaterial.SetFloat(WifiIntensity, currentWifiVignetteIntensity);
            activeEffect = true;
            yield return null;
        }
    }
    private IEnumerator DeactiveWifiEffect(float fadeTime)
    {
        float timer = 0f;
        float startValue = wifiMaterial.GetFloat(WifiIntensity);
        float endValue = 0f;
        
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            wifiMaterial.SetFloat(WifiIntensity, Mathf.Lerp(startValue, endValue,timer / fadeTime ));
            yield return null;
        }
        ResetWifiValue();
        fullScreenWifiEffect.SetActive(false);
    }
    private void ResetWifiValue()
    {
        activeEffect = false;
        currentWifiVignetteIntensity = activeEffect ? 0f : vignetteWifiIntensityStart;
        currentWifiEffectSpeed = activeEffect ? Vector2.zero : effectWifiSpeedStart;
        wifiMaterial.SetFloat(WifiIntensity, currentWifiVignetteIntensity);
        wifiMaterial.SetVector(WifiSpeed, currentWifiEffectSpeed);
    }
    #endregion

    #region Gravity Effect
    public void ToggleGravityEffect(bool activate)
    {
        activeEffect = activate;
        StopAllCoroutines();
        if (activeEffect == true)
        {
            activeEffect = true;
            StartCoroutine(ActiveGravityEffect(true,fadeTime));
        }
        else
        {
            activeEffect = false;
            StartCoroutine(DeactiveGravtyEffect(fadeTime));
        }
    }

    private IEnumerator ActiveGravityEffect(bool fadein, float fadeTime)
    {
        float startIntensity = fadein ? 0f : gravityVignetteIntensityStat;
        float endIntensity = fadein ? gravityVignetteIntensityStat : 0f; 
        float timer = 0f;
        fullScreenGravityEffect.SetActive(true);
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            currentGravityIntensity = Mathf.Lerp(startIntensity, endIntensity, timer / fadeTime);
            gravityMaterial.SetFloat(gravityIntensity, currentGravityIntensity);
            activeEffect = true;
            yield return null;
        }
    }

    private IEnumerator DeactiveGravtyEffect(float fadetime)
    {
        float startIntensity = gravityMaterial.GetFloat(gravityIntensity);
        float endIntensity = 0f;
        float timer = 0f;
        while (timer < fadetime)
        {
            timer += Time.deltaTime;
            gravityMaterial.SetFloat(gravityIntensity, Mathf.Lerp(startIntensity, endIntensity, timer / fadetime));
            yield return null;
        }
        ResetGravityEffectValue();
        fullScreenGravityEffect.SetActive(false);
    }
    private void ResetGravityEffectValue()
    {
        activeEffect = false;
        currentGravityIntensity = activeEffect ? 0f : gravityVignetteIntensityStat;
        currentGravitySpeed = activeEffect ? 0f : gravitySpeedEffectStat;
        gravityMaterial.SetFloat(gravityIntensity, currentGravityIntensity);
        gravityMaterial.SetFloat(gravitySpeed, currentGravitySpeed);
    }
    #endregion

    #region Radio Effect
    public void ToggleRadioEffect(bool b)
    {
        //activeEffect = b;
        if (activeEffect == false)
        {
            activeEffect = true;
            StartCoroutine(ActiveRadioEffect(true, fadeTime));
        }
        else
        {
            activeEffect = false;
            StartCoroutine(DeactiveRadioEffect(fadeTime));
        }
    }
    private IEnumerator ActiveRadioEffect(bool fadin, float fadeTime)
    {
        float timer = 0f;
        float startIntensity = fadin ? 0f : radioVignetteIntesityStat;
        float endIntensity = fadin ? radioVignetteIntesityStat : 0f;
        fullScreenRadioEffect.SetActive(true);
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            currentRaidoIntensity = Mathf.Lerp(startIntensity, endIntensity, timer / fadeTime);
            radioMaterial.SetFloat(radioIntensity, currentRaidoIntensity);
            activeEffect = true;
            yield return null;
        }
    }
    private IEnumerator DeactiveRadioEffect(float fadeTime)
    {
        float startValue = radioMaterial.GetFloat(radioIntensity);
        float endValue = 0f;
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            radioMaterial.SetFloat(radioIntensity, Mathf.Lerp(startValue, endValue,timer / fadeTime));
            yield return null;
        }
        ResetRadioEffect();
        fullScreenRadioEffect.SetActive(false);
    }
    private void ResetRadioEffect()
    {
        activeEffect = false;
        currentRaidoIntensity = activeEffect ? 0f : radioVignetteIntesityStat;
        currentRadioSpeed = activeEffect ? Vector2.zero : radioSpeedStat;
        radioMaterial.SetFloat(radioIntensity, currentRaidoIntensity);
        radioMaterial.SetVector(radioSpeed,currentRadioSpeed);
    }
    #endregion
}
