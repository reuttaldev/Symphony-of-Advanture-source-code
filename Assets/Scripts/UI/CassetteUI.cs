using Google.Apis.Sheets.v4.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CassetteUI : MonoBehaviour
{
    [SerializeField]
    public Image image;
    [SerializeField]
    float  angle = 0,radAngle;
    private RectTransform rectTransform;
    Vector3 centerPoint;
    public void MoveInCircle(bool forward ,float duration)
    {
        StartCoroutine(MoveInCircle(duration));
    }
    public void PlaceAroundCircle(float degree)
    {
        angle = degree;
        radAngle = degree *Mathf.Deg2Rad ;
        rectTransform = GetComponent<RectTransform>();
        // world position of the rectTransform
        centerPoint = rectTransform.TransformPoint(rectTransform.anchoredPosition);
        SetPosition();
    }
    private void SetPosition()
    {
        transform.position = new Vector3(
            centerPoint.x,                             
            centerPoint.y + Mathf.Cos(radAngle) * WalkmanUI.radius,      
            centerPoint.z + Mathf.Sin   (radAngle) * WalkmanUI.radius         
        );
        transform.Rotate(WalkmanUI.radius * angle, 0, 0);
    }

    public IEnumerator MoveInCircle(float degreesToMove)
    {
        float targetAngle =angle + degreesToMove;
        float radTargetAngle = targetAngle * Mathf.Deg2Rad;
        while (radAngle < radTargetAngle)
        {
            radAngle += WalkmanUI.rotationSpeed *Time.deltaTime;
            // avoid overshooting
            //angle = Mathf.Min(angle, targetAngle);
            SetPosition();
            yield return null;
        }
        // make sure final position is exact
        SetPosition();
    }

}
