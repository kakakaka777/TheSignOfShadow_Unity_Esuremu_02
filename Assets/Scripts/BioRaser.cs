using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioRaser : MonoBehaviour
{
    [SerializeField] float parentMoveSpeedX = 0.5f;

    [SerializeField] GameObject objectA;
    [SerializeField] float detectionDistance = 5.0f;
    [SerializeField] GameObject[] childrenToMoveZ;
    [SerializeField] GameObject[] childrenToMoveY;

    [SerializeField] float childMoveSpeedZ = 0.3f;
    [SerializeField] float childMoveSpeedY = 0.3f;

    [SerializeField] private float targetChildZPosition = -10.0f;
    [SerializeField] private float targetChildYPosition = 4;

    private bool isChildMovingZ = false;

    private void Update()
    {
        transform.Translate(Vector3.left * parentMoveSpeedX * Time.deltaTime);
 
        
        if (objectA != null)
        {
            float distance = Vector3.Distance(transform.position, objectA.transform.position);
            //Debug.Log("プレイヤーまでの距離: "+ distance);

            if (distance < detectionDistance)
            {
                isChildMovingZ = true;
            }
        }

        
        if (isChildMovingZ)
        {
            foreach (GameObject child in childrenToMoveZ)
            {
                if (child != null)
                {
                    
                    if (child.transform.position.z > targetChildZPosition)
                    {
                        
                        child.transform.Translate(Vector3.forward * -childMoveSpeedZ * Time.deltaTime);

                       
                        if (child.transform.position.z < targetChildZPosition)
                        {
                            Vector3 currentPos = child.transform.position;
                            currentPos.z = targetChildZPosition;
                            child.transform.position = currentPos;
                        }
                    }
                }
            }

            foreach (GameObject child in childrenToMoveY)
            {
                //Debug.Log("いけた1");

                if (child != null)
                {
                    //Debug.Log("いけた1.5");

                    if (child.transform.position.y < targetChildYPosition)
                    {
                        Vector3 currentChildYPos = child.transform.position;
                        currentChildYPos.y += childMoveSpeedY * Time.deltaTime; // Y軸プラス方向へ加算
                        child.transform.position = currentChildYPos;
                        //Debug.Log("いけた2");


                        if (child.transform.position.y > targetChildYPosition)
                        {
                            //Debug.Log("いけた3");

                            Vector3 currentPos = child.transform.position;
                            currentPos.y = targetChildYPosition;
                            child.transform.position = currentPos;
                        }
                    }
                }
            }

        }


    }
}
