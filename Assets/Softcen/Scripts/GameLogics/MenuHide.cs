using UnityEngine;

public class MenuHide : MonoBehaviour {
    public bool stateWhenHide;
    public bool changeInteractable = false;
    public CanvasGroup canvasGroup;

    private bool m_blockRayCast;
    public void HideItem(bool state)
    {
        if (state == true)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                if (changeInteractable == true)
                {
                    canvasGroup.interactable = false;
                    m_blockRayCast = canvasGroup.blocksRaycasts;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                if (changeInteractable == true)
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = m_blockRayCast;
                }
            }
            else
            {
                if (stateWhenHide)
                    gameObject.SetActive(true);
            }

        }
    }
}
