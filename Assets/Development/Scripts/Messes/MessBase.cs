using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessBase : MonoBehaviour, IMessParent
{
    [field: SerializeField] public MessSO MessSO { get; private set; }

    private IMessParent messParent;
    [HideInInspector] public MessBase mess;

    public virtual void Interact(PlayerStateMachine player)
    {

    }
    public virtual void InteractAlt(PlayerStateMachine player)
    {
        
    }

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

    public void DestroyMess()
    {
        messParent.ClearMess();
        Destroy(gameObject);
    }

    public static MessBase SpawnMess(MessSO messSO , Transform spawnPosition)
    {
        GameObject messPrefab = Instantiate(messSO.prefab , spawnPosition.position , spawnPosition.rotation);
        MessBase mess = messPrefab.GetComponent<MessBase>();
       // mess.GetComponent<MessBase>().SetMessParent(messParent);
      
        return mess;
    }


    // IMessParent Interface Implementation

    public Transform GetMessTransform()
    {
        return mess.transform;
    }

    public void SetMess(MessBase mess)
    {
        this.mess = mess;
    }

    public MessBase GetMess()
    {
        return mess;
    }

    public void ClearMess()
    {
        mess = null;
    }

    public bool HasMess()
    {
        return mess != null;
    }
}
