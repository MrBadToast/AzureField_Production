using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [SerializeField,LabelText("내려가는 위치값")] public float UnderLocal; 
    [SerializeField,LabelText("내려가는 속도")] public float speed;
    [SerializeField,LabelText("오브젝트 지정")] public  PuzzleDoor puzzleDoor;
    private float initialYPosition;

    private bool IsMoveDown;
   private void Awake() 
   {
        initialYPosition = transform.position.y;
   }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.layer == 6 || other.gameObject.layer == 8)
        {
            puzzleDoor = FindObjectOfType<PuzzleDoor>();
            puzzleDoor.KeyCount ++;
            Debug.Log("추가됨 " + puzzleDoor.KeyCount);
            IsMoveDown = true;
            MoveDown();
        }
    }
    private void OnTriggerStay(Collider other) 
    {
        if(other.gameObject.layer == 6 ||  other.gameObject.layer == 8)
        {
            IsMoveDown = true;
            MoveDown();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 6 || other.gameObject.layer == 8)
        {
            puzzleDoor = FindObjectOfType<PuzzleDoor>();
            puzzleDoor.KeyCount --;  
            Debug.Log("빠짐 " + puzzleDoor.KeyCount);
            IsMoveDown = false;
            MoveDown();
        }
    }

    private void MoveDown()
    {
        Vector3 newPosition = transform.position - Vector3.up * speed * Time.deltaTime;
        if(IsMoveDown)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Max(initialYPosition + UnderLocal,newPosition.y), 
            transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, Mathf.Max(initialYPosition - UnderLocal, newPosition.y), 
            transform.position.z);
        }

    }
}
