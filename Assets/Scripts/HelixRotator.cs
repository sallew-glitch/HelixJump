using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using UnityEngine;

public class HelixRotator : MonoBehaviour
{
    public float rotationSpeed = 300f;
    public float rotationSpeedAndroid = 50f;
    public bool controlEnabled = true;

    private void Update()
    {
#if UNITY_STANDALONE
        //PC controls
        if (Input.GetMouseButton(0) && controlEnabled)
            {
                float mouseX = Input.GetAxisRaw("Mouse X");
                transform.Rotate(transform.position.x, -mouseX * rotationSpeed * Time.deltaTime, transform.position.z);
            }
            

#elif UNITY_ANDROID
            //Android controls
            if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && controlEnabled)
            {
                float xDeltaPos = Input.GetTouch(0).deltaPosition.x;
                transform.Rotate(transform.position.x, -xDeltaPos * rotationSpeedAndroid * Time.deltaTime, transform.position.z);
            }

        print("This is android");
#endif
    }
}
