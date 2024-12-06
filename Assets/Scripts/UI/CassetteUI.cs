using Google.Apis.Sheets.v4.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CassetteUI : MonoBehaviour
{
    [SerializeField]
    public Text trackNameText;
    public Image image;
    [SerializeField]
    float radAngle, radTargetAngle;
    Vector3 centerPoint;
    bool forward;
    const float TwoPi = Mathf.PI * 2;
    private const float RotationThreshold = 0.1f;
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

    public void MoveToFront(bool f)
    {
        Debug.Log(name +  "is moving forwards");
        forward = f;
        StopAllCoroutines();
        radTargetAngle = NormalizeAngle(WalkmanUI.frontRotation * Mathf.Deg2Rad);
        StartCoroutine(MoveInCircleCoroutine(WalkmanUI.frontRotationSpeed));
    }

    public void MoveToBack(bool f)
    {
        forward = f;
        StopAllCoroutines();
        radTargetAngle = NormalizeAngle(WalkmanUI.backRotation * Mathf.Deg2Rad);
        StartCoroutine(MoveInCircleCoroutine(WalkmanUI.backRotationSpeed));
    }
    private IEnumerator MoveInCircleCoroutine(float speed)
    {
        while (Mathf.Abs(radTargetAngle - radAngle) > RotationThreshold)
        {
            float step = speed * Time.deltaTime;
            radAngle += forward ? step : -step;
            radAngle = NormalizeAngle(radAngle);
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
    private float NormalizeAngle(float angle)
    {
        while (angle < 0) angle += TwoPi;
        while (angle >= TwoPi) angle -= TwoPi;
        return angle;
    }
}
