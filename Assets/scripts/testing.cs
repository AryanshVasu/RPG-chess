// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using boardPiece;

// public class testing : MonoBehaviour
// {
//     Dictionary<Vector2, List<piece>> onBoard;
//     List<Vector2> battleFields;

//     void Start(){
//         pawn p1 = new pawn(new Vector2(1,2), true);
//         pawn p2 = new pawn(new Vector2(6,2), true);
//         pawn p3 = new pawn(new Vector2(6,1), false);
//         pawn p4 = new pawn(new Vector2(1,2), true);
//         bishop b1 = new bishop(new Vector2(3,4), true);

//         battleFields = new List<Vector2>();
        
//         onBoard = new Dictionary<Vector2, List<piece>>(){
//             {p1.pos, new List<piece>(){p1 , p4}},
//             {p2.pos, new List<piece>(){p2}},
//             {p3.pos, new List<piece>(){p3}},
//             // {p4.pos, new List<piece>(){p4}},
//             {b1.pos, new List<piece>(){b1}}
//         };

//         printList(b1.moveList);
//         b1.calculateMove(onBoard, battleFields);
//         printList(b1.moveList);
//         Move(b1,new Vector2(3,6));
//         // Debug.Log("p1 pos: " + p1.pos);
//         // Debug.Log(onBoard.ContainsKey(new Vector2(0,1)));
//         // Debug.Log(onBoard[new Vector2(3,6)][0]);
//     }

//     void Move(piece p, Vector2 pos){
//         if (onBoard[p.pos].Count == 1){
//             onBoard.Remove(p.pos);
//         }else{
//             onBoard[p.pos].Remove(p);
//         }

//         if (onBoard.TryGetValue(pos, out List<piece> pList)){
//             pList.Add(p);
//         }else{
//             onBoard.Add(pos, new List<piece>(){p});
//         }

//         p.pos = pos;
//     }

//     void printList(List<Vector2> l){
//         if (l == null) return;
//         foreach(Vector2 pos in l){
//             Debug.Log(pos);
//         }
//     }

// }
