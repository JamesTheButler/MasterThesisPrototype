using UnityEngine;
using UnityEngine.UI;

public class OnValueChangedText : MonoBehaviour {
    public string format;

    private Text ValueText;

    private void Awake() {
        ValueText = GetComponent<Text>();
    }

    public void OnSliderValueChanged(float value) {
        ValueText.text = value.ToString(format);
    }
}