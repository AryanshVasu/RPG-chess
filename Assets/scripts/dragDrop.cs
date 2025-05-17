using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragDrop : MonoBehaviour
{
    Vector3 offset;
    Camera cam;
    Vector2 pickPosition;
    Transform pieceVisual;

    boardGraphic boardGraphic;
    void Start(){
        boardGraphic = GameObject.Find("playArea").GetComponent<boardGraphic>();
        cam = Camera.main;
        pieceVisual = transform.GetChild(0);
    }

    void OnMouseDown(){
        offset = transform.position - GetMouseWorldPos();
        pieceVisual.position += offset - new Vector3(0,0,0.01f);
        pieceVisual.localScale *= 1.2f;
        pickPosition = transform.localPosition;

        boardGraphic.PickPiece(pickPosition);
    }

    void OnMouseDrag(){
        // Move the object to follow the mouse while maintaining the offset
        transform.position = GetMouseWorldPos();
    }

    Vector3 GetMouseWorldPos(){
        // Get the mouse position in screen space
        Vector3 mousePoint = Input.mousePosition;

        // Convert it to world space
        return cam.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseUp(){
        Vector2 dropPosition = GetMouseWorldPos();
        boardGraphic.dropPiece(pickPosition, transform.localPosition + Vector3.Scale(offset,transform.localScale));
        // Optionally, you can add code here to handle when the drag ends
        
    }
}
