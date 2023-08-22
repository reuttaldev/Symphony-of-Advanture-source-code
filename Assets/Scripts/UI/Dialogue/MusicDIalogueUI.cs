using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MusicDialogueUI : UIInterface
{
    [SerializeField]
    private TMP_Text emotionTxt; // this is the emotion we are asking the player get the character to feel in this specific dialogue
    [SerializeField]
    private InputActionReference confirm; // this input will let us know the player has made their choice 
    public UnityEvent OnPlayerLabeledTrack; // parameters are the chosen track ID and the emotion invoked (the label)
    private void Update()
    {
        if (active && confirm.action.WasPressedThisFrame())
        {
        Debug.Log("Ac");
            PlayerMadeSongSelection();
        }
    }
    void PlayerMadeSongSelection()
    {
        OnPlayerLabeledTrack.Invoke();
        // once player has made their choice, music dialogue is over. close the panel
        MakeInvisible();

    }
}
