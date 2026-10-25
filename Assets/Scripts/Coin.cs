using System.Collections;
using UnityEngine;

// Score item: shows "+points", plays a collect animation and fades out after being picked up
public class Coin : Interactable
{
    public int points = 50;
    public float collectDuration = 0.4f;   // length of the collect animation and fade
    public GameObject collectEffect;       // optional particle effect
    public GameObject floatingTextPrefab;  // the "+50" text prefab

    bool collected;

    protected override void Interact(PlayerController player)
    {
        if (collected) return;   // prevents giving points twice
        collected = true;

        GameManager.Instance.AddScore(points);
        AudioManager.Instance.PlayCoin();
        GetComponent<Collider2D>().enabled = false;

        // Spawn the floating "+50" text just above the coin
        if (floatingTextPrefab != null)
        {
            GameObject obj = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            obj.GetComponent<FloatingText>().SetText("+" + points);
        }

        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // Play the collect animation (needs a "Collect" trigger in the coin's Animator)
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("Collect");

        StartCoroutine(FadeAndDestroy());
    }

    // Fade the sprite out, then remove the coin
    IEnumerator FadeAndDestroy()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Color start = sr.color;
        float t = 0f;

        while (t < collectDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - t / collectDuration;
            sr.color = new Color(start.r, start.g, start.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}