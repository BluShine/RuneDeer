using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentTransferer : MonoBehaviour {

    public Transform target;

    [BitStrap.Button]
    public void TransferComponents()
    {
        Transform[] targetArr = target.GetComponentsInChildren<Transform>();
        foreach(Transform t in transform.GetComponentsInChildren<Transform>())
        {
            Transform targetT = null;
            foreach(Transform j in targetArr)
            {
                if(j.name == t.name)
                {
                    targetT = j;
                }
            }
            if(targetT != null)
            {
                foreach(Component c in t.GetComponents<Component>())
                {
                    Debug.Log("copying " + c.name);
                    if (!(c is Transform) && !(c is SkinnedMeshRenderer) && !(c is Animator))
                    {
                        UnityEditorInternal.ComponentUtility.CopyComponent(c);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetT.gameObject);
                        Debug.Log("Copied " + c.name);
                    }
                }
            }
        }
    }
}
