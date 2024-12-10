using Google.Apis.Sheets.v4.Data;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CassetteUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text[] trackNameTexts;
    [SerializeField]
    Image[] images;
    [SerializeField]
    float radAngle, radTargetAngle;
    Vector3 centerPoint;
    bool forward;
    const float RotationThreshold = 0.1f;
    /*public void MoveInCircle(bool f, float degreesToMove)
    {
        forward = f;
        if (forward)
            radTargetAngle += degreesToMove * Mathf.Deg2Rad;
        else
            radTargetAngle -= degreesToMove * Mathf.Deg2Rad;
        Debug.Log(Mathf.Abs(radTargetAngle - radAngle));
        StopAllCoroutines();    
        StartCoroutine(MoveInCircleCoroutine());
    }
        public void PlaceAroundCircle(float degree)
    {
        radAngle = degree *Mathf.Deg2Rad ;
        radTargetAngle = radAngle;
        rectTransform = GetComponent<RectTransform>();
        // world position of the rectTransform
        centerPoint = rectTransform.TransformPoint(rectTransform.anchoredPosition);
        SetPosition();
    }*/

    public void SetImage(Sprite sprite)
    {
        foreach (var item in images)
        {
            item.sprite = sprite;
        }
    }
    public void SetText(string t)
    {
        foreach (var item in trackNameTexts)
        {
            item.text = t;
        }
    }
    public void MoveToFrontWithAnim(bool f)
    {
        forward = f;
        StopAllCoroutines();
        radTargetAngle = WalkmanUI.radFrontRotation;
        StartCoroutine(MoveInCircleCoroutine(WalkmanUI.frontRotationSpeed));
    }

    public void MoveToBackWithAnim(bool f)
    {
        forward = f;
        StopAllCoroutines();
        radTargetAngle = WalkmanUI.radBackRotation;
        StartCoroutine(MoveInCircleCoroutine(WalkmanUI.backRotationSpeed));
    }

    public void PlaceAtFrontTarget()
    {
        radAngle = WalkmanUI.radFrontRotation;
        SetPosition();
    }
    public void PlaceAtBackTarget()
    {
        radAngle = WalkmanUI.radBackRotation;
        SetPosition();
    }
    private IEnumerator MoveInCircleCoroutine(float speed)
    {
        while (Mathf.Abs(radTargetAngle - radAngle) > RotationThreshold)
        {
            float step = speed * Time.deltaTime;
            radAngle += forward ? step : -step;
            radAngle = WalkmanUI.NormalizeAngle(radAngle);
            SetPosition();
            yield return new WaitForEndOfFrame();
        }
    }

    private void SetPosition()
    {
        GetComponent<RectTransform>().anchoredPosition3D =  new Vector3(0, 
            Mathf.Cos(radAngle) * WalkmanUI.radius,
            Mathf.Sin(radAngle) * WalkmanUI.radius  
        );
      
        transform.rotation = Quaternion.Euler(radAngle* Mathf.Rad2Deg, 0, 0);  // Set the rotation
    }

}
