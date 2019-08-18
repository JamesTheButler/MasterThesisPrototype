using UnityEngine;

public class ColliderSetter : MonoBehaviour {
    [SerializeField] private GameObject colliderGroup;
    [SerializeField] private GameObject boxColliderPrefab;

    public bool isSet = false;

    public void setInfo(MyColliderData[] colliderData) {
        for(int i =0; i<colliderData.Length; i++) {
            switch (colliderData[i].type) {
                case ColliderType.NONE:
                    break;
                case ColliderType.BOX:
                    GameObject go = Instantiate(boxColliderPrefab, colliderGroup.transform);
                    go.transform.position = colliderData[i].position;
                    go.transform.localScale = colliderData[i].size;
                    go.GetComponent<MyCollider>().setType(colliderData[i].type);
                    break;
            }
        }
        isSet = true;
    }
}