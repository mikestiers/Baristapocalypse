using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mess : MonoBehaviour
{
    [field: SerializeField] public MessSO MessSO { get; private set; }

    private IMessParent messParent;

    public MessSO GetMessSO()
    {
        return MessSO;
    }

    public void SetMessParent(IMessParent messParent)
    {
        if (this.messParent != null)
        {
            this.messParent.ClearMess();
        }
        this.messParent = messParent;

        if (messParent.HasMess())
        {
            Debug.Log("Cleaning station already has a mess");
        }
        messParent.SetMess(this);

        transform.parent = messParent.GetMessTransform();
        transform.localPosition = Vector3.zero;
    }

    public IMessParent GetMessParent()
    {
        return messParent;
    }

    public void CleanMess()
    {
        messParent.ClearMess();
        Destroy(gameObject);
    }

    public static Mess SpawnMess(MessSO messSO, IMessParent messParent)
    {
        GameObject messPrefab = Instantiate(messSO.prefab);
        Mess mess = messPrefab.GetComponent<Mess>();
        mess.GetComponent<Mess>().SetMessParent(messParent);
      
        return mess;
    }
}
