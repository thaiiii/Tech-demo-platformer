using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    public static PlayerEffects Instance;

    public float speedRate = 1f;
    public float speedEffectDuration = 0f;
    public float sizeUpValue = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ApplyEffect(string effectName)
    {
        switch (effectName)
        {
            case "SpeedBoost":
                StartCoroutine(SpeedBoostEffect(speedRate, speedEffectDuration));
                break;
            case "SizeBoost":
                SizeBoostEffect(sizeUpValue);
                break;
            default:
                Debug.Log($"Khong co hieu ung");
                break;
        }
    }
    public IEnumerator SpeedBoostEffect(float speedRate, float speedEffectDuration)
    {
        FindAnyObjectByType<Player>().speed *= speedRate;
        yield return new WaitForSeconds(speedEffectDuration);
        FindAnyObjectByType<Player>().speed /= speedRate;
    }
    public void SizeBoostEffect(float sizeUpValue)
    {
        StartCoroutine(SmoothSizeChange(FindFirstObjectByType<Player>(), sizeUpValue, 1f));
        StartCoroutine(SmoothZoom(sizeUpValue, 1.5f));
    }
    private IEnumerator SmoothSizeChange(Player player, float sizeUpValue, float duration)
    {
        Vector3 playerScale = FindFirstObjectByType<Player>().transform.localScale;
        Vector3 targetScale;
        //Thay doi kich thuoc tu tu

        targetScale = new Vector3(
            playerScale.x * sizeUpValue,
            playerScale.y * sizeUpValue,
            playerScale.z * sizeUpValue);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            FindFirstObjectByType<Player>().transform.localScale = Vector3.Lerp(playerScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Đợi frame tiếp theo
        }
        FindFirstObjectByType<Player>().transform.localScale = targetScale;

        //Thay doi radius check wall
        FindFirstObjectByType<Player>().checkWallRadius *= FindFirstObjectByType<Player>().transform.localScale.y;
    }
    private IEnumerator SmoothZoom(float targetSize, float duration)
    {
        float startSize = Camera.main.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(startSize, startSize * sizeUpValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Camera.main.orthographicSize = startSize * sizeUpValue;
    }
   

}
