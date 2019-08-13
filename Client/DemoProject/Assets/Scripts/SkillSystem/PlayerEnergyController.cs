using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyController : MonoBehaviour
{
    static float width = 19.2f;
    static float height = 10.8f;
    private NetWorkManager NetClass;
    private EnergySpherePool SphereManager;
    private YVector2 UpdateVec;
    private Vector2 LocalVec;

    private void Awake()
    {
        GameObject GameManager = GameObject.Find("GameManager");
        NetClass = GameManager.GetComponent<NetWorkManager>();
        SphereManager = GameManager.GetComponent<EnergySpherePool>();
        UpdateVec = new YVector2();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NetClass.LocalPlayer == 1)
        {
            int CurIndex = collision.gameObject.GetComponent<SphereInfo>().SphereId;
            EnergySphere SendClass = new EnergySphere() { SphereId = CurIndex };
            NetClass.SendDataToServer(SendClass, (int)Protocal.MESSAGE_COLLECT);
            SphereManager.Collect(CurIndex);

            //主客户端随机生成
            UpdateVec.X = Random.Range(-width, width);
            UpdateVec.Y = Random.Range(-height, height);
            LocalVec.x = UpdateVec.X;
            LocalVec.y = UpdateVec.Y;
            SendClass.Position = UpdateVec;
            NetClass.SendDataToServer(SendClass, (int)Protocal.MESSAGE_GENERATORENERGY);
            SphereManager.GeneratorNewSphere(CurIndex, LocalVec);
        }
    }
}
