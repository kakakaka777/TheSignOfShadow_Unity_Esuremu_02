using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMessage : MessageFunction
{
    public GameObject flyPrefab;
    public int flyCount = 10;
    public float spawnRadius = 2.0f;

    [SerializeField] int flyKabashirasNumberMax = 6;

    [SerializeField] float minScale = 0.05f;
    [SerializeField] float maxScale = 0.1f;


    private int flyKabashirasNumber = 0;

    private void Update()
    {
        if (canUse == false)
        {
            return;
        }

        

    }

    public override void OnActivate(Vector3 playerPosition)
    {

        

        flyKabashirasNumber = 0;

        Debug.Log("蝿を放つ（メッセージを残す機能)を実行する");
        if (canUse == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            flyKabashirasNumber += 1;
            if (flyKabashirasNumber <= flyKabashirasNumberMax)
            {
                GameObject fly = Instantiate(flyPrefab, playerPosition, Quaternion.identity);

            }

            Debug.Log("LeaveScarできる回数: " + flyKabashirasNumber + "/" + flyKabashirasNumberMax);


        }




    }

}
