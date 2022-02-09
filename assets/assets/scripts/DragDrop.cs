using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class DragDrop : MonoBehaviour
{
    // event to make the enemy react
    [SerializeField] UnityEvent<ChessPieceBehavoir, int, int> onDropPiece;

    ChessPieceBehavoir pieceBehavoir;
    [SerializeField] GameObject Board;
    ChessBoardBehavoir boardBehavoir;

    int OnTop = -1;

    void OnEnable() {
        pieceBehavoir = GetComponent<ChessPieceBehavoir>();
        boardBehavoir = Board.GetComponent<ChessBoardBehavoir>();
    }

    public void OnMouseDrag() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = boardBehavoir.InfrontPosZ + OnTop;
        transform.position = mousePos;
    }

    public void OnMouseUp() {    
        int closestColumn = pieceBehavoir.Column;
        int closestRow = pieceBehavoir.Row;
        Vector3 lastValidSnapPoint = boardBehavoir.SquareCenterPosAt(closestColumn, closestRow);
        float closestDistance = Vector3.Magnitude(lastValidSnapPoint);

        // search for the closest square to the piece
        for(int i = 0; i < boardBehavoir.gridSize; i++)                                          {
            for(int j = 0; j < boardBehavoir.gridSize; j++) {
                float CurrentDistance = Vector3.Distance(transform.position, boardBehavoir.SquareCenterPosAt(i, j));
                if(CurrentDistance < closestDistance) {
                    closestDistance = CurrentDistance;
                    closestColumn = i;
                    closestRow = j;
                }
            }
        }

        // if the piece is in range and the move is valid
        //bool valid = false;
        if(closestDistance - 1 < boardBehavoir.squareCenterRange && pieceBehavoir.checkValidMoveFull(closestColumn, closestRow)) {
            pieceBehavoir.movePiece(closestColumn, closestRow);
            transform.position = boardBehavoir.SquareCenterPosAt(closestColumn, closestRow);

            /////////////////////////////////////////
            /*string a = "";
            for(int i = 0; i < boardBehavoir.gridSize; i++) {
                for(int j = 0; j < boardBehavoir.gridSize; j++) {
                    if(boardBehavoir.pieceAtpos(i,j)) {a += "X";}
                    else {a += "0";}
                }
                a += "\n";
            }
            Debug.Log("my move:\n" + a);*/
            /////////////////////////////////////////

            onDropPiece.Invoke(pieceBehavoir, closestColumn, closestRow);


            /////////////////////////////////////////
            /*a = "";
            for(int i = 0; i < boardBehavoir.gridSize; i++) {
                for(int j = 0; j < boardBehavoir.gridSize; j++) {
                    if(boardBehavoir.pieceAtpos(i,j)) {a += "X";}
                    else {a += "0";}
                }
                a += "\n";
            }
            Debug.Log("His move:\n" + a);*/
            /////////////////////////////////////////
        }
        else {transform.position =  boardBehavoir.SquareCenterPosAt(pieceBehavoir.Column, pieceBehavoir.Row);}
    }
}