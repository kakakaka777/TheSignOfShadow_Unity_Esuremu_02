using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMessage : MessageFunction
{
    public GameObject flyPrefab;
    


    [SerializeField] int flyKabashirasNumberMax = 6;

    


    private int flyKabashirasNumber = 0;

    private void Update()
    {
        if (canUse == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            flyKabashirasNumber += 1;

            if (PlayerID.playerID == 0)
            {
                if (flyKabashirasNumber <= flyKabashirasNumberMax)
                {
                    Instantiate(flyPrefab, player01.position, Quaternion.identity);
                }
            }
            else if (PlayerID.playerID == 1)
            {
                if (flyKabashirasNumber <= flyKabashirasNumberMax)
                {
                    Instantiate(flyPrefab, player02.position, Quaternion.identity);
                }
            }


            Debug.Log("蝿設置できる回数: " + flyKabashirasNumber + "/" + flyKabashirasNumberMax);


        }
        if (flyKabashirasNumber >= flyKabashirasNumberMax)
        {
            flyKabashirasNumber = flyKabashirasNumberMax;
        }

    }

    public override void OnActivate(Vector3 playerPosition)
    {



        flyKabashirasNumber = 0;

        

        Debug.Log("蝿を放つ（メッセージを残す機能)を実行する"); 
        

        

    }
}
