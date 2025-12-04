using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementPopup : MonoBehaviour {
    public TextMeshProUGUI text;
    public RectTransform popupPanel;
    private Canvas canvas;

    public static AchievementPopup instance;

    private Queue<string> pendingAchievements = new Queue<string>();
    private bool showingPopup = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        instance = this;
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update() {
    }

    public void ShowAchievement(string msg) {
        pendingAchievements.Enqueue(msg);
        if (!showingPopup) {
            StartCoroutine(ShowAnimation());
        }
    }

    private static float easeInOutQuad(float x) {
        return x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
    }

    IEnumerator ShowAnimation() {
        while (pendingAchievements.Count > 0) {
            var msg = pendingAchievements.Dequeue();
            var values = text.GetPreferredValues(msg);
            popupPanel.anchoredPosition = new Vector2(values.x + 64, popupPanel.anchoredPosition.y);
            popupPanel.sizeDelta = new Vector2(values.x + 32, popupPanel.sizeDelta.y);
            text.SetText(msg);

            var w = values.x + 16;

            canvas.enabled = true;
            showingPopup = true;
            {
                float elapsed = 0f;
                while (elapsed < 1f) {
                    float t = Mathf.Min(elapsed, 1f);
                    float eased = easeInOutQuad(t);
                    popupPanel.anchoredPosition =
                        new Vector2((w + 48f + 32f) * -eased + (w + 48), popupPanel.anchoredPosition.y);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            yield return new WaitForSeconds(2.5f);

            {
                float elapsed = 0f;
                while (elapsed < 1f) {
                    float t = Mathf.Min(elapsed, 1f);
                    float eased = easeInOutQuad(t);
                    popupPanel.anchoredPosition =
                        new Vector2((w + 48f + 32f) * (eased - 1) + (w + 48), popupPanel.anchoredPosition.y);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }


            canvas.enabled = false;
            showingPopup = false;
            
            yield return new WaitForSeconds(0.5f);
        }
    }
}