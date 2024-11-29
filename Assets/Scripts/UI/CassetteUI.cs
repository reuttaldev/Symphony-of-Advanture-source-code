using Google.Apis.Sheets.v4.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CassetteUI : MonoBehaviour
{
    [SerializeField]
    public Image image;
    [SerializeField]
    float  angle = 0,radAngle,radTargetAngle, targetAngle;
    private RectTransform rectTransform;
    Vector3 centerPoint;
    bool forward;
    public void MoveInCircle(bool f ,float degreesToMove)
    {
        targetAngle = (targetAngle + degreesToMove) % 360;
        radTargetAngle = targetAngle * Mathf.Deg2Rad;
        forward = f;
    }
    private void Update()
    {
        if(angle < targetAngle)
        {   
            if(forward) 
                radAngle += WalkmanUI.rotationSpeed * Time.deltaTime;
            else
                radAngle -= WalkmanUI.rotationSpeed * Time.deltaTime;
            angle = radAngle * Mathf.Rad2Deg;
            SetPosition();
        }
    }
    public void PlaceAroundCircle(float degree)
    {
        angle = degree;
        targetAngle = degree;
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
        //transform.Rotate(radAngle, 0, 0);
        transform.rotation = Quaternion.Euler(angle, 0, 0f);  // Set the rotation
    }


}
