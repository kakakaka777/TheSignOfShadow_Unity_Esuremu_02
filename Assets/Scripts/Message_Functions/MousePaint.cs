using System.Collections;
using System.Collections.Generic;
using Es.TexturePaint;
using UnityEngine;

namespace Es.TexturePaint.Sample
{




    public class MousePaint : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    var paintObject = hitInfo.transform.GetComponent<DynamicPaintObject>();
                    if (paintObject != null)
                        paintObject.Paint(hitInfo);
                }
            }
        }
    }
}
