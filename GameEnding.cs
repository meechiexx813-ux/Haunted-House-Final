using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameEnding : MonoBehaviour
{
    [Header("Timing")]
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;

    [Header("References")]
    public GameObject player;
    public UIDocument uiDocument;

    [Header("Audio")]
    public AudioSource exitAudio;
    public AudioSource caughtAudio;

    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;
    bool m_HasAudioPlayed;
    float m_Timer;

    VisualElement m_EndScreen;
    VisualElement m_CaughtScreen;

    void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("GameEnding: uiDocument is not assigned.");
            enabled = false;
            return;
        }

        var root = uiDocument.rootVisualElement;
        m_EndScreen = root.Q<VisualElement>("EndScreen");
        m_CaughtScreen = root.Q<VisualElement>("CaughtScreen");

        if (m_EndScreen == null)
            Debug.LogError("GameEnding: EndScreen element not found in UI.");
        else
            m_EndScreen.style.opacity = 0f;

        if (m_CaughtScreen == null)
            Debug.LogError("GameEnding: CaughtScreen element not found in UI.");
        else
            m_CaughtScreen.style.opacity = 0f;

        m_IsPlayerAtExit = false;
        m_IsPlayerCaught = false;
        m_HasAudioPlayed = false;
        m_Timer = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("GameEnding trigger hit by: " + other.name);

        if (other.gameObject != player && !other.CompareTag("Player"))
            return;

        Debug.Log("GameEnding: PLAYER REACHED EXIT");
        m_IsPlayerAtExit = true;
        m_IsPlayerCaught = false;
        m_Timer = 0f;
        m_HasAudioPlayed = false;
    }

    public void CaughtPlayer()
    {
        if (m_IsPlayerCaught)
            return;

        Debug.Log("GameEnding: PLAYER CAUGHT");
        m_IsPlayerCaught = true;
        m_IsPlayerAtExit = false;
        m_Timer = 0f;
        m_HasAudioPlayed = false;
    }

    void Update()
    {
        if (m_IsPlayerAtExit)
        {
            EndLevel(m_EndScreen, false, exitAudio);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel(m_CaughtScreen, true, caughtAudio);
        }
    }

    void EndLevel(VisualElement element, bool doRestart, AudioSource audioSource)
    {
        if (element == null)
            return;

        if (!m_HasAudioPlayed)
        {
            if (audioSource != null)
                audioSource.Play();
            m_HasAudioPlayed = true;
        }

        m_Timer += Time.deltaTime;

        float alpha = Mathf.Clamp01(m_Timer / fadeDuration);
        element.style.opacity = alpha;

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            if (doRestart)
            {
                Debug.Log("GameEnding: Reloading scene Main");
                SceneManager.LoadScene("Main");
            }
            else
            {
                Debug.Log("GameEnding: Quitting game");
                Application.Quit();
#if UNITY_EDITOR
                Time.timeScale = 0f;
#endif
            }
        }
    }
}
